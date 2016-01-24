using System;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace WcfPerformanceCounters
{
	internal class WcfPerformanceCountersBehaviorExtensionElement : BehaviorExtensionElement
	{
		public override Type BehaviorType => typeof (WcfPerformanceCountersEndpointBehavior);
        
		[ConfigurationProperty("instanceNameFormat")]
		public string InstanceNameFormat => this["instanceNameFormat"] as string;

	    protected override object CreateBehavior()
		{
			var config = new WcfPerformanceCountersConfig
			{
				InstanceNameFormat = string.IsNullOrWhiteSpace(InstanceNameFormat) ? "{0}" : InstanceNameFormat
			};
            
			return new WcfPerformanceCountersEndpointBehavior(config);
		}
	}
}