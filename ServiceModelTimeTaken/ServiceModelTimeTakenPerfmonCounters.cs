using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace ServiceModelTimeTaken
{
    public class GroupPerformanceCounter
    {
        public PerformanceCounter Executing;
        public PerformanceCounter Hits;
        public PerformanceCounter TimeTaken;
    }

    public class ServiceModelTimeTakenPerfmonCounters
    {
        static ReaderWriterLockSlim _rwlock = new ReaderWriterLockSlim();
        static Dictionary<string, GroupPerformanceCounter> _dictionaryPerfCounters = new Dictionary<string, GroupPerformanceCounter>();

        static public void CreateCategory()
        {
            //
            //  check if the category exists
            //
            if (!PerformanceCounterCategory.Exists("ServiceModelTimeTaken"))
            {
                CounterCreationDataCollection ccdc = new CounterCreationDataCollection();
                CounterCreationData ccd = new CounterCreationData("Executing", "", PerformanceCounterType.NumberOfItems32);
                ccdc.Add(ccd);
                ccd = new CounterCreationData("Hits", "", PerformanceCounterType.NumberOfItems32);
                ccdc.Add(ccd);
                ccd = new CounterCreationData("TimeTaken", "", PerformanceCounterType.NumberOfItems32);
                ccdc.Add(ccd);

                PerformanceCounterCategory.Create("ServiceModelTimeTaken", "Custom performance counter to capture WCF operation timetaken", PerformanceCounterCategoryType.MultiInstance, ccdc);
            }
        }
        static public void DeleteCategory()
        {
            //
            //  check if the category exists
            //
            if (PerformanceCounterCategory.Exists("ServiceModelTimeTaken"))
            {
                PerformanceCounterCategory.Delete("ServiceModelTimeTaken");
            }
        }
        static public GroupPerformanceCounter GetCounter(string action)
        {
            //
            //  remove the response text from the action
            //
            if (action.EndsWith("Response", StringComparison.OrdinalIgnoreCase))
            {
                action = action.Remove(action.Length - 8);
            }
            if (action.Length > 200)
            {
                action = action.Remove(200);
            }
            action = action.Replace('/', '-');
            GroupPerformanceCounter ret = null;

            //
            //  check if we have our perfmon counter 
            //      for this action in our dictionary object
            //  
            _rwlock.EnterReadLock();
            if (_dictionaryPerfCounters.TryGetValue(action, out ret))
            {
                _rwlock.ExitReadLock();
                return ret;
            }
            _rwlock.ExitReadLock();

            //
            //  get the writer lock
            //
            _rwlock.EnterWriteLock();
            try
            {
                //
                //  double check
                //
                if (_dictionaryPerfCounters.TryGetValue(action, out ret))
                {
                    return ret;
                }
                //
                //  check if the category exists
                //
                if (PerformanceCounterCategory.Exists("ServiceModelTimeTaken"))
                {
                    //
                    //  create instance
                    //
                    ret = new GroupPerformanceCounter();
                    ret.Executing = new PerformanceCounter("ServiceModelTimeTaken", "Executing", action, false);
                    ret.Hits = new PerformanceCounter("ServiceModelTimeTaken", "Hits", action, false);
                    ret.TimeTaken = new PerformanceCounter("ServiceModelTimeTaken", "TimeTaken", action, false);

                    ret.Executing.RawValue = 0;
                    ret.Hits.RawValue = 0;
                    ret.TimeTaken.RawValue = 0;

                    _dictionaryPerfCounters.Add(action, ret);
                }
            }
            finally
            {
                _rwlock.ExitWriteLock();
            }

            return ret;
        }
    }
}
