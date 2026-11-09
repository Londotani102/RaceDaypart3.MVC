namespace RaceDay.MVC.Models
{
    public class ResultViewModel
    {
        public int ResultId { get; set; }
        public int EventEnrollmentId { get; set; }
        public TimeSpan FinishTime { get; set; }
        public int Position { get; set; }
        public DateTime RecordedAt { get; set; }

        public ResultEnrolmentViewModel? EventEnrolment { get; set; }
    }

    public class ResultEnrolmentViewModel
    {
        public int EventEnrolmentId { get; set; }
        public int EventEnrollmentId { get; set; }
        public int ParticipantId { get; set; }
        public int EventId { get; set; }
    }
}