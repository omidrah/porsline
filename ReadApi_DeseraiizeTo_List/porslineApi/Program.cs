using System.Text.Json;
public class Program
{
    public static async Task Main(string[] args)
    {
        var surveyService = new SurveyApiService();
        //var folders = await surveyService.GetFoldersWithSurveysAsync("https://survey.porsline.ir/api/folders/");                
        //Console.WriteLine($" folder Count: {folders.Count}");
        //var flatList = surveyService.ConvertToFlatList(folders);
        //Console.WriteLine($"Survey count in folder ): {flatList.Count}");


        // خواندن پاسخ‌های پرسشنامه
        // surveyId = 108037
        var responseData = await surveyService.GetSurveyResponsesAsync($"https://survey.porsline.ir/api/v2/surveys/1080037/responses/results-table/");

        Console.WriteLine($"ًQuestion Count: {responseData.Header?.Count ?? 0}");
        Console.WriteLine($"Responser Count: {responseData.RespondersCount}");

        var respondents = surveyService.ConvertToRespondentDetails(responseData);

        var jsonOutput = JsonSerializer.Serialize(respondents, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
        await File.WriteAllTextAsync("flat_output.json", jsonOutput);
     
        surveyService.Dispose();
    }
}