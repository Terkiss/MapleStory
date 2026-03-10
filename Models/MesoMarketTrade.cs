using System;
using System.ComponentModel.DataAnnotations;

namespace MapleStoryMarketGraph.Models;

/// <summary>
/// 메소마켓 거래 내역을 나타내는 데이터 모델입니다. (SQLite 저장용)
/// </summary>
public class MesoMarketTrade
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>
    /// 거래 유형: 매수 (Meso -> Point) / 매도 (Point -> Meso)
    /// </summary>
    [Required]
    public string TradeType { get; set; } = "매수";

    /// <summary>
    /// 거래된 메소 수량
    /// </summary>
    [Required]
    public double MesoAmount { get; set; }

    /// <summary>
    /// 거래된 메이플포인트 금액
    /// </summary>
    [Required]
    public double PointAmount { get; set; }

    /// <summary>
    /// 1억당 포인트 가격 (계산 필드)
    /// </summary>
    public double UnitPrice { get; set; }

    public string Memo { get; set; } = string.Empty;
}
