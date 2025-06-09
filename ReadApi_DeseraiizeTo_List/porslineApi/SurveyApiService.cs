using porslineApi.JsonModel;
using porsOnlineApi.JsonModel;
using System.Text.Json;

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
                        SurveyLanguage = survey.Language == 0 ? "fa" : "en",
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

    public async Task<SurveyResponseData> GetSurveyResponsesAsync(string apiUrl)
    {
        try
        {
            var response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync();
            var surveyResponse = JsonSerializer.Deserialize<SurveyResponseData>(jsonContent, _jsonOptions);

            return surveyResponse ?? new SurveyResponseData();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"خطا در دریافت پاسخ‌ها: {ex.Message}");
            return new SurveyResponseData();
        }
    }
    // تبدیل داده‌های خام به مدل جزئیات پاسخ‌دهندگان
    public List<RespondentDetail> ConvertToRespondentDetails(SurveyResponseData responseData)
    {
        if (responseData?.Header == null || responseData?.Body == null)
            return new List<RespondentDetail>();

        var respondentDetails = new List<RespondentDetail>();

        foreach (var responseBody in responseData.Body)
        {
            var respondent = new RespondentDetail
            {
                ResponderId = responseBody.ResponderId,
                ResponderCode = responseBody.ResponderCode,
                Answers = new List<QuestionAnswer>()
            };

            // پردازش پاسخ‌ها بر اساس header
            for (int i = 0; i < responseData.Header.Count && i < responseBody.Data.Count; i++)
            {
                var header = responseData.Header[i];
                var answerValue = responseBody.Data[i];

                ProcessAnswer(respondent, header, answerValue, i);
            }

            respondentDetails.Add(respondent);
        }

        return respondentDetails;
    }

    private void ProcessAnswer(RespondentDetail respondent, ResponseHeader header, object answerValue, int index)
    {
        var answer = new QuestionAnswer
        {
            QuestionId = header.Id,
            QuestionTitle = header.Title,
            QuestionType = header.CellType,
            Answer = answerValue,
            IsAnswered = answerValue != null,
            AnswerText = answerValue?.ToString()
        };

        // پردازش بر اساس نوع سوال/ستون
        switch (header.Title.ToLower())
        {
            case "review_token":
                respondent.ReviewToken = answerValue?.ToString();
                break;

            case "responder_code":
                // قبلاً تنظیم شده
                break;

            case "started_date_time":
                if (DateTime.TryParseExact(answerValue?.ToString(), "yyyy/MM/dd-HH:mm:ss", null,
                    System.Globalization.DateTimeStyles.None, out DateTime started))
                {
                    respondent.StartedDateTime = started;
                }
                break;

            case "submitted_date_time":
                if (DateTime.TryParseExact(answerValue?.ToString(), "yyyy/MM/dd-HH:mm:ss", null,
                    System.Globalization.DateTimeStyles.None, out DateTime submitted))
                {
                    respondent.SubmittedDateTime = submitted;
                }
                break;

            default:
                // سوالات عادی بر اساس محتوا
                ProcessSpecificAnswer(respondent, header, answerValue, answer);
                break;
        }

        if (!string.IsNullOrEmpty(answer.QuestionTitle) &&
            !answer.QuestionTitle.Contains("_date_time") &&
            !answer.QuestionTitle.Contains("review_token") &&
            !answer.QuestionTitle.Contains("responder_code"))
        {
            respondent.Answers.Add(answer);
        }
    }

    private void ProcessSpecificAnswer(RespondentDetail respondent, ResponseHeader header, object answerValue, QuestionAnswer answer)
    {
        var title = header.Title;
        var answerText = answerValue?.ToString();

        if (title.Contains("شماره تماس"))
        {
            respondent.PhoneNumber = answerText;
        }
        else if (title.Contains("نام و نام‌خانوادگی") || title.Contains("نام‌خانوادگی"))
        {
            respondent.FullName = answerText;
        }
        else if (title.Contains("جنسیت"))
        {
            respondent.Gender = answerText;
        }
        else if (title.Contains("استان"))
        {
            respondent.Province = answerText;
        }
        else if (title.Contains("امتیازی می‌دین") && title.Contains("وایزر"))
        {
            if (int.TryParse(answerText, out int rating))
                respondent.WiserRating = rating;
        }
        else if (title.Contains("توصیه کنین"))
        {
            if (int.TryParse(answerText, out int npsScore))
                respondent.RecommendationScore = npsScore;
        }
        else if (title.Contains("فروشنده") && title.Contains("امتیاز"))
        {
            if (int.TryParse(answerText, out int salesRating))
                respondent.SalesPersonRating = salesRating;
        }
        if (header.AllowMultipleSelect == true && !string.IsNullOrEmpty(answerText))
        {
            answer.SelectedChoices = answerText.Split(',').Select(x => x.Trim()).ToList();
        }

        if (header.SubQuestions?.Any() == true && header.Type == 8)
        {
            answer.SubAnswers = new Dictionary<string, string>();
        }
    }
    public SurveyStatistics GetStatistics(List<RespondentDetail> respondents)
    {
        if (respondents == null || !respondents.Any())
        {
            return new SurveyStatistics(); // آمار خالی برگردان
        }

        // فیلتر کردن داده‌ها برای محاسبات
        var wiserRatings = respondents.Where(r => r.WiserRating.HasValue).Select(r => r.WiserRating.Value);
        var npsScores = respondents.Where(r => r.RecommendationScore.HasValue).Select(r => r.RecommendationScore.Value);
        var completionTimes = respondents.Where(r => r.CompletionTime.HasValue).Select(r => r.CompletionTime.Value);

        return new SurveyStatistics
        {
            TotalRespondents = respondents.Count,
            CompletedResponses = respondents.Count(r => r.IsComplete),
            AverageWiserRating = wiserRatings.Any() ? wiserRatings.Average() : 0,
            AverageNPSScore = npsScores.Any() ? npsScores.Average() : 0,
            PromotersCount = respondents.Count(r => r.NPSCategory == "طرفدار"),
            DetractorsCount = respondents.Count(r => r.NPSCategory == "منتقد"),
            PassiveCount = respondents.Count(r => r.NPSCategory == "بی‌تفاوت"),
            AverageCompletionTime = completionTimes.Any() ?
                TimeSpan.FromTicks((long)completionTimes.Average(t => t.Ticks)) :
                TimeSpan.Zero
        };
    }
    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
