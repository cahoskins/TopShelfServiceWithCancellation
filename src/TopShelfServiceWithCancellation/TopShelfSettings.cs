namespace TopShelfServiceWithCancellation
{
    public class TopShelfSettings
    {
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public string ServiceName { get; set; }
        public int TimerExpiresEverySeconds { get; set; }
        public int MaxLoops { get; set; }
    }
}
