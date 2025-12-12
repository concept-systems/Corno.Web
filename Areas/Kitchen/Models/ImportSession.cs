using System;
using System.Collections.Generic;

namespace Corno.Web.Areas.Kitchen.Models
{
    public class ImportSession
    {
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public string FileName { get; set; }
        public DateTime StartTime { get; set; }
        public ImportStatus Status { get; set; }
        public int TotalRecords { get; set; }
        public int ProcessedRecords { get; set; }
        public int ImportedCount { get; set; }
        public int UpdatedCount { get; set; }
        public int ErrorCount { get; set; }
        public int SkippedCount { get; set; }
        public string CurrentMessage { get; set; }
        public double PercentComplete { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime? EndTime { get; set; }
        public List<string> ErrorMessages { get; set; }
        public ImportSummary Summary { get; set; }
        public string CurrentStep { get; set; }
        public TimeSpan? EstimatedTimeRemaining { get; set; }
        public object ImportResults { get; set; }
        
        // Detailed progress information
        public Dictionary<string, object> ProgressDetails { get; set; }
        public List<string> ProcessingSteps { get; set; }
        public int CurrentWarehouseOrderIndex { get; set; }
        public int TotalWarehouseOrders { get; set; }
        public string CurrentWarehouseOrderNo { get; set; }
        public int RecordsInCurrentBatch { get; set; }
        public int TotalBatches { get; set; }
        public int CurrentBatch { get; set; }

        public ImportSession()
        {
            ErrorMessages = new List<string>();
            ProgressDetails = new Dictionary<string, object>();
            ProcessingSteps = new List<string>();
            Status = ImportStatus.Pending;
            StartTime = DateTime.Now;
        }
    }

    public enum ImportStatus
    {
        Pending = 0,
        Reading = 1,
        Validating = 2,
        Processing = 3,
        Completed = 4,
        Cancelled = 5,
        Failed = 6
    }

    public class ImportSummary
    {
        public int TotalRecords { get; set; }
        public int SuccessfullyImported { get; set; }
        public int Updated { get; set; }
        public int Errors { get; set; }
        public int Skipped { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public List<ImportErrorDetail> ErrorDetails { get; set; }
        public Dictionary<string, int> StatisticsByWarehouseOrder { get; set; }
        public double RecordsPerSecond { get; set; }
        
        // Detailed summary information
        public Dictionary<string, object> SummaryDetails { get; set; }
        public List<string> ProcessingSteps { get; set; }
        public Dictionary<string, TimeSpan> StepDurations { get; set; }
        public int TotalBatches { get; set; }
        public int TotalWarehouseOrders { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long FileSizeBytes { get; set; }

        public ImportSummary()
        {
            ErrorDetails = new List<ImportErrorDetail>();
            StatisticsByWarehouseOrder = new Dictionary<string, int>();
            SummaryDetails = new Dictionary<string, object>();
            ProcessingSteps = new List<string>();
            StepDurations = new Dictionary<string, TimeSpan>();
        }
    }

    public class ImportErrorDetail
    {
        public int RowNumber { get; set; }
        public string WarehouseOrderNo { get; set; }
        public string Field { get; set; }
        public string ErrorMessage { get; set; }
        public string Value { get; set; }
    }
}

