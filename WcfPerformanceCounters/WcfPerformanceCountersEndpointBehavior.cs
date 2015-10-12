using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace WcfPerformanceCounters
{
	internal class WcfPerformanceCountersEndpointBehavior : IEndpointBehavior
	{
		private readonly WcfPerformanceCountersConfig _wcfPerformanceCountersConfig;

		public WcfPerformanceCountersEndpointBehavior(WcfPerformanceCountersConfig config)
		{
			_wcfPerformanceCountersConfig = config;
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint,
			EndpointDispatcher endpointDispatcher)
		{
			endpointDispatcher.DispatchRuntime.MessageInspectors.Add(
				new MessageInspector(_wcfPerformanceCountersConfig));
		}

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			clientRuntime.MessageInspectors.Add(new MessageInspector(_wcfPerformanceCountersConfig));
		}

		public void Validate(ServiceEndpoint endpoint)
		{
		}
	}
}