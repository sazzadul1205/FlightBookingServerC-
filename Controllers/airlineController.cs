using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FlightBooking.Controllers;

[ApiController]
[Route("[controller]")]
public class AirlineController : ControllerBase
{

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AirlineController> _logger;

    public AirlineController(IHttpClientFactory httpClientFactory, ILogger<AirlineController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }


    [HttpGet]
    public async Task<ActionResult> GetAirline()
    {

        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(
                $"https://uthaotrip.com/api/api/GetAirLines"
            );

            // if (!response.IsSuccessStatusCode)
            // {
            //     _logger.LogError($"External API error: {response.StatusCode}");
            //     return StatusCode((int)response.StatusCode, new
            //     {
            //         error = "External API request failed",
            //         details = response.ReasonPhrase
            //     });
            // }

            // Read and return the raw JSON string
            var content = await response.Content.ReadAsStringAsync();

            // Return the JSON string as content with proper content type
            return Content(content, "application/json");

        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed");
            return StatusCode(500, new { error = "Network error occurred" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred");
            return StatusCode(500, new { error = "An unexpected error occurred" });
        }
    }
}