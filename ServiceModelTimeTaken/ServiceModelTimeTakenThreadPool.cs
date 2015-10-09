using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ServiceModelTimeTaken
{
	internal enum CustomPerfmonCounterType
	{
		EXECUTING,
		TIMETAKEN
	}

	internal class ServiceModelTimeTakenThreadPool
	{
		private readonly Dictionary<long, string> _currentRunningRequests = new Dictionary<long, string>();

		private readonly Queue<PerfmonWorkItem> _queue;
		private readonly List<Thread> _threads;
		private readonly Semaphore _workWaiting;

		private int _dumpCount;
		private string _dumpIncomingMessage;
		private long _dumpstartTicks;
		private ManualResetEvent _pollingEvent;

		private ServiceModelTimeTakenConfig _serviceModelTimeTakenConfig = new ServiceModelTimeTakenConfig()
		{
			bHangDump = false
		};

		public ServiceModelTimeTakenThreadPool(int numThreads)
		{
			if (numThreads <= 0)
				throw new ArgumentOutOfRangeException("numThreads");

			_threads = new List<Thread>(numThreads);
			_queue = new Queue<PerfmonWorkItem>();
			_workWaiting = new Semaphore(0, int.MaxValue);

			_dumpCount = 0;

			for (int i = 0; i < numThreads; i++)
			{
				var t = new Thread(Run);
				t.IsBackground = true;
				_threads.Add(t);
				t.Start();
			}
		}

		public void StartHangDumpThread(ServiceModelTimeTakenConfig config)
		{
			_pollingEvent = new ManualResetEvent(false);
			_serviceModelTimeTakenConfig = config;

			var t = new Thread(HangDumpThread);
			t.IsBackground = true;
			_threads.Add(t);
			t.Start();
		}

		public void QueuePerfmonWorkItem(CustomPerfmonCounterType ctype, string action, int duration, long startTicks,
			string incomingMessage)
		{
			var item = new PerfmonWorkItem();
			item.ctype = ctype;
			item.action = action;
			item.duration = duration;
			item.startTicks = startTicks;
			item.incomingMessage = incomingMessage;

			lock (_queue) _queue.Enqueue(item);
			_workWaiting.Release();
		}

		private void Run()
		{
			try
			{
				while (true)
				{
					PerfmonWorkItem item = null;

					while (item == null)
					{
						lock (_queue)
						{
							if (_queue.Count > 0)
							{
								item = _queue.Dequeue();
								break;
							}
						}
						_workWaiting.WaitOne();
					}
					//
					//  get the performance counter
					//
					GroupPerformanceCounter gpc = ServiceModelTimeTakenPerfmonCounters.GetCounter(item.action);
					if (gpc != null)
					{
						//
						//  set the values
						//
						switch (item.ctype)
						{
							case CustomPerfmonCounterType.EXECUTING:
							{
								if (_serviceModelTimeTakenConfig.bHangDump)
								{
									lock (_currentRunningRequests)
									{
										try
										{
											//
											//  we may get two requests with same tick value
											//
											_currentRunningRequests.Add(item.startTicks, item.incomingMessage);
										}
										catch
										{
										}
									}
								}
								gpc.Executing.Increment();

								break;
							}
							case CustomPerfmonCounterType.TIMETAKEN:
							{
								if (_serviceModelTimeTakenConfig.bHangDump)
								{
									lock (_currentRunningRequests)
									{
										_currentRunningRequests.Remove(item.startTicks);
									}
								}
								gpc.Executing.Decrement();
								gpc.Hits.Increment();
								gpc.TimeTaken.RawValue = item.duration;

								break;
							}
						}
					}
				}
			}
			catch
			{
			}
		}


		private void HangDumpThread()
		{
			bool bCaptureDump = false;
			try
			{
				while (true)
				{
					_pollingEvent.WaitOne(_serviceModelTimeTakenConfig.pollIntervalSeconds*1000);

					long lastTicks = DateTime.Now.Ticks - (_serviceModelTimeTakenConfig.captureDumpAfterSeconds*10000000);

					lock (_currentRunningRequests)
					{
						foreach (long startTicks in _currentRunningRequests.Keys)
						{
							if (startTicks < lastTicks)
							{
								_dumpstartTicks = startTicks;
								_dumpIncomingMessage = _currentRunningRequests[startTicks];

								if (_dumpCount < _serviceModelTimeTakenConfig.dumpLimit)
								{
									_dumpCount++;
									//capture a dump
									bCaptureDump = true;
								}
								break;
							}
						}
					}

					if (bCaptureDump)
					{
						Process.Start(_serviceModelTimeTakenConfig.dumpCmd, Process.GetCurrentProcess().Id.ToString());

						bCaptureDump = false;

						//
						//  capture only one dump per request
						//      remove this request from our list of running requests
						//
						lock (_currentRunningRequests)
						{
							_currentRunningRequests.Remove(_dumpstartTicks);
						}
					}
				}
			}
			catch
			{
			}
		}

		private class PerfmonWorkItem
		{
			public string action;
			public CustomPerfmonCounterType ctype;
			public int duration;
			public string incomingMessage;
			public long startTicks;
		}
	}
}