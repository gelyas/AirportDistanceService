using System.ComponentModel.DataAnnotations;

namespace AirportDistanceService.Models;

/// <summary>
/// Модель запроса для расчета расстояния между аэропортами
/// </summary>
public class DistanceRequest
{
    /// <summary>
    /// Трехбуквенный код IATA аэропорта отправления
    /// </summary>
    [Required(ErrorMessage = "Код аэропорта отправления обязателен")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Код аэропорта должен содержать ровно 3 символа")]
    [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Код аэропорта должен содержать только заглавные латинские буквы")]
    public string FromAirport { get; set; } = string.Empty;

    /// <summary>
    /// Трехбуквенный код IATA аэропорта назначения
    /// </summary>
    [Required(ErrorMessage = "Код аэропорта назначения обязателен")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Код аэропорта должен содержать ровно 3 символа")]
    [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Код аэропорта должен содержать только заглавные латинские буквы")]
    public string ToAirport { get; set; } = string.Empty;
}
