using porsOnlineApi.JsonModel;

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
