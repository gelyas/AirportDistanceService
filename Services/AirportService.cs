using AirportDistanceService.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AirportDistanceService.Services;

/// <summary>
/// Сервис для получения информации об аэропортах
/// </summary>
public class AirportService : IAirportService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AirportService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public AirportService(HttpClient httpClient, ILogger<AirportService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Получает информацию об аэропорте по IATA коду
    /// </summary>
    /// <param name="iataCode">Трехбуквенный код IATA</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Информация об аэропорте</returns>
    public async Task<Airport> GetAirportAsync(string iataCode, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Запрос информации об аэропорте {IataCode}", iataCode);

            var url = $"https://places-dev.continent.ru/airports/{iataCode.ToUpper()}";
            
            var response = await _httpClient.GetAsync(url, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = $"Не удалось получить информацию об аэропорте {iataCode}. Статус: {response.StatusCode}";
                _logger.LogError(errorMessage);
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new ArgumentException($"Аэропорт с кодом {iataCode} не найден", nameof(iataCode));
                }
                
                throw new HttpRequestException(errorMessage);
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidOperationException($"Получен пустой ответ для аэропорта {iataCode}");
            }

            var airport = JsonSerializer.Deserialize<Airport>(content, _jsonOptions);
            
            if (airport == null)
            {
                throw new InvalidOperationException($"Не удалось десериализовать данные аэропорта {iataCode}");
            }

            // Валидация полученных данных
            ValidateAirportData(airport, iataCode);

            _logger.LogInformation("Успешно получена информация об аэропорте {IataCode}: {Name}", 
                airport.Iata, airport.Name);

            return airport;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Ошибка HTTP при запросе аэропорта {IataCode}", iataCode);
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Ошибка десериализации JSON для аэропорта {IataCode}", iataCode);
            throw new InvalidOperationException($"Неверный формат данных для аэропорта {iataCode}", ex);
        }
        catch (Exception ex) when (!(ex is ArgumentException || ex is HttpRequestException || ex is InvalidOperationException))
        {
            _logger.LogError(ex, "Неожиданная ошибка при получении аэропорта {IataCode}", iataCode);
            throw;
        }
    }

    /// <summary>
    /// Валидирует данные аэропорта
    /// </summary>
    /// <param name="airport">Данные аэропорта</param>
    /// <param name="requestedIataCode">Запрошенный IATA код</param>
    private static void ValidateAirportData(Airport airport, string requestedIataCode)
    {
        if (string.IsNullOrWhiteSpace(airport.Iata))
        {
            throw new InvalidOperationException($"IATA код не указан для аэропорта {requestedIataCode}");
        }

        if (string.IsNullOrWhiteSpace(airport.Name))
        {
            throw new InvalidOperationException($"Название не указано для аэропорта {requestedIataCode}");
        }

        if (airport.Latitude == 0 && airport.Longitude == 0)
        {
            throw new InvalidOperationException($"Координаты не указаны для аэропорта {requestedIataCode}");
        }

        // Проверяем, что полученный IATA код соответствует запрошенному
        if (!string.Equals(airport.Iata, requestedIataCode, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Полученный IATA код {airport.Iata} не соответствует запрошенному {requestedIataCode}");
        }
    }
}
