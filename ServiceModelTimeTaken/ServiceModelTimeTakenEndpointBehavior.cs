using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.ServiceModel.Description;

namespace ServiceModelTimeTaken
{
    class ServiceModelTimeTakenEndpointBehavior : IEndpointBehavior
    {
        private ServiceModelTimeTakenConfig serviceModelTimeTakenConfig;
        public ServiceModelTimeTakenEndpointBehavior(ServiceModelTimeTakenConfig config)
        {
            serviceModelTimeTakenConfig = config;
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint,
            System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(
                new ServiceModelTimeTakenMessageInspector(serviceModelTimeTakenConfig));
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
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
