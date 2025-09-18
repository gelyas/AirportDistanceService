namespace AirportDistanceService.Models;

/// <summary>
/// Модель ответа с информацией о расстоянии между аэропортами
/// </summary>
public class DistanceResponse
{
    /// <summary>
    /// Аэропорт отправления
    /// </summary>
    public Airport FromAirport { get; set; } = new();

    /// <summary>
    /// Аэропорт назначения
    /// </summary>
    public Airport ToAirport { get; set; } = new();

    /// <summary>
    /// Расстояние в милях
    /// </summary>
    public double DistanceInMiles { get; set; }

    /// <summary>
    /// Расстояние в километрах
    /// </summary>
    public double DistanceInKilometers { get; set; }

    /// <summary>
    /// Время выполнения запроса в миллисекундах
    /// </summary>
    public long ExecutionTimeMs { get; set; }
}
