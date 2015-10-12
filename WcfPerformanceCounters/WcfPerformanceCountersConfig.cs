namespace WcfPerformanceCounters
{
	internal class WcfPerformanceCountersConfig
	{
		public bool bHangDump;
		public long captureDumpAfterSeconds;
		public string dumpCmd;
		public int dumpLimit;
		public int pollIntervalSeconds; // in milliseconds
	}
}