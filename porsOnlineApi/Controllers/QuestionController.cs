using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using porsOnlineApi.Models.ViewModels;

namespace porsOnlineApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<QuestionController> _logger;
        private readonly ApiConfig _apiSettings;

        public QuestionController(ILogger<QuestionController> logger, IHttpClientFactory httpClientFactory, IOptions<ApiConfig> apiSettings)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;

        }

        [HttpGet("QuestionInfo/{surveyId}/{questionId}")]
        public async Task<IActionResult> QuestionInfo(int surveryId, int questionId)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", _apiSettings.ApiKey);
            //surveyId = 117083  , QuestionId = 2728642
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiSettings.BaseUrl}/v2/surveys/{surveryId}/questions/{questionId}/");

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("External API call failed with status code {StatusCode}", response.StatusCode);
                return StatusCode((int)response.StatusCode, "Failed to get data from external API");
            }
            var responseBody = await response.Content.ReadAsStringAsync();
            //var result = JsonSerializer.Deserialize<myResponseModel>(responseBody);
            //return Ok(result);
            return Content(responseBody, "application/json");
        }

    }
}
