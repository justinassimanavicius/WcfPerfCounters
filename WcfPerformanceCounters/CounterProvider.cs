using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace WcfPerformanceCounters
{
	public class CounterProvider
	{
	    private const string CounterName_Executing = "Executing";
	    private const string CounterName_Hits = "Hits";
	    private const string CounterName_TimeTaken = "TimeTaken";
	    private const string CounterName_HitsPerSecond = "Hits Per Second";
	    private const string CounterName_AverageDuration = "Average duration";
	    private const string CounterName_AverageDurationBase = "Average duration base";
	    private const string CategoryName = "ServiceModelTimeTaken";
	    private static readonly ReaderWriterLockSlim RowLock = new ReaderWriterLockSlim();

		private static readonly Dictionary<string, CounterGroup> Counters =
			new Dictionary<string, CounterGroup>();

	    public static void CreateCategory()
	    {
	        if (PerformanceCounterCategory.Exists(CategoryName))
	        {
	            return;
	        }

	        var ccdc = new CounterCreationDataCollection();

	        var ccd = new CounterCreationData()
	        {
	            CounterName = CounterName_Executing,
	            CounterHelp = "Indicates number of calls executing at the given moment.",
	            CounterType = PerformanceCounterType.NumberOfItems32
	        };
	        ccdc.Add(ccd);

	        ccd = new CounterCreationData()
	        {
	            CounterName = CounterName_Hits,
	            CounterHelp = "Indicates number of requests since application startup.",
	            CounterType = PerformanceCounterType.NumberOfItems32
	        };
	        ccdc.Add(ccd);

	        ccd = new CounterCreationData()
	        {
	            CounterName = CounterName_TimeTaken,
	            CounterHelp = "Indicates the time it took for last call to complete in milliseconds.",
	            CounterType = PerformanceCounterType.NumberOfItems32
	        };
	        ccdc.Add(ccd);

	        ccd = new CounterCreationData()
	        {
	            CounterName = CounterName_HitsPerSecond,
	            CounterHelp = "Indicates number of calls initiated per second.",
	            CounterType = PerformanceCounterType.RateOfCountsPerSecond32
	        };
	        ccdc.Add(ccd);

	        ccd = new CounterCreationData()
	        {
	            CounterName = CounterName_AverageDuration,
	            CounterHelp = "Indicates average duration of a call to complete.",
	            CounterType = PerformanceCounterType.AverageTimer32
	        };
	        ccdc.Add(ccd);

	        var avgOpTimeBaseCounter = new CounterCreationData
	        {
	            CounterName = CounterName_AverageDurationBase,
	            CounterHelp = "",
	            CounterType = PerformanceCounterType.AverageBase
	        };
	        ccdc.Add(avgOpTimeBaseCounter);

	        PerformanceCounterCategory.Create(CategoryName,
	            "Custom performance counters to capture WCF client performance.",
	            PerformanceCounterCategoryType.MultiInstance, ccdc);
	    }

	    public static void DeleteCategory()
		{
			//
			//  check if the category exists
			//
			if (PerformanceCounterCategory.Exists(CategoryName))
			{
				PerformanceCounterCategory.Delete(CategoryName);
			}
		}

	    public static CounterGroup GetCounter(string action)
	    {
	        //
	        //  remove the response text from the action
	        //
	        if (string.IsNullOrWhiteSpace(action))
	        {
	            action = "unknown";
	        }
	        if (action.EndsWith("Response", StringComparison.OrdinalIgnoreCase))
	        {
	            action = action.Remove(action.Length - 8);
	        }
	        if (action.Length > 200)
	        {
	            action = action.Remove(200);
	        }
	        action = action.Replace('/', '-');
	        CounterGroup counterGroup = null;

	        //
	        //  check if we have our perfmon counter 
	        //      for this action in our dictionary object
	        //  

	        counterGroup = GetExistingInstance(action);

	        if (counterGroup != null)
	        {
	            return counterGroup;
	        }

	        return CreateInstance(action);
	    }

	    private static CounterGroup GetExistingInstance(string action)
	    {
            try
            {
                RowLock.EnterReadLock();
                CounterGroup counterGroup;
                if (Counters.TryGetValue(action, out counterGroup))
                {
                    return counterGroup;
                }
            }
            finally
            {
                RowLock.ExitReadLock();
            }
	        return null;
	    }

	    private static CounterGroup CreateInstance(string action)
	    {
	        CounterGroup counterGroup;
	        RowLock.EnterWriteLock();
	        try
	        {
	            if (Counters.TryGetValue(action, out counterGroup))
	            {
	                return counterGroup;
	            }

	            counterGroup = CreateGroupPerformanceCounter(action);
	            Counters.Add(action, counterGroup);
	        }
	        finally
	        {
	            RowLock.ExitWriteLock();
	        }

	        return counterGroup;
	    }

	    private static CounterGroup CreateGroupPerformanceCounter(string action)
		{

			if (!PerformanceCounterCategory.Exists(CategoryName))
			{
				return null;
			}

			var result = new CounterGroup
			{
				Executing = new PerformanceCounter(CategoryName, CounterName_Executing, action, false),
				Hits = new PerformanceCounter(CategoryName, CounterName_Hits, action, false),
				TimeTaken = new PerformanceCounter(CategoryName, CounterName_TimeTaken, action, false),
				AverageDuration = new PerformanceCounter(CategoryName, CounterName_AverageDuration, action, false),
				AverageDurationBase = new PerformanceCounter(CategoryName, CounterName_AverageDurationBase, action, false),
				HitsPerSecond = new PerformanceCounter(CategoryName, CounterName_HitsPerSecond, action, false)
			};

			result.Executing.RawValue = 0;
			result.Hits.RawValue = 0;
			result.TimeTaken.RawValue = 0;
			return result;

		}
	}
}