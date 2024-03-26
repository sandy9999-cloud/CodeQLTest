using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

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

        private const string WeakKey = "12345678"; // Weak key for illustration purposes only. Do not use in production.

        [HttpPost("encrypt")]
        public IActionResult Encrypt([FromBody] string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
                return BadRequest("Plaintext cannot be empty.");

            byte[] encryptedBytes = EncryptString(plaintext, WeakKey);
            string encryptedText = Convert.ToBase64String(encryptedBytes);

            return Ok(new { EncryptedText = encryptedText });
        }

        [HttpPost("decrypt")]
        public IActionResult Decrypt([FromBody] string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return BadRequest("Encrypted text cannot be empty.");

          try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                string decryptedText = DecryptString(encryptedBytes, WeakKey);

                return Ok(new { DecryptedText = decryptedText });
            }
            catch (FormatException)
            {
                return BadRequest("Invalid base64 encoded string.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Decryption failed: {ex.Message}");
            }
        }

        private byte[] EncryptString(string plaintext, string key)
        {
            using (DESCryptoServiceProvider desCryptoProvider = new DESCryptoServiceProvider())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(plaintext);
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);

                desCryptoProvider.Key = keyBytes;
                desCryptoProvider.IV = keyBytes;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, desCryptoProvider.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(inputBytes, 0, inputBytes.Length);
                        cryptoStream.FlushFinalBlock();

                        return memoryStream.ToArray();
                    }
                }
            }
        }

        private string DecryptString(byte[] cipherText, string key)
        {
            using (DESCryptoServiceProvider desCryptoProvider = new DESCryptoServiceProvider())
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);

                desCryptoProvider.Key = keyBytes;
                desCryptoProvider.IV = keyBytes;

                using (MemoryStream memoryStream = new MemoryStream(cipherText))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, desCryptoProvider.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

    }
}
