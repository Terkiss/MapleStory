using TeruTeruPandas.Core;
using TeruTeruPandas.IO;
using System.Collections.Concurrent;

namespace MapleStoryMarketGraph.Services;

/// <summary>
/// 메이플스토리 시장 데이터를 로드하고 관리하는 싱글톤 서비스입니다.
/// 자체 구현 엔진인 TeruTeruPandas를 사용하여 CSV 데이터를 메모리에 상주시켜 고속 스위칭을 지원합니다.
/// </summary>
public class MarketDataService
{
    // TeruTeruPandas의 핵심 객체: 여러 DataFrame(테이블)을 관리하는 컨테이너
    private readonly DataUniverse _universe = new();

    // 데이터가 위치한 디렉토리 경로
    private readonly string _dataDir;

    // 지원하는 타임프레임(시간봉) 리스트
    private readonly string[] _timeframes = { "5분", "10분", "30분", "1시간", "3시간", "6시간", "12시간", "24시간" };

    public MarketDataService()
    {
        // 프로젝트 루트 기준 data 폴더 경로를 설정합니다 (참조 방식)
        // bin/Debug/net9.0/ 에서 4단계 위가 workspace root이며, 그곳의 data 폴더를 참조함
        _dataDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data"));
        Console.WriteLine("커서 :: " + _dataDir);
        // 만약 위 경로에 데이터가 없다면 대체 경로 확인 (Project root 기준)
        if (!Directory.Exists(_dataDir))
        {
            _dataDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../data"));
        }

        // 최종적으로 bin 폴더 내부도 확인
        if (!Directory.Exists(_dataDir))
        {
            _dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
        }

        // 서비스 생성 시 모든 데이터를 즉시 메모리에 로딩합니다.
        InitializeData();
    }

    /// <summary>
    /// 모든 타임프레임의 CSV 파일을 순회하며 읽어와 DataUniverse에 등록합니다.
    /// </summary>
    private void InitializeData()
    {
        foreach (var tf in _timeframes)
        {
            string filePath = Path.Combine(_dataDir, $"{tf}.csv");
            if (File.Exists(filePath))
            {
                // 엔진의 CsvReader를 사용하여 CSV -> DataFrame 변환
                var df = CsvReader.ReadCsv(filePath);

                // DataUniverse에 테이블 이름(봉 단위)으로 저장
                _universe.AddTable(tf, df);
                Console.WriteLine($"[MarketDataService] 데이터 로드 완료: {tf} ({df.RowCount} 행)");
            }
            else
            {
                Console.WriteLine($"[MarketDataService] 경고: 파일을 찾을 수 없습니다: {filePath}");
            }
        }
    }

    /// <summary>
    /// 요청한 타임프레임의 DataFrame 객체를 반환합니다.
    /// </summary>
    /// <param name="timeframe">"5분", "1시간" 등</param>
    public DataFrame GetDataFrame(string timeframe)
    {
        return _universe.GetTable(timeframe);
    }

    /// <summary>
    /// 사용 가능한 모든 타임프레임 목록을 반환합니다.
    /// </summary>
    public string[] GetAvailableTimeframes() => _timeframes;
}
