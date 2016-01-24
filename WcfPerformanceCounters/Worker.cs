using System;
using System.Collections.Generic;
using System.Threading;

namespace WcfPerformanceCounters
{
	
	internal class Worker
	{
		
		private readonly Queue<WorkItem> _queue;
	    private readonly Semaphore _workWaiting;
        
		public Worker(int numThreads = 1)
		{
		    if (numThreads <= 0)
				throw new ArgumentOutOfRangeException(nameof(numThreads));

			var threads = new List<Thread>(numThreads);
			_queue = new Queue<WorkItem>();
			_workWaiting = new Semaphore(0, int.MaxValue);
            
			for (var i = 0; i < numThreads; i++)
			{
			    var t = new Thread(Run) {IsBackground = true};
			    threads.Add(t);
				t.Start();
			}
		}
        
		public void QueueWorkItem(WorkItem item)
		{
		    lock (_queue) _queue.Enqueue(item);
			_workWaiting.Release();
		}

		private void Run()
		{
			try
			{
				while (true)
				{
					ProcessItem();
				}
			}
			catch
			{
			}
		}

	    private void ProcessItem()
	    {
	        var item = GetNextWorkItem();

	        var counter = CounterProvider.GetCounter(item.Action);

	        if (counter == null) return;

	        switch (item.CounterType)
	        {
	            case OperationState.BeforeCall:
	            {
	                counter.Executing.Increment();
	                counter.HitsPerSecond.Increment();
	                break;
	            }
	            case OperationState.AfterCall:
	            {
	                counter.Executing.Decrement();
	                counter.Hits.Increment();
	                counter.AverageDuration.IncrementBy(item.DurationInTicks);
	                counter.AverageDurationBase.Increment();
	                counter.TimeTaken.RawValue = item.DurationInMs;

	                break;
	            }
	        }
	    }

	    private WorkItem GetNextWorkItem()
	    {
	        while (true)
	        {
	            lock (_queue)
	            {
	                if (_queue.Count > 0)
	                {
	                    return _queue.Dequeue();
	                }
	            }
	            _workWaiting.WaitOne();
	        }
	    }

	   
	}
}