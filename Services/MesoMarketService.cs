using MapleStoryMarketGraph.Data;
using MapleStoryMarketGraph.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapleStoryMarketGraph.Services;

/// <summary>
/// SQLite를 사용하여 메소마켓 거래 내역을 관리하는 서비스입니다.
/// </summary>
public class MesoMarketService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public MesoMarketService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    /// <summary>
    /// 새로운 메소마켓 거래를 추가합니다.
    /// </summary>
    public async Task AddTradeAsync(MesoMarketTrade trade)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();

        context.MesoMarketTrades.Add(trade);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// 특정 사용자의 모든 거래 내역을 최신순으로 가져옵니다.
    /// </summary>
    public async Task<List<MesoMarketTrade>> GetTradesAsync(string userId)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.MesoMarketTrades
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync();
    }

    /// <summary>
    /// 특정 사용자의 요약 통계를 계산합니다.
    /// </summary>
    public async Task<(double TotalBuyMeso, double TotalSellMeso, double AvgBuyPrice, double AvgSellPrice)> GetStatisticsAsync(string userId)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        var trades = await context.MesoMarketTrades
            .Where(t => t.UserId == userId)
            .ToListAsync();

        var buyTrades = trades.Where(t => t.TradeType == "매수").ToList();
        var sellTrades = trades.Where(t => t.TradeType == "매도").ToList();

        double totalBuyMeso = buyTrades.Sum(t => t.MesoAmount);
        double totalSellMeso = sellTrades.Sum(t => t.MesoAmount);

        double avgBuyPrice = buyTrades.Any() ? buyTrades.Average(t => t.UnitPrice) : 0;
        double avgSellPrice = sellTrades.Any() ? sellTrades.Average(t => t.UnitPrice) : 0;

        return (totalBuyMeso, totalSellMeso, avgBuyPrice, avgSellPrice);
    }

    /// <summary>
    /// 거래 내역을 삭제합니다.
    /// </summary>
    public async Task DeleteTradeAsync(int id)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        var trade = await context.MesoMarketTrades.FindAsync(id);
        if (trade != null)
        {
            context.MesoMarketTrades.Remove(trade);
            await context.SaveChangesAsync();
        }
    }
}
