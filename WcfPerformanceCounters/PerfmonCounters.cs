using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace WcfPerformanceCounters
{
	public class PerfmonCounters
	{
		private static readonly ReaderWriterLockSlim _rwlock = new ReaderWriterLockSlim();

		private static readonly Dictionary<string, GroupPerformanceCounter> _dictionaryPerfCounters =
			new Dictionary<string, GroupPerformanceCounter>();

		public static void CreateCategory()
		{
			//
			//  check if the category exists
			//
			if (!PerformanceCounterCategory.Exists("ServiceModelTimeTaken"))
			{
				var ccdc = new CounterCreationDataCollection();
				var ccd = new CounterCreationData("Executing", "", PerformanceCounterType.NumberOfItems32);
				ccdc.Add(ccd);
				ccd = new CounterCreationData("Hits", "", PerformanceCounterType.NumberOfItems32);
				ccdc.Add(ccd);
				ccd = new CounterCreationData("TimeTaken", "", PerformanceCounterType.NumberOfItems32);
				ccdc.Add(ccd);

				PerformanceCounterCategory.Create("ServiceModelTimeTaken",
					"Custom performance counter to capture WCF operation timetaken", PerformanceCounterCategoryType.MultiInstance, ccdc);
			}
		}

		public static void DeleteCategory()
		{
			//
			//  check if the category exists
			//
			if (PerformanceCounterCategory.Exists("ServiceModelTimeTaken"))
			{
				PerformanceCounterCategory.Delete("ServiceModelTimeTaken");
			}
		}

		public static GroupPerformanceCounter GetCounter(string action)
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
			GroupPerformanceCounter group = null;

			//
			//  check if we have our perfmon counter 
			//      for this action in our dictionary object
			//  
			
			try
			{
				_rwlock.EnterReadLock();
				if (_dictionaryPerfCounters.TryGetValue(action, out group))
				{
					return group;
				}
			}
			finally
			{
				_rwlock.ExitReadLock();
			}

			//
			//  get the writer lock
			//
			_rwlock.EnterWriteLock();
			try
			{
				//
				//  double check
				//
				if (_dictionaryPerfCounters.TryGetValue(action, out group))
				{
					return group;
				}
				//
				//  check if the category exists
				//
				group = CreateGroupPerformanceCounter(action);
					_dictionaryPerfCounters.Add(action, group);
				
			}
			finally
			{
				_rwlock.ExitWriteLock();
			}

			return group;
		}

		private static GroupPerformanceCounter CreateGroupPerformanceCounter(string action)
		{

			if (!PerformanceCounterCategory.Exists("ServiceModelTimeTaken"))
			{
				return null;
			}

			//
			//  create instance
			//
			var result = new GroupPerformanceCounter
			{
				Executing = new PerformanceCounter("ServiceModelTimeTaken", "Executing", action, false),
				Hits = new PerformanceCounter("ServiceModelTimeTaken", "Hits", action, false),
				TimeTaken = new PerformanceCounter("ServiceModelTimeTaken", "TimeTaken", action, false)
			};

			result.Executing.RawValue = 0;
			result.Hits.RawValue = 0;
			result.TimeTaken.RawValue = 0;
			return result;

		}
	}
}