namespace WcfPerformanceCounters
{
    internal class WorkItem
    {
        public string Action;
        public OperationState CounterType;
        public long DurationInTicks;
        public long DurationInMs { get; set; }
    }
}