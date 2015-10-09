using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text.RegularExpressions;

namespace ServiceModelTimeTaken
{
	internal class ServiceModelTimeTakenMessageInspector : IClientMessageInspector, IDispatchMessageInspector
	{
		private static readonly ServiceModelTimeTakenThreadPool _customThreadPool = new ServiceModelTimeTakenThreadPool(1);

		private static readonly Regex rgx = new Regex("[^a-zA-Z0-9 -]", RegexOptions.Compiled);

		public ServiceModelTimeTakenMessageInspector(ServiceModelTimeTakenConfig config)
		{
			if (config.bHangDump)
			{
				_customThreadPool.StartHangDumpThread(config);
			}
		}


		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			long ticks = DateTime.Now.Ticks;


			string action = request.Headers.Action ?? "Unknown";

			action = rgx.Replace(action, "");
			_customThreadPool.QueuePerfmonWorkItem(CustomPerfmonCounterType.EXECUTING, action, 0, ticks, request.ToString());

			ServiceCallInfo callInfo;
			callInfo.Action = action;
			callInfo.Ticks = ticks;

			return callInfo;
		}

		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
			var callInfo = (ServiceCallInfo) correlationState;

			var elapsedSpan = new TimeSpan(DateTime.Now.Ticks - callInfo.Ticks);


			_customThreadPool.QueuePerfmonWorkItem(CustomPerfmonCounterType.TIMETAKEN, callInfo.Action,
				(int) elapsedSpan.TotalMilliseconds, callInfo.Ticks, string.Empty);
		}

		public object AfterReceiveRequest(ref Message request,
			IClientChannel channel,
			InstanceContext instanceContext)
		{
			long ticks = DateTime.Now.Ticks;


			_customThreadPool.QueuePerfmonWorkItem(CustomPerfmonCounterType.EXECUTING, request.Headers.Action, 0, ticks,
				request.ToString());
			return ticks;
		}

		public void BeforeSendReply(ref Message reply,
			object correlationState)
		{
			var startime = (long) correlationState;

			var elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startime);

			_customThreadPool.QueuePerfmonWorkItem(CustomPerfmonCounterType.TIMETAKEN, reply.Headers.Action,
				(int) elapsedSpan.TotalMilliseconds, startime, string.Empty);
		}


		private struct ServiceCallInfo
		{
			public string Action;
			public long Ticks;
		}
	}
}