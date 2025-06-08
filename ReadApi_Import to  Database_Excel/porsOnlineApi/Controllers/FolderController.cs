using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using porsOnlineApi.Extensions;
using porsOnlineApi.JsonModel;
using porsOnlineApi.Models;
using porsOnlineApi.Models.ViewModels;
using porsOnlineApi.Services;

namespace porsOnlineApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FolderController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<FolderController> _logger;
        private readonly ApiConfig _apiSettings;
        private readonly ISurveyManagementService _surveyService;
        public FolderController(ILogger<FolderController> logger, IHttpClientFactory httpClientFactory,IOptions<ApiConfig> apiSettings,
             
            ISurveyManagementService surveyService)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
            _surveyService = surveyService;
        }

        [HttpGet("Folders")]
        public async Task<IActionResult> GetAllFolder()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", _apiSettings.ApiKey);
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiSettings.BaseUrl}/folders/");

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("External API call failed with status code {StatusCode}", response.StatusCode);
                return StatusCode((int)response.StatusCode, "Failed to get data from external API");
            }
            var responseBody = await response.Content.ReadAsStringAsync();
            var surveyFolders = System.Text.Json.JsonSerializer.Deserialize<SurveyFolderCollection>(responseBody);


            // Example operations
            if (surveyFolders != null)
            {
                // Get all active surveys
                var activeSurveys = surveyFolders.GetActiveSurveys();

                // Get surveys with NPS label
                var npsSurveys = surveyFolders.GetSurveysByLabel("NPS");

                // Calculate response rates
                foreach (var survey in activeSurveys)
                {
                    var responseRate = survey.GetResponseRate();
                    Console.WriteLine($"Survey: {survey.Name}, Response Rate: {responseRate:F2}%");
                }

                // Find recently active surveys
                var recentSurveys = surveyFolders.SelectMany(f => f.Surveys)
                                                .Where(s => s.IsRecentlyActive());
            }
            return Content(responseBody, "application/json");
        }
        [HttpGet("FolderList")]
        public async Task<IActionResult> FolderList()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", _apiSettings.ApiKey);

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiSettings.BaseUrl}/folders/");

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("External API call failed with status code {StatusCode}", response.StatusCode);
                return StatusCode((int)response.StatusCode, "Failed to get data from external API");
            }
            var responseBody = await response.Content.ReadAsStringAsync();
            var surveyFolders = System.Text.Json.JsonSerializer.Deserialize<SurveyFolderCollection>(responseBody);
            return Ok(surveyFolders.Select(x => new { x.Name , x.Id}).ToList());
        }
        [HttpGet("SurveryByFolderName/{title}")]
        public async Task<IActionResult> SurveryByFolderName(string title)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", _apiSettings.ApiKey);

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiSettings.BaseUrl}/folders/");

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("External API call failed with status code {StatusCode}", response.StatusCode);
                return StatusCode((int)response.StatusCode, "Failed to get data from external API");
            }
            var responseBody = await response.Content.ReadAsStringAsync();
            var surveyFolders = System.Text.Json.JsonSerializer.Deserialize<SurveyFolderCollection>(responseBody);
            if (surveyFolders == null) return Ok(null);                

            var folds = surveyFolders.Where(x => x.Name.Contains(title)).ToList();
            if(folds==null) return Ok(null);    

            var activeSurveys = folds.SelectMany(y=>y.Surveys).Select(z=> new { z.Name,z.Id,z.Labels,z.CreatedDate});
            return Ok(activeSurveys);;

            
        }

        [HttpGet("SurveryByFolderid/{folderId}")]
        public async Task<IActionResult> SurveryByFolderid(int folderId)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", _apiSettings.ApiKey);

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiSettings.BaseUrl}/folders/");

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("External API call failed with status code {StatusCode}", response.StatusCode);
                return StatusCode((int)response.StatusCode, "Failed to get data from external API");
            }
            var responseBody = await response.Content.ReadAsStringAsync();
            var surveyFolders = System.Text.Json.JsonSerializer.Deserialize<SurveyFolderCollection>(responseBody);
            if (surveyFolders == null) return Ok(null);

            var folds = surveyFolders.FirstOrDefault(x => x.Id == folderId);
            if (folds == null) return Ok(null);

            var activeSurveys = folds.Surveys.Select(z => new { z.Name, z.Id, z.Labels, z.CreatedDate });
            return Ok(activeSurveys); 

        }

    }
}
