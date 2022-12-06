namespace Model
{
    public class PulseCheckResult
    {
        public string AccountName { get; set; }
        public int Frequency { get; set; }
        public Double DiffInMinutes { get; set; }
        public DateTime LastRegistered { get;set; }
    }
}
