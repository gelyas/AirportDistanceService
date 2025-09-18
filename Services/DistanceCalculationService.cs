using AirportDistanceService.Models;

namespace AirportDistanceService.Services;

/// <summary>
/// Сервис для расчета расстояния между аэропортами
/// </summary>
public class DistanceCalculationService : IDistanceCalculationService
{
    private const double EarthRadiusInMiles = 3959.0; // Радиус Земли в милях
    private const double EarthRadiusInKilometers = 6371.0; // Радиус Земли в километрах

    /// <summary>
    /// Рассчитывает расстояние между двумя аэропортами в милях
    /// </summary>
    /// <param name="fromAirport">Аэропорт отправления</param>
    /// <param name="toAirport">Аэропорт назначения</param>
    /// <returns>Расстояние в милях</returns>
    public double CalculateDistanceInMiles(Airport fromAirport, Airport toAirport)
    {
        return CalculateDistance(fromAirport, toAirport, EarthRadiusInMiles);
    }

    /// <summary>
    /// Рассчитывает расстояние между двумя аэропортами в километрах
    /// </summary>
    /// <param name="fromAirport">Аэропорт отправления</param>
    /// <param name="toAirport">Аэропорт назначения</param>
    /// <returns>Расстояние в километрах</returns>
    public double CalculateDistanceInKilometers(Airport fromAirport, Airport toAirport)
    {
        return CalculateDistance(fromAirport, toAirport, EarthRadiusInKilometers);
    }

    /// <summary>
    /// Рассчитывает расстояние между двумя точками на сфере по формуле гаверсинуса
    /// </summary>
    /// <param name="fromAirport">Аэропорт отправления</param>
    /// <param name="toAirport">Аэропорт назначения</param>
    /// <param name="earthRadius">Радиус Земли в нужных единицах измерения</param>
    /// <returns>Расстояние в указанных единицах измерения</returns>
    private static double CalculateDistance(Airport fromAirport, Airport toAirport, double earthRadius)
    {
        // Конвертируем градусы в радианы
        var lat1Rad = DegreesToRadians(fromAirport.Latitude);
        var lat2Rad = DegreesToRadians(toAirport.Latitude);
        var deltaLatRad = DegreesToRadians(toAirport.Latitude - fromAirport.Latitude);
        var deltaLonRad = DegreesToRadians(toAirport.Longitude - fromAirport.Longitude);

        // Формула гаверсинуса
        var a = Math.Sin(deltaLatRad / 2) * Math.Sin(deltaLatRad / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(deltaLonRad / 2) * Math.Sin(deltaLonRad / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadius * c;
    }

    /// <summary>
    /// Конвертирует градусы в радианы
    /// </summary>
    /// <param name="degrees">Градусы</param>
    /// <returns>Радианы</returns>
    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
