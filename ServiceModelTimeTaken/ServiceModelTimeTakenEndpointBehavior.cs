using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace ServiceModelTimeTaken
{
	internal class ServiceModelTimeTakenEndpointBehavior : IEndpointBehavior
	{
		private readonly ServiceModelTimeTakenConfig serviceModelTimeTakenConfig;

		public ServiceModelTimeTakenEndpointBehavior(ServiceModelTimeTakenConfig config)
		{
			serviceModelTimeTakenConfig = config;
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint,
			EndpointDispatcher endpointDispatcher)
		{
			endpointDispatcher.DispatchRuntime.MessageInspectors.Add(
				new ServiceModelTimeTakenMessageInspector(serviceModelTimeTakenConfig));
		}

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			clientRuntime.MessageInspectors.Add(new ServiceModelTimeTakenMessageInspector(serviceModelTimeTakenConfig));
		}

		public void Validate(ServiceEndpoint endpoint)
		{
		}
	}
}