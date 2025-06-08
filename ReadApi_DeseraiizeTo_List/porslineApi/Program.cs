using porsOnlineApi.JsonModel;
using System.Text.Json;

public class FolderSurveyItem
{
    public int FolderId { get; set; }
    public string FolderName { get; set; }
    public int FolderOrder { get; set; }
    public SharedBy FolderSharedBy { get; set; }
    public List<SharedWith> FolderSharedWith { get; set; }

    public int? SurveyId { get; set; }
    public string SurveyName { get; set; }
    public string SurveyLanguage { get; set; }
    public DateTime? SurveyCreatedDate { get; set; }
    public bool? SurveyActive { get; set; }
    public bool? SurveyIsStopped { get; set; }
    public int? SurveyViews { get; set; }
    public int? SurveySubmittedResponses { get; set; }
    public string SurveyPreviewCode { get; set; }
    public string SurveyReportCode { get; set; }
    public string SurveyUrlSlug { get; set; }
    public bool? SurveyIsTemplate { get; set; }
    public int? SurveyQuestionCount { get; set; }
    public Theme SurveyTheme { get; set; }
    public Subdomain SurveySubdomain { get; set; }

    public List<Question> SurveyQuestions { get; set; }

    public Settings SurveySettings { get; set; }
    public List<Variable> SurveyVariables { get; set; }
    public List<Welcome> SurveyWelcome { get; set; }
    public List<Appreciation> SurveyAppreciations { get; set; }

    public bool HasSurvey => SurveyId.HasValue;
    public string DisplayName => HasSurvey ? $"{FolderName} - {SurveyName}" : FolderName;
    public int TotalQuestions => SurveyQuestions?.Count ?? 0;
    public int TotalChoices => SurveyQuestions?.Sum(q => q.Choices?.Count ?? 0) ?? 0;
    public bool HasQuestions => SurveyQuestions?.Any() == true;
}
public class SurveyApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public SurveyApiService()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", "API-Key 63421de240a02920790af4eca3c53cd38a8ebe5c");
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
    public async Task<List<Folder>> GetFoldersWithSurveysAsync(string apiUrl)
    {
        try
        {
            var response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync();

            if (jsonContent.TrimStart().StartsWith("{"))
            {
                var singleFolder = JsonSerializer.Deserialize<Folder>(jsonContent, _jsonOptions);
                return singleFolder != null ? new List<Folder> { singleFolder } : new List<Folder>();
            }

            var folders = JsonSerializer.Deserialize<List<Folder>>(jsonContent, _jsonOptions);
            return folders ?? new List<Folder>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"err: {ex.Message}");
            return new List<Folder>();
        }
    }

    public List<FolderSurveyItem> ConvertToFlatList(List<Folder> folders)
    {
        var flatList = new List<FolderSurveyItem>();

        foreach (var folder in folders)
        {
            if (folder.Surveys != null && folder.Surveys.Any())
            {
                foreach (var survey in folder.Surveys)
                {
                    flatList.Add(new FolderSurveyItem
                    {
                        FolderId = folder.Id,
                        FolderName = folder.Name,
                        FolderOrder = folder.Order,
                        FolderSharedBy = folder.SharedBy,
                        FolderSharedWith = folder.SharedWith,

                        SurveyId = survey.Id,
                        SurveyName = survey.Name,
                        SurveyLanguage = survey.Language==0?"fa":"en",
                        SurveyCreatedDate = survey.CreatedDate,
                        SurveyActive = survey.Active,
                        SurveyIsStopped = survey.IsStopped,
                        SurveyViews = survey.Views,
                        SurveySubmittedResponses = survey.SubmittedResponses,
                        SurveyPreviewCode = survey.PreviewCode,
                        SurveyReportCode = survey.ReportCode,
                        SurveyUrlSlug = survey.UrlSlug,
                        SurveyIsTemplate = survey.IsTemplate,
                        SurveyQuestionCount = survey.QuestionCount,
                        SurveyTheme = survey.Theme,
                        SurveySubdomain = survey.Subdomain,

                        SurveyQuestions = survey.Questions,
                        SurveySettings = survey.Settings,
                        SurveyVariables = survey.Variables,
                        SurveyWelcome = survey.Welcome,
                        SurveyAppreciations = survey.Appreciations
                    });
                }
            }
            else
            {
                flatList.Add(new FolderSurveyItem
                {
                    FolderId = folder.Id,
                    FolderName = folder.Name,
                    FolderOrder = folder.Order,
                    FolderSharedBy = folder.SharedBy,
                    FolderSharedWith = folder.SharedWith
                });
            }
        }

        return flatList;
    }
    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
public class Program
{
    public static async Task Main(string[] args)
    {
        var surveyService = new SurveyApiService();
        var folders = await surveyService.GetFoldersWithSurveysAsync("https://survey.porsline.ir/api/folders/");                
        Console.WriteLine($" folder Count: {folders.Count}");
        var flatList = surveyService.ConvertToFlatList(folders);

        Console.WriteLine($"Survey count in folder ): {flatList.Count}");

        var jsonOutput = JsonSerializer.Serialize(flatList, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
        await File.WriteAllTextAsync("flat_output.json", jsonOutput);
     
        surveyService.Dispose();
    }
}