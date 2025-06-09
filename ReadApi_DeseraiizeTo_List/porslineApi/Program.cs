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

        var stats = surveyService.GetStatistics(respondents);
        //Console.WriteLine($"\n--- آمار کلی ---");
        //Console.WriteLine(stats.GetSummary());

        var jsonOutput1 = JsonSerializer.Serialize(stats.GetSummary(), new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
        await File.WriteAllTextAsync("Summary_output.json", jsonOutput1);
       

        //if (stats.TotalRespondents > 0)
        //{
        //    Console.WriteLine($"\n--- آمار تفصیلی ---");
        //    var genderStats = respondents.Where(r => !string.IsNullOrEmpty(r.Gender))
        //                                .GroupBy(r => r.Gender)
        //                                .Select(g => new { Gender = g.Key, Count = g.Count() });

        //    Console.WriteLine("توزیع جنسیت:");
        //    foreach (var item in genderStats)
        //    {
        //        Console.WriteLine($"  {item.Gender}: {item.Count} نفر");
        //    }          
        //    var provinceStats = respondents.Where(r => !string.IsNullOrEmpty(r.Province))
        //                                  .GroupBy(r => r.Province)
        //                                  .OrderByDescending(g => g.Count())
        //                                  .Take(5);

        //    Console.WriteLine("\nتاپ 5 استان:");
        //    foreach (var item in provinceStats)
        //    {
        //        Console.WriteLine($"  {item.Key}: {item.Count()} نفر");
        //    }
        //  }
       

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