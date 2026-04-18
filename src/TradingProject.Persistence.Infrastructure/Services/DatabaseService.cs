using Microsoft.Extensions.Options;
using Npgsql;
using TradingProject.Persistence.Application.Abstractions;
using TradingProject.Persistence.Infrastructure.Settings;

namespace TradingProject.Persistence.Infrastructure.Services;

public class DatabaseService(IOptions<TradingConnectionSettings> settings) : IDatabaseService
{
    private NpgsqlConnection GetConnection() => new(
        $"Host={settings.Value.Host};Port={settings.Value.Port};Database=trading;Username=trading;Password={settings.Value.Password}");

    public int GetOpenPositionsCount()
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM trades WHERE status='open'", conn);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public double GetDailyPnl()
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new NpgsqlCommand(
            "SELECT COALESCE(SUM(pnl_usdt),0) FROM trades WHERE status='closed' AND close_at >= DATE_TRUNC('day',NOW())", conn);
        return Convert.ToDouble(cmd.ExecuteScalar());
    }

    public double GetTotalPnl()
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new NpgsqlCommand(
            "SELECT COALESCE(SUM(pnl_usdt),0) FROM trades WHERE status='closed'", conn);
        return Convert.ToDouble(cmd.ExecuteScalar());
    }

    public Stats GetStats()
    {
        using var conn = GetConnection();
        conn.Open();

        double pnlDay = 0, pnlWeek = 0, pnlMonth = 0, pnlTotal = 0;
        int cDay = 0, cWeek = 0, cMonth = 0, cTotal = 0, wins = 0;

        foreach (var (label, trunc) in new[] { ("day", "day"), ("week", "week"), ("month", "month") })
        {
            using var cmd = new NpgsqlCommand(
                $"SELECT COALESCE(SUM(pnl_usdt),0), COUNT(*) FROM trades WHERE status='closed' AND close_at >= DATE_TRUNC('{trunc}',NOW())", conn);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) continue;
            var pnl = Convert.ToDouble(r[0]);
            var cnt = Convert.ToInt32(r[1]);
            switch (label)
            {
                case "day":   pnlDay   = pnl; cDay   = cnt; break;
                case "week":  pnlWeek  = pnl; cWeek  = cnt; break;
                case "month": pnlMonth = pnl; cMonth = cnt; break;
            }
        }

        using (var cmd = new NpgsqlCommand("SELECT COALESCE(SUM(pnl_usdt),0), COUNT(*) FROM trades WHERE status='closed'", conn))
        using (var r = cmd.ExecuteReader())
        {
            if (r.Read()) { pnlTotal = Convert.ToDouble(r[0]); cTotal = Convert.ToInt32(r[1]); }
        }

        using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM trades WHERE status='closed' AND pnl_usdt > 0", conn))
            wins = Convert.ToInt32(cmd.ExecuteScalar());

        return new Stats(pnlDay, pnlWeek, pnlMonth, pnlTotal, cDay, cWeek, cMonth, cTotal, wins,
            cTotal > 0 ? wins * 100.0 / cTotal : 0);
    }

    public PnlSummary GetPnlSummary()
    {
        using var conn = GetConnection();
        conn.Open();

        double Query(string trunc)
        {
            using var cmd = new NpgsqlCommand(
                "SELECT COALESCE(SUM(pnl_usdt),0) FROM trades WHERE status='closed'" +
                (trunc != "all" ? $" AND close_at >= DATE_TRUNC('{trunc}',NOW())" : ""), conn);
            return Convert.ToDouble(cmd.ExecuteScalar());
        }

        return new PnlSummary(Query("day"), Query("week"), Query("month"), Query("all"));
    }

    public List<OpenPosition> GetOpenPositions()
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new NpgsqlCommand(
            "SELECT symbol, side, price, quantity, usdt_value, stop_loss, take_profit, ai_score, created_at " +
            "FROM trades WHERE status='open' ORDER BY created_at DESC", conn);
        using var r = cmd.ExecuteReader();
        var list = new List<OpenPosition>();
        while (r.Read())
            list.Add(new OpenPosition(
                r.GetString(0), r.GetString(1),
                r.GetDouble(2), r.GetDouble(3), r.GetDouble(4),
                r.IsDBNull(5) ? null : r.GetDouble(5),
                r.IsDBNull(6) ? null : r.GetDouble(6),
                r.IsDBNull(7) ? null : r.GetInt32(7),
                r.GetDateTime(8)));
        return list;
    }

    public List<ClosedTrade> GetLastTrades(int limit = 5)
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new NpgsqlCommand(
            $"SELECT symbol, side, price, close_price, pnl_usdt, pnl_pct, ai_score, created_at, close_at " +
            $"FROM trades WHERE status='closed' ORDER BY close_at DESC LIMIT {limit}", conn);
        using var r = cmd.ExecuteReader();
        var list = new List<ClosedTrade>();
        while (r.Read())
            list.Add(new ClosedTrade(
                r.GetString(0), r.GetString(1),
                r.GetDouble(2),
                r.IsDBNull(3) ? 0 : r.GetDouble(3),
                r.IsDBNull(4) ? 0 : r.GetDouble(4),
                r.IsDBNull(5) ? 0 : r.GetDouble(5),
                r.IsDBNull(6) ? null : r.GetInt32(6),
                r.GetDateTime(7),
                r.IsDBNull(8) ? DateTime.MinValue : r.GetDateTime(8)));
        return list;
    }
    public void LogTradeOpen(OpenPosition trade)
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new NpgsqlCommand(
            "INSERT INTO trades (symbol, side, status, price, quantity, usdt_value, stop_loss, take_profit, ai_score, created_at) " +
            "VALUES (@s, @side, 'open', @p, @q, @v, @sl, @tp, @ai, @dt)", conn);
        cmd.Parameters.AddWithValue("s", trade.Symbol);
        cmd.Parameters.AddWithValue("side", trade.Side);
        cmd.Parameters.AddWithValue("p", trade.Entry);
        cmd.Parameters.AddWithValue("q", trade.Quantity);
        cmd.Parameters.AddWithValue("v", trade.UsdtValue);
        cmd.Parameters.AddWithValue("sl", trade.StopLoss ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("tp", trade.TakeProfit ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("ai", trade.AiScore ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("dt", trade.CreatedAt);
        cmd.ExecuteNonQuery();
    }

    public void LogTradeClose(int tradeId, double closePrice, double pnlUsdt, double pnlPct, string reason)
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new NpgsqlCommand(
            "UPDATE trades SET status='closed', close_price=@cp, pnl_usdt=@pnl, pnl_pct=@pct, close_at=NOW(), ai_reason=@r WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("cp", closePrice);
        cmd.Parameters.AddWithValue("pnl", pnlUsdt);
        cmd.Parameters.AddWithValue("pct", pnlPct);
        cmd.Parameters.AddWithValue("r", reason);
        cmd.Parameters.AddWithValue("id", tradeId);
        cmd.ExecuteNonQuery();
    }

    public void UpdateTakeProfit(int tradeId, double takeProfit)
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new NpgsqlCommand("UPDATE trades SET take_profit=@tp WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("tp", takeProfit);
        cmd.Parameters.AddWithValue("id", tradeId);
        cmd.ExecuteNonQuery();
    }

    public void LogOpportunity(OpportunityData opportunity)
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new NpgsqlCommand(
            "INSERT INTO opportunities (symbol, score, reason, price, created_at) VALUES (@s, @score, @r, @p, NOW())", conn);
        cmd.Parameters.AddWithValue("s", opportunity.Symbol);
        cmd.Parameters.AddWithValue("score", opportunity.Score);
        cmd.Parameters.AddWithValue("r", opportunity.Reason);
        cmd.Parameters.AddWithValue("p", opportunity.Price);
        cmd.ExecuteNonQuery();
    }

    public void LogPortfolioSnapshot(PortfolioData portfolio)
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new NpgsqlCommand(
            "INSERT INTO portfolio_snapshots (free_usdt, total_usdt, created_at) VALUES (@f, @t, NOW())", conn);
        cmd.Parameters.AddWithValue("f", portfolio.FreeUsdt);
        cmd.Parameters.AddWithValue("t", portfolio.TotalUsdt);
        cmd.ExecuteNonQuery();
    }
}
