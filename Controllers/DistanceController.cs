using AirportDistanceService.Models;
using AirportDistanceService.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AirportDistanceService.Controllers;

/// <summary>
/// Контроллер для расчета расстояния между аэропортами
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DistanceController : ControllerBase
{
    private readonly IAirportService _airportService;
    private readonly IDistanceCalculationService _distanceCalculationService;
    private readonly ILogger<DistanceController> _logger;

    public DistanceController(
        IAirportService airportService,
        IDistanceCalculationService distanceCalculationService,
        ILogger<DistanceController> logger)
    {
        _airportService = airportService;
        _distanceCalculationService = distanceCalculationService;
        _logger = logger;
    }

    /// <summary>
    /// Рассчитывает расстояние между двумя аэропортами
    /// </summary>
    /// <param name="request">Запрос с кодами аэропортов</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Информация о расстоянии между аэропортами</returns>
    /// <response code="200">Расстояние успешно рассчитано</response>
    /// <response code="400">Некорректные параметры запроса</response>
    /// <response code="404">Один из аэропортов не найден</response>
    /// <response code="500">Внутренняя ошибка сервера</response>
    [HttpPost("calculate")]
    [ProducesResponseType(typeof(DistanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CalculateDistance(
        [FromBody] DistanceRequest request,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            _logger.LogInformation("Запрос расчета расстояния между аэропортами {FromAirport} и {ToAirport}", 
                request.FromAirport, request.ToAirport);

            // Валидация входных данных
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = new ApiError
                {
                    ErrorCode = "VALIDATION_ERROR",
                    Message = "Ошибка валидации входных данных",
                    Details = string.Join("; ", errors)
                };

                _logger.LogWarning("Ошибка валидации: {Errors}", string.Join("; ", errors));
                return BadRequest(errorResponse);
            }

            // Проверка, что аэропорты разные
            if (string.Equals(request.FromAirport, request.ToAirport, StringComparison.OrdinalIgnoreCase))
            {
                var errorResponse = new ApiError
                {
                    ErrorCode = "SAME_AIRPORTS",
                    Message = "Аэропорты отправления и назначения не могут быть одинаковыми"
                };

                _logger.LogWarning("Попытка расчета расстояния между одинаковыми аэропортами: {Airport}", 
                    request.FromAirport);
                return BadRequest(errorResponse);
            }

            // Получаем информацию об аэропортах параллельно
            var fromAirportTask = _airportService.GetAirportAsync(request.FromAirport, cancellationToken);
            var toAirportTask = _airportService.GetAirportAsync(request.ToAirport, cancellationToken);

            await Task.WhenAll(fromAirportTask, toAirportTask);

            var fromAirport = await fromAirportTask;
            var toAirport = await toAirportTask;

            // Рассчитываем расстояние
            var distanceInMiles = _distanceCalculationService.CalculateDistanceInMiles(fromAirport, toAirport);
            var distanceInKilometers = _distanceCalculationService.CalculateDistanceInKilometers(fromAirport, toAirport);

            stopwatch.Stop();

            var response = new DistanceResponse
            {
                FromAirport = fromAirport,
                ToAirport = toAirport,
                DistanceInMiles = Math.Round(distanceInMiles, 2),
                DistanceInKilometers = Math.Round(distanceInKilometers, 2),
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds
            };

            _logger.LogInformation("Расстояние между {FromAirport} и {ToAirport}: {DistanceMiles} миль ({DistanceKm} км). Время выполнения: {ExecutionTime}мс",
                request.FromAirport, request.ToAirport, response.DistanceInMiles, response.DistanceInKilometers, response.ExecutionTimeMs);

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Аэропорт не найден при расчете расстояния между {FromAirport} и {ToAirport}", 
                request.FromAirport, request.ToAirport);

            var errorResponse = new ApiError
            {
                ErrorCode = "AIRPORT_NOT_FOUND",
                Message = ex.Message
            };

            return NotFound(errorResponse);
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Ошибка HTTP при расчете расстояния между {FromAirport} и {ToAirport}", 
                request.FromAirport, request.ToAirport);

            var errorResponse = new ApiError
            {
                ErrorCode = "EXTERNAL_SERVICE_ERROR",
                Message = "Ошибка при обращении к внешнему сервису аэропортов",
                Details = ex.Message
            };

            return StatusCode(StatusCodes.Status502BadGateway, errorResponse);
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            _logger.LogWarning("Операция отменена при расчете расстояния между {FromAirport} и {ToAirport}", 
                request.FromAirport, request.ToAirport);

            var errorResponse = new ApiError
            {
                ErrorCode = "OPERATION_CANCELLED",
                Message = "Операция была отменена"
            };

            return StatusCode(StatusCodes.Status408RequestTimeout, errorResponse);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Неожиданная ошибка при расчете расстояния между {FromAirport} и {ToAirport}", 
                request.FromAirport, request.ToAirport);

            var errorResponse = new ApiError
            {
                ErrorCode = "INTERNAL_SERVER_ERROR",
                Message = "Внутренняя ошибка сервера",
                Details = ex.Message
            };

            return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        }
    }

    /// <summary>
    /// Получает информацию об аэропорте по IATA коду
    /// </summary>
    /// <param name="iataCode">Трехбуквенный код IATA</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Информация об аэропорте</returns>
    /// <response code="200">Информация об аэропорте получена</response>
    /// <response code="400">Некорректный код аэропорта</response>
    /// <response code="404">Аэропорт не найден</response>
    /// <response code="500">Внутренняя ошибка сервера</response>
    [HttpGet("airport/{iataCode}")]
    [ProducesResponseType(typeof(Airport), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAirport(
        [FromRoute] [Required] [StringLength(3, MinimumLength = 3)] [RegularExpression(@"^[A-Z]{3}$")] string iataCode,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Запрос информации об аэропорте {IataCode}", iataCode);

            var airport = await _airportService.GetAirportAsync(iataCode.ToUpper(), cancellationToken);

            _logger.LogInformation("Успешно получена информация об аэропорте {IataCode}", iataCode);

            return Ok(airport);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Аэропорт {IataCode} не найден", iataCode);

            var errorResponse = new ApiError
            {
                ErrorCode = "AIRPORT_NOT_FOUND",
                Message = ex.Message
            };

            return NotFound(errorResponse);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Ошибка HTTP при получении аэропорта {IataCode}", iataCode);

            var errorResponse = new ApiError
            {
                ErrorCode = "EXTERNAL_SERVICE_ERROR",
                Message = "Ошибка при обращении к внешнему сервису аэропортов",
                Details = ex.Message
            };

            return StatusCode(StatusCodes.Status502BadGateway, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Неожиданная ошибка при получении аэропорта {IataCode}", iataCode);

            var errorResponse = new ApiError
            {
                ErrorCode = "INTERNAL_SERVER_ERROR",
                Message = "Внутренняя ошибка сервера",
                Details = ex.Message
            };

            return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        }
    }
}
