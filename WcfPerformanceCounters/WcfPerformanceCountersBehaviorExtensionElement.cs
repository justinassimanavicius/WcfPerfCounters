using System;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace WcfPerformanceCounters
{
	internal class WcfPerformanceCountersBehaviorExtensionElement : BehaviorExtensionElement
	{
		public override Type BehaviorType
		{
			get { return typeof (WcfPerformanceCountersEndpointBehavior); }
		}


		[ConfigurationProperty("captureDumpAfterSeconds")]
		public long captureDumpAfterSeconds
		{
			get { return (long) this["captureDumpAfterSeconds"]; }
		}

		[ConfigurationProperty("dumpCmd")]
		public string dumpCmd
		{
			get { return (string) this["dumpCmd"]; }
		}

		[ConfigurationProperty("dumpLimit")]
		public int dumpLimit
		{
			get { return (int) this["dumpLimit"]; }
		}

		[ConfigurationProperty("pollIntervalSeconds")]
		public int pollIntervalSeconds
		{
			get { return (int) this["pollIntervalSeconds"]; }
		}

		protected override object CreateBehavior()
		{
			var config = new WcfPerformanceCountersConfig()
			{
				dumpCmd = dumpCmd,
				captureDumpAfterSeconds = captureDumpAfterSeconds,
				bHangDump = captureDumpAfterSeconds > 0 ? true : false,
				pollIntervalSeconds = pollIntervalSeconds,
				dumpLimit = dumpLimit
			};

			if (config.bHangDump)
			{
				//
				//  check the values
				//
				if (config.pollIntervalSeconds < 1)
				{
					config.pollIntervalSeconds = 1;
				}
				if (config.dumpLimit < 1)
				{
					config.dumpLimit = 1;
				}
				if (string.IsNullOrWhiteSpace(config.dumpCmd))
				{
					config.bHangDump = false;
				}
			}
			return new WcfPerformanceCountersEndpointBehavior(config);
		}
	}
}