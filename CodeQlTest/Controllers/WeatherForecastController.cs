using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace CodeQlTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [ApiController]
        [Route("api/[controller]")]
        public class ValuesController : ControllerBase
        {
            [HttpPost]
            public IActionResult Post([FromBody] string userInput)
            {
                return Ok(new { UserInput = userInput }); // Intentionally returning user input without proper validation
            }
        }

        public double Divide(double numerator, double denominator)
        {
            return numerator / denominator;
        }
    }
}
