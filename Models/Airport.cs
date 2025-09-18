using System.Text.Json.Serialization;

namespace AirportDistanceService.Models;

/// <summary>
/// Модель аэропорта с информацией о местоположении
/// </summary>
public class Airport
{
    /// <summary>
    /// Трехбуквенный код IATA аэропорта
    /// </summary>
    [JsonPropertyName("iata")]
    public string Iata { get; set; } = string.Empty;

    /// <summary>
    /// Название аэропорта
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Город, в котором расположен аэропорт
    /// </summary>
    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Страна, в которой расположен аэропорт
    /// </summary>
    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Широта аэропорта
    /// </summary>
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    /// <summary>
    /// Долгота аэропорта
    /// </summary>
    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }
}
