using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.Text.RegularExpressions;

namespace ServiceModelTimeTaken
{
	class ServiceModelTimeTakenMessageInspector : IClientMessageInspector ,IDispatchMessageInspector
    {
        public ServiceModelTimeTakenMessageInspector(ServiceModelTimeTakenConfig config)
        {
            if (config.bHangDump)
            {
                _customThreadPool.StartHangDumpThread(config);
            }
        }

        static ServiceModelTimeTakenThreadPool _customThreadPool = new ServiceModelTimeTakenThreadPool(1);
        
		static Regex rgx = new Regex("[^a-zA-Z0-9 -]", RegexOptions.Compiled);

        
        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, 
                                                System.ServiceModel.IClientChannel channel, 
                                                System.ServiceModel.InstanceContext instanceContext)
        {
            long ticks =  DateTime.Now.Ticks;


            _customThreadPool.QueuePerfmonWorkItem(CustomPerfmonCounterType.EXECUTING, request.Headers.Action, 0, ticks, request.ToString());
            return ticks;
        }

        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, 
                                    object correlationState)
        {
            long startime = (long)correlationState;

            TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startime);

            _customThreadPool.QueuePerfmonWorkItem(CustomPerfmonCounterType.TIMETAKEN, reply.Headers.Action, (int)elapsedSpan.TotalMilliseconds, startime, string.Empty);

            
        }

		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			long ticks = DateTime.Now.Ticks;


			var action = request.Headers.Action ?? "Unknown";

			action = rgx.Replace(action, "");
			_customThreadPool.QueuePerfmonWorkItem(CustomPerfmonCounterType.EXECUTING, action, 0, ticks, request.ToString());

			ServiceCallInfo callInfo;
			callInfo.Action = action;
			callInfo.Ticks = ticks;

			return callInfo;
		}

		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
			ServiceCallInfo callInfo = (ServiceCallInfo)correlationState;

			TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - callInfo.Ticks);

			
			_customThreadPool.QueuePerfmonWorkItem(CustomPerfmonCounterType.TIMETAKEN, callInfo.Action, (int)elapsedSpan.TotalMilliseconds, callInfo.Ticks, string.Empty);

            
		}


		private struct ServiceCallInfo
		{
			public string Action;
			public long Ticks;
		}
    }
}
