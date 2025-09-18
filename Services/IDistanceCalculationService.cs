using AirportDistanceService.Models;

namespace AirportDistanceService.Services;

/// <summary>
/// Интерфейс сервиса для расчета расстояния между аэропортами
/// </summary>
public interface IDistanceCalculationService
{
    /// <summary>
    /// Рассчитывает расстояние между двумя аэропортами
    /// </summary>
    /// <param name="fromAirport">Аэропорт отправления</param>
    /// <param name="toAirport">Аэропорт назначения</param>
    /// <returns>Расстояние в милях</returns>
    double CalculateDistanceInMiles(Airport fromAirport, Airport toAirport);

    /// <summary>
    /// Рассчитывает расстояние между двумя аэропортами
    /// </summary>
    /// <param name="fromAirport">Аэропорт отправления</param>
    /// <param name="toAirport">Аэропорт назначения</param>
    /// <returns>Расстояние в километрах</returns>
    double CalculateDistanceInKilometers(Airport fromAirport, Airport toAirport);
}