public class SurveyStatistics
{
    public int TotalRespondents { get; set; }
    public int CompletedResponses { get; set; }
    public double AverageWiserRating { get; set; }
    public double AverageNPSScore { get; set; }
    public int PromotersCount { get; set; }
    public int DetractorsCount { get; set; }
    public int PassiveCount { get; set; }
    public TimeSpan AverageCompletionTime { get; set; }

    // محاسبه NPS Score با چک کردن تقسیم بر صفر
    public double NPSScore => TotalRespondents > 0 ?
        ((double)(PromotersCount - DetractorsCount) / TotalRespondents) * 100 : 0;

    // Properties کمکی
    public double CompletionRate => TotalRespondents > 0 ?
        ((double)CompletedResponses / TotalRespondents) * 100 : 0;

    public string CompletionRateText => $"{CompletionRate:F1}%";
    public string NPSScoreText => $"{NPSScore:F1}";

    // متد نمایش خلاصه
    public string GetSummary()
    {
                        return $@"آمار کلی پرسشنامه:
                • Total Respons‌e: {TotalRespondents:N0}
                • Total Compelete Response: {CompletedResponses:N0} ({CompletionRateText})
                • Avg. Wizer Score: {AverageWiserRating:F1}/10
                • Avg. Recommendation Score: {AverageNPSScore:F1}/10
                • Scoring NPS: {NPSScoreText}
                • Fans: {PromotersCount:N0} | بی‌تفاوت: {PassiveCount:N0} | منتقدان: {DetractorsCount:N0}
                • Avg. Completation Time: {AverageCompletionTime} دقیقه";
    }
}