namespace porsOnlineApi.Models.ViewModels
{
    public class SyncResult
    {
        public bool Success { get; set; }
        public int FoldersProcessed { get; set; }
        public int SurveysProcessed { get; set; }
        public int DetailedSurveysProcessed { get; set; }
        public int TotalRecordsSaved { get; set; }
        public List<string> Errors { get; set; } = new();
        public TimeSpan Duration { get; set; }
        public DateTime SyncStartTime { get; set; }
        public DateTime SyncEndTime { get; set; }
        public string ExcelPath { get; set; } = string.Empty;
    }
}
