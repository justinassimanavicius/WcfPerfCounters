using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace WcfPerformanceCounters
{
	internal class WcfPerformanceCountersEndpointBehavior : IEndpointBehavior
	{
        private static readonly Worker Worker = new Worker();

        private readonly WcfPerformanceCountersConfig _config;

		public WcfPerformanceCountersEndpointBehavior(WcfPerformanceCountersConfig config)
		{
			_config = config;
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
		}

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			foreach (ClientOperation operation in clientRuntime.Operations)
            {
                operation.ParameterInspectors.Add(new ParameterInspector(Worker, _config.InstanceNameFormat));
            }
        }

		public void Validate(ServiceEndpoint endpoint)
		{
		}
        
    }
}