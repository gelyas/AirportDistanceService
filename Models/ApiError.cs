namespace AirportDistanceService.Models;

/// <summary>
/// Модель ошибки API
/// </summary>
public class ApiError
{
    /// <summary>
    /// Код ошибки
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Дополнительные детали ошибки
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Время возникновения ошибки
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
