using AirportDistanceService.Middleware;
using AirportDistanceService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Airport Distance Service API", 
        Version = "v1",
        Description = "REST API для расчета расстояния между аэропортами"
    });
    
    // Включаем XML комментарии для Swagger
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Настройка HttpClient для внешнего API
builder.Services.AddHttpClient<IAirportService, AirportService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "AirportDistanceService/1.0");
});

// Регистрация сервисов
builder.Services.AddScoped<IAirportService, AirportService>();
builder.Services.AddScoped<IDistanceCalculationService, DistanceCalculationService>();

// Настройка логирования
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Настройка CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Airport Distance Service API v1");
        c.RoutePrefix = string.Empty; // Swagger UI на корневом пути
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Добавляем middleware для глобальной обработки исключений
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseRouting();
app.MapControllers();

app.Run();
