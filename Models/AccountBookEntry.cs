using System;

namespace MapleStoryMarketGraph.Models;

/// <summary>
/// 가계부의 단일 거래 내역을 나타내는 데이터 모델입니다.
/// </summary>
public class AccountBookEntry
{
    public string UserId { get; set; } = string.Empty; // 사용자 식별자
    public DateTime Timestamp { get; set; } = DateTime.Now; // 거래 일시
    public string Type { get; set; } = "지출"; // 수입 / 지출
    public string Category { get; set; } = "기타"; // 거래 카테고리
    public string Title { get; set; } = string.Empty; // 거래 요약
    public double Amount { get; set; } // 거래 금액
    public string Unit { get; set; } = "Meso"; // 화폐 단위
    public bool IsUp { get; set; } = false; // UI용 지표 (상승/하락)
    public string Memo { get; set; } = string.Empty; // 추가 메모
}
