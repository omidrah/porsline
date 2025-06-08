using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using porsOnlineApi.Extensions;
using porsOnlineApi.JsonModel;
using porsOnlineApi.Models.ViewModels;
using porsOnlineApi.Services;

namespace porsOnlineApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SurveryController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SurveryController> _logger;
        private readonly ApiConfig _apiSettings;
        private readonly ISurveyManagementService _surveyService;

        public SurveryController(ILogger<SurveryController> logger, IHttpClientFactory httpClientFactory,IOptions<ApiConfig> apiSettings, ISurveyManagementService surveyService)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
            _surveyService = surveyService;

        }


        [HttpGet("Survery/{surveyId}")]
        public async Task<IActionResult> Survery(int surveyId)
        {
            
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", _apiSettings.ApiKey);

            //var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiSettings.BaseUrl}/v2/surveys/117083/");

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiSettings.BaseUrl}/v2/surveys/{surveyId}/");

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("External API call failed with status code {StatusCode}", response.StatusCode);
                return StatusCode((int)response.StatusCode, "Failed to get data from external API");
            }
            var responseBody = await response.Content.ReadAsStringAsync();
            var detailedSurvey = System.Text.Json.JsonSerializer.Deserialize<DetailedSurvey>(responseBody);
            if (detailedSurvey != null)
            {
               
                //Console.WriteLine($"Survey Name: {detailedSurvey.Name}");
                //Console.WriteLine($"Folder: {detailedSurvey.Folder.Name}");
                Console.WriteLine($"Language: {detailedSurvey.Language}");
                Console.WriteLine($"Status: {detailedSurvey.GetStatusDescription()}");
                Console.WriteLine($"Total Questions: {detailedSurvey.GetTotalQuestions()}");
                Console.WriteLine($"Required Questions: {detailedSurvey.GetRequiredQuestions()}");
                Console.WriteLine($"Has Welcome Page: {detailedSurvey.HasWelcomePage()}");
                Console.WriteLine($"Has Appreciation Page: {detailedSurvey.HasAppreciationPage()}");

                // Analyze questions
                var multipleChoiceQuestions = detailedSurvey.GetMultipleChoiceQuestions();
                var textQuestions = detailedSurvey.GetTextQuestions();
                var ratingQuestions = detailedSurvey.GetRatingQuestions();

                Console.WriteLine($"Multiple Choice Questions: {multipleChoiceQuestions.Count()}");
                Console.WriteLine($"Text/Numeric Questions: {textQuestions.Count()}");
                Console.WriteLine($"Rating Questions: {ratingQuestions.Count()}");

                // Detailed question analysis
                foreach (var question in detailedSurvey.Questions)
                {
                    Console.WriteLine($"Question: {question.Title}");
                    Console.WriteLine($"Type: {question.GetQuestionTypeDescription()}");
                    Console.WriteLine($"Required: {question.AnswerRequired}");

                    if (question.HasChoices())
                    {
                        Console.WriteLine($"Number of Choices: {question.GetChoiceCount()}");
                        Console.WriteLine($"Allows Multiple Selection: {question.AllowsMultipleSelection()}");

                        foreach (var choice in question.Choices!)
                        {
                            Console.WriteLine($"  - {choice.Name}");
                        }
                    }

                    if (question.IsRatingScale())
                    {
                        Console.WriteLine($"Rating Steps: {question.Steps}");
                    }

                    if (question.IsTextInput())
                    {
                        Console.WriteLine($"Answer Type: {question.AnswerType}");
                        if (question.NumberMaxValue.HasValue)
                            Console.WriteLine($"Max Value: {question.NumberMaxValue}");
                        if (question.NumberMinValue.HasValue)
                            Console.WriteLine($"Min Value: {question.NumberMinValue}");
                    }

                    Console.WriteLine("---");
                }

                // Survey settings analysis
                var settings = detailedSurvey.Settings;
                Console.WriteLine("Survey Settings:");
                Console.WriteLine($"Authentication Required: {settings.AuthenticationNeeded}");
                Console.WriteLine($"Edit Response Enabled: {settings.EditResponseEnabled}");
                Console.WriteLine($"Local Storage Enabled: {settings.LocalStorageIsEnabled}");

                // Theme analysis
                var theme = detailedSurvey.Theme;
                Console.WriteLine("Theme Settings:");
                Console.WriteLine($"Background Color: {theme.BackgroundColor}");
                Console.WriteLine($"Question Color: {theme.QuestionColor}");
                Console.WriteLine($"Button Color: {theme.ButtonColor}");
                Console.WriteLine($"Font Size: {theme.FontSize}");
                Console.WriteLine($"Is Public Theme: {theme.IsPublic}");
            }
           
            return Content(responseBody, "application/json");
        }
        [HttpGet("ResultOfSurvey/{surveyId}")]
        public async Task<IActionResult> ResultOfSurveys(int surveyId)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", _apiSettings.ApiKey);
            //var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiSettings.BaseUrl}/v2/surveys/117083/responses/results-table/");

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiSettings.BaseUrl}/v2/surveys/{surveyId}/responses/results-table/");

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("External API call failed with status code {StatusCode}", response.StatusCode);
                return StatusCode((int)response.StatusCode, "Failed to get data from external API");
            }
            var responseBody = await response.Content.ReadAsStringAsync();      
            return Content(responseBody, "application/json");
        }

    }
}
