using System.Diagnostics;
using System.ServiceModel.Dispatcher;

namespace WcfPerformanceCounters
{
    internal class ParameterInspector : IParameterInspector
    {
        private readonly Worker _worker;
        private readonly string _instanceNameFormat;

        public ParameterInspector(Worker worker, string instanceNameFormat)
        {
            _worker = worker;
            _instanceNameFormat = instanceNameFormat;
        }

        public object BeforeCall(string operationName, object[] inputs)
        {
            var item = new WorkItem
            {
                CounterType = OperationState.BeforeCall,
                Action = GetActionName(operationName)
            };

            _worker.QueueWorkItem(item);

            var stopWatch = Stopwatch.StartNew();

            var callInfo = new ServiceCallInfo
            {
                StopWatch = stopWatch
            };

            return callInfo;
        }

        private string GetActionName(string operationName)
        {
            return string.Format(_instanceNameFormat, operationName);
        }

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
            var callInfo = (ServiceCallInfo)correlationState;
            callInfo.StopWatch.Stop();

            var item = new WorkItem
            {
                CounterType = OperationState.AfterCall,
                Action = GetActionName(operationName),
                DurationInTicks = callInfo.StopWatch.ElapsedTicks,
                DurationInMs = callInfo.StopWatch.ElapsedMilliseconds
            };
            _worker.QueueWorkItem(item);
        }
       
        private class ServiceCallInfo
        {
            public Stopwatch StopWatch { get; set; }
        }
    }
}