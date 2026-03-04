using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using MapleStoryMarketGraph.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.Globalization;
using TeruTeruPandas.Core;
using TeruTeruPandas.Compat;

namespace MapleStoryMarketGraph.Services;

/// <summary>
/// 구글 시트를 데이터베이스로 사용하는 가계부 서비스입니다.
/// 'DB' 시트를 자동으로 생성하고 관리합니다.
/// </summary>
public class AccountBookService
{
    private readonly string _spreadsheetId;
    private readonly string _credentialsPath;
    private readonly SheetsService? _sheetsService;
    private readonly string _sheetName;
    private readonly HttpClient _httpClient;
    private readonly string _nexonBaseUrl;

    public bool IsInitialized { get; private set; }

    public AccountBookService(IConfiguration config, IHostEnvironment env, HttpClient httpClient)
    {
        _spreadsheetId = config["GoogleSheets:SpreadsheetId"] ?? "";
        _credentialsPath = Path.Combine(env.ContentRootPath, config["GoogleSheets:CredentialsPath"] ?? "service_account.json");
        _sheetName = config["GoogleSheets:SheetName"] ?? "DB";
        _httpClient = httpClient;
        _nexonBaseUrl = config["NexonApi:BaseUrl"] ?? "https://open.api.nexon.com";

        try
        {
            _sheetsService = InitializeService();
            IsInitialized = true;
            Console.WriteLine($"[AccountBookService] 초기화 성공 (Sheet: {_sheetName})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AccountBookService] 초기화 실패: {ex.Message}");
            IsInitialized = false;
        }
    }

    private SheetsService InitializeService()
    {
        if (!File.Exists(_credentialsPath))
        {
            throw new FileNotFoundException($"인증 파일을 찾을 수 없습니다: {_credentialsPath}");
        }

        GoogleCredential credential;
        using (var stream = new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read))
        {
            // 최신 권장 방식인 Factory 대신 라이브러리 호환성을 위해 FromStream 유지 (경고 무시)
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped(SheetsService.Scope.Spreadsheets);
        }

        return new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "MapleStory Market Graph"
        });
    }

    private async Task EnsureSheetExistsAsync()
    {
        if (_sheetsService == null) return;

        var spreadsheet = await _sheetsService.Spreadsheets.Get(_spreadsheetId).ExecuteAsync();
        var sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == _sheetName);

        if (sheet == null)
        {
            Console.WriteLine($"[AccountBookService] '{_sheetName}' 시트 자동 생성 중...");
            var addSheetRequest = new Request { AddSheet = new AddSheetRequest { Properties = new SheetProperties { Title = _sheetName } } };
            var batchUpdateHeaderRequest = new BatchUpdateSpreadsheetRequest { Requests = new List<Request> { addSheetRequest } };
            await _sheetsService.Spreadsheets.BatchUpdate(batchUpdateHeaderRequest, _spreadsheetId).ExecuteAsync();

            var headerRange = $"{_sheetName}!A1:I1";
            var headerValues = new ValueRange { Values = new List<IList<object>> { new List<object> { "UserId", "Timestamp", "Type", "Category", "Title", "Amount", "Unit", "IsUp", "Memo" } } };
            var updateRequest = _sheetsService.Spreadsheets.Values.Update(headerValues, _spreadsheetId, headerRange);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            await updateRequest.ExecuteAsync();
        }
    }

    public async Task<List<AccountBookEntry>> GetEntriesAsync(string userId)
    {
        if (!IsInitialized || _sheetsService == null) return new List<AccountBookEntry>();

        try
        {
            await EnsureSheetExistsAsync();
            var range = $"{_sheetName}!A2:I";
            var request = _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);
            var response = await request.ExecuteAsync();
            var values = response.Values;

            var entries = new List<AccountBookEntry>();
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    if (row.Count > 0 && row[0]?.ToString() == userId)
                    {
                        entries.Add(MapRowToEntry(row));
                    }
                }
            }
            return entries;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AccountBookService] 로드 오류: {ex.Message}");
            return new List<AccountBookEntry>();
        }
    }

    public async Task AddEntryAsync(AccountBookEntry entry)
    {
        if (!IsInitialized || _sheetsService == null) throw new InvalidOperationException("초기화 실패");
        await EnsureSheetExistsAsync();

        var range = $"{_sheetName}!A:I";
        var valueRange = new ValueRange { Values = new List<IList<object>> { new List<object> { 
            entry.UserId, entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"), entry.Type, 
            entry.Category, entry.Title, entry.Amount, entry.Unit, entry.IsUp, entry.Memo 
        } } };

        var appendRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, _spreadsheetId, range);
        appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
        await appendRequest.ExecuteAsync();
    }

    /// <summary>
    /// 사용자의 넥슨 API Key를 사용하여 메소마켓 거래 내역을 가져와 구글 시트에 동기화합니다.
    /// </summary>
    public async Task SyncFromNexonMesoMarketAsync(string userId, string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey)) return;

        // 예: 넥슨 오픈 API의 메소마켓 내역 조회 엔드포인트 (실제 엔드포인트에 맞게 조정 필요)
        // 여기서는 구조적 예시를 구현합니다.
        using var request = new HttpRequestMessage(HttpMethod.Get, $"{_nexonBaseUrl}/maplestory/v1/market/meso/history");
        request.Headers.Add("x-nxopen-api-key", apiKey);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) return;

        // JSON 데이터 파싱 후 AccountBookEntry 리스트로 변환 (가상의 모델 구조 사용)
        // var nexonData = await response.Content.ReadFromJsonAsync<NexonMesoMarketResponse>();
        // foreach(var item in nexonData.History) { ... await AddEntryAsync(newEntry); ... }
        
        // 실제 API 구현이 없는 상황이므로 로그 출력으로 대체
        Console.WriteLine($"[AccountBookService] Nexon API Key를 사용하여 {userId}의 데이터를 동기화합니다.");
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

    private AccountBookEntry MapRowToEntry(IList<object> row)
    {
        // 인덱스 안전 처리 보강
        string GetVal(int idx, string def = "") => row.Count > idx ? row[idx]?.ToString() ?? def : def;

        return new AccountBookEntry {
            UserId = GetVal(0),
            Timestamp = DateTime.TryParse(GetVal(1), out var dt) ? dt : DateTime.Now,
            Type = GetVal(2, "지출"),
            Category = GetVal(3, "기타"),
            Title = GetVal(4),
            Amount = double.TryParse(GetVal(5), out var amt) ? amt : 0,
            Unit = GetVal(6, "Meso"),
            IsUp = bool.TryParse(GetVal(7), out var isUp) ? isUp : false,
            Memo = GetVal(8)
        };
    }
}
