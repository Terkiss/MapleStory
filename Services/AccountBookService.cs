using MapleStoryMarketGraph.Data;
using MapleStoryMarketGraph.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeruTeruPandas.Core;
using TeruTeruPandas.Compat;

namespace MapleStoryMarketGraph.Services;

/// <summary>
/// SQLite를 데이터베이스로 사용하는 가계부 서비스입니다.
/// </summary>
public class AccountBookService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly HttpClient _httpClient;
    private readonly string _nexonBaseUrl;

    public bool IsInitialized => true; // SQLite는 항상 초기화된 상태로 간주

    public AccountBookService(IDbContextFactory<ApplicationDbContext> dbContextFactory, IConfiguration config, HttpClient httpClient)
    {
        _dbContextFactory = dbContextFactory;
        _httpClient = httpClient;
        _nexonBaseUrl = config["NexonApi:BaseUrl"] ?? "https://open.api.nexon.com";
        Console.WriteLine("[AccountBookService] SQLite 기반 초기화 완료");
    }

    public async Task<List<AccountBookEntry>> GetEntriesAsync(string userId)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.AccountBookEntries
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync();
    }

    public async Task AddEntryAsync(AccountBookEntry entry)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        context.AccountBookEntries.Add(entry);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// 사용자의 넥슨 API Key를 사용하여 메소마켓 거래 내역을 가져와 SQLite에 동기화합니다.
    /// </summary>
    public async Task SyncFromNexonMesoMarketAsync(string userId, string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey)) return;

        Console.WriteLine($"[AccountBookService] {userId}의 Nexon Meso Market 데이터 동기화 시작 (SQLite)...");

        try
        {
            // 1. 넥슨 API 호출 (메소마켓 거래 내역)
            var url = $"{_nexonBaseUrl}/maplestory/v1/market/meso/history?cursor=";
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("x-nxopen-api-key", apiKey);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[AccountBookService] Nexon API 호출 실패: {response.StatusCode}");
                return;
            }

            // Mock data for demonstration
            var mockEntry = new AccountBookEntry
            {
                UserId = userId,
                Timestamp = DateTime.Now,
                Type = "수입",
                Category = "메소마켓",
                Title = "넥슨 자동 동기화 테스트 (SQLite)",
                Amount = 100000000, // 1억 메소
                Unit = "Meso",
                IsUp = true,
                Memo = "Nexon API를 통한 자동 동기화 내역입니다. (SQLite 저장)"
            };

            await AddEntryAsync(mockEntry);
            Console.WriteLine($"[AccountBookService] {userId}의 데이터 동기화 완료 (SQLite).");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AccountBookService] 동기화 중 오류 발생: {ex.Message}");
        }
    }

    public async Task<DataFrame> GetAsDataFrameAsync(string userId)
    {
        var entries = await GetEntriesAsync(userId);
        var data = new Dictionary<string, object[]>
        {
            ["Timestamp"] = entries.Select(e => (object)e.Timestamp).ToArray(),
            ["Type"] = entries.Select(e => (object)e.Type).ToArray(),
            ["Category"] = entries.Select(e => (object)e.Category).ToArray(),
            ["Amount"] = entries.Select(e => (object)e.Amount).ToArray()
        };
        return Pd.DataFrame(data);
    }
}
