using System.Diagnostics;

namespace WcfPerformanceCounters
{
	public class CounterGroup
	{
		public PerformanceCounter Executing;
		public PerformanceCounter Hits;
		public PerformanceCounter TimeTaken;
	    public PerformanceCounter AverageDuration { get; set; }
	    public PerformanceCounter AverageDurationBase { get; set; }
	    public PerformanceCounter HitsPerSecond { get; set; }
	}
}