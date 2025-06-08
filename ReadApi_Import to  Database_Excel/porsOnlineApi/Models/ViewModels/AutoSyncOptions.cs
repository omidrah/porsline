namespace porsOnlineApi.Models.ViewModels
{
    public class AutoSyncOptions
    {
        public bool Enabled { get; set; } = true;
        public int IntervalMinutes { get; set; } = 60;
        public bool SyncOnStartup { get; set; } = true;
        public bool IncludeDetailedSurveys { get; set; } = true;
        public bool ExportToExcel { get; set; } = true;
        public string ExcelOutputPath { get; set; } = "exports";
        public bool CleanupOldFiles { get; set; } = true;
        public int KeepFilesForDays { get; set; } = 7;
        public List<TimeOnly> ScheduledTimes { get; set; } = new();
    }
}
