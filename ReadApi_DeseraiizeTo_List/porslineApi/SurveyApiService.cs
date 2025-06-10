using porslineApi.JsonModel;
using porsOnlineApi.JsonModel;
using System.Linq;
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
            Console.WriteLine($"err : {ex.Message}");
            return new SurveyResponseData();
        }
    }
    public List<RespondentDetail> ConvertToRespondentDetails(SurveyResponseData responseData)
    {
        if (responseData?.Header == null || responseData?.Body == null)
            return new List<RespondentDetail>();

        var respondentDetails = new List<RespondentDetail>();

        foreach (var responseBody in responseData.Body)
        {
            var respondent = new RespondentDetail
            {               
                Answers = new List<QuestionAnswer>()
            };
            for (int i = 0; i < responseData.Header.Count && i < responseBody.Data.Count; i++)
            {
                var header = responseData.Header[i];
                ProcessAnswer(respondent, header, responseBody);
            }

            respondentDetails.Add(respondent);
        }

        return respondentDetails;
    }

    private void ProcessAnswer(RespondentDetail respondent, ResponseHeader header, ResponseBody answerValue)
    {
        //var answer = new QuestionAnswer
        //{
        //    QuestionId = header.Id,
        //    QuestionTitle = header.Title,
        //    QuestionType = header.CellType,
        //    SubAnswers = new Dictionary<string, string>(),
        //    Answer = answerValue,
        //    IsAnswered = answerValue != null
        //};
        ProcessSpecificAnswer(respondent, header, answerValue);


        //if (!string.IsNullOrEmpty(answer.QuestionTitle) &&
        //    !answer.QuestionTitle.Contains("_date_time") &&
        //    !answer.QuestionTitle.Contains("review_token") &&
        //    !answer.QuestionTitle.Contains("responder_code"))
        //{
           // respondent.Answers.Add(answer);
        //}
    }
    int bodyindex = 0;
    private void ProcessSpecificAnswer(RespondentDetail respondent, ResponseHeader header, ResponseBody answerValue)
    {
        var title = header.Title;
        if (header.Type == null)
        {
            bodyindex++;
            return;
        }
        switch (header.Type)
        {
            case 1:
                break;
            case 2: //string 
                if (title.Contains("شماره تماس"))
                {
                    respondent.PhoneNumber = answerValue.Data[bodyindex].ToString();
                }
                else if (title.Contains("نام و نام‌خانوادگی") || title.Contains("نام‌خانوادگی"))
                {
                    respondent.FullName = answerValue.Data[bodyindex].ToString();
                }
                bodyindex++;
                break;
            case 3:
                if (title.Contains("جنسیت"))
                {
                    respondent.GenderId = header.Choices.First(x => x.Name == answerValue.Data[bodyindex].ToString()).Id;
                    respondent.Gender = answerValue.Data[bodyindex].ToString();
                    bodyindex++;
                    GenerateJson(header, "gender.json"); //get id of gender
                }
                else if (title.Contains("قبل از مراجعه به فروشگاه "))
                {
                    //id = 51972037 بله می شناختم
                    //id = 51972036 نه نمی شناختم 
                    respondent.PreviousAcquaintance = header.Choices.First(x => x.Name == answerValue.Data[bodyindex].ToString()).Id;
                    bodyindex++;
                    GenerateJson(header, "PreviousAcquaintance.json"); //Id of Acquaintance
                }
                else if (title.Contains("از چه طریقی با "))
                {
                    respondent.AcquaintanceMethod = header.Choices.First(x => x.Name == answerValue.Data[bodyindex].ToString()).Id;
                    bodyindex++;                    
                    GenerateJson(header, "AcquaintanceMethod.json"); //Id of methods
                }

                else if (title.Contains("نصاب") && !string.IsNullOrEmpty(answerValue.Data[bodyindex].ToString()))
                {
                    respondent.InstallerScore = header.Choices.First(x => x.Name == answerValue.Data[bodyindex].ToString()).Id;
                    bodyindex++;
                    GenerateJson(header, "InstallerBe.json"); //Id of behaviour of intstaller                    
                }

                else if (header.AllowMultipleSelect == true)
                {
                    var tmp = new List<string>();
                    for (int i = 0; i < header.Choices.Count; i++)
                    {
                        if ((answerValue.Data[bodyindex]) != null)
                            tmp.Add(answerValue.Data[bodyindex].ToString());
                        bodyindex++;
                    }
                    respondent.BestFeatures = new int[tmp.Count];
                    for (int i = 0; i < tmp.Count; i++)
                    {
                        respondent.BestFeatures[i] = header.Choices.First(x => x.Name == tmp[i]).Id;
                    }
                }
                break;
            case 4:
                break;
            case 5:
                break;
            case 6: //int
                if (title.Contains("امتیازی می‌دین") && title.Contains("وایزر"))
                {
                    if (int.TryParse(answerValue.Data[bodyindex].ToString(), out int rating))
                        respondent.WizerScore = rating;
                }
                else if (title.Contains("توصیه کنین"))
                {
                    if (int.TryParse(answerValue.Data[bodyindex].ToString(), out int npsScore))
                        respondent.RecommendationScore = npsScore;
                }
                else if (title.Contains("امتیازی می‌دین") && title.Contains("فروشنده") && !string.IsNullOrEmpty(answerValue.Data[bodyindex].ToString()))
                {
                    if (int.TryParse(answerValue.Data[bodyindex].ToString(), out int salesRating))
                        respondent.SellerScore = salesRating;
                }
                bodyindex++;

                break;
            case 7:
                break;
            case 8: //Sub Question. 
                if (header.SubQuestions?.Any() == true)
                {
                    for (int i = 0; i < header.SubQuestions.Count; i++)
                    {
                        var qes = header.SubQuestions[i].Title;
                        var ans = answerValue.Data[bodyindex].ToString();
                        //answer.SubAnswers.Add(qes, ans);
                        if (qes.Contains("منطقه") || qes.Contains("محله"))
                        {
                            respondent.Region = ans.ToString();
                        }
                        else if (qes.Contains("نام فروشگاه"))
                        {
                            respondent.StoreName = ans.ToString();
                        }
                        else if (qes.Contains("نام فروشنده"))
                        {
                            respondent.SellerName = ans.ToString();
                        }
                        else if (qes.Contains("نام نصاب"))
                        {
                            respondent.InstallerNam = ans.ToString();
                        }
                    }
                    bodyindex++;
                }

                break;
            case 9:
                break;
            case 10:  //one selection from dropdown sample Province
                if (title.Contains("استان"))
                {
                    respondent.ProvinceId = header.Choices.First(x => x.Name == answerValue.Data[bodyindex].ToString()).Id;
                    respondent.Province = answerValue.Data[bodyindex].ToString();
                    bodyindex++;
                    GenerateJson(header,"province.json"); //Id of province
                }
                break;
        }


    }

    public static async Task GenerateJson(ResponseHeader header,string jsonFilename)
    {
        var jsonOutput = JsonSerializer.Serialize(header.Choices, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
      await  File.WriteAllTextAsync(jsonFilename, jsonOutput);
    }
    public static async Task GenerateJson(List<RespondentDetail> respondents, string jsonFilename)
    {
        var jsonOutput = JsonSerializer.Serialize(respondents, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
        await File.WriteAllTextAsync(jsonFilename, jsonOutput);
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}