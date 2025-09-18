using AirportDistanceService.Models;

namespace AirportDistanceService.Services;

/// <summary>
/// Интерфейс сервиса для работы с аэропортами
/// </summary>
public interface IAirportService
{
    /// <summary>
    /// Получает информацию об аэропорте по IATA коду
    /// </summary>
    /// <param name="iataCode">Трехбуквенный код IATA</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Информация об аэропорте</returns>
    Task<Airport> GetAirportAsync(string iataCode, CancellationToken cancellationToken = default);
}
