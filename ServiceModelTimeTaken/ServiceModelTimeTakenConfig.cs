using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Configuration;
using System.Configuration;

namespace ServiceModelTimeTaken
{
    class ServiceModelTimeTakenConfig
    {
        public bool bHangDump;
        public long captureDumpAfterSeconds;
        public string dumpCmd;
        public int pollIntervalSeconds; // in milliseconds
        public int dumpLimit;
    }

    class ServiceModelTimeTakenBehaviorExtensionElement : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            ServiceModelTimeTakenConfig config = new ServiceModelTimeTakenConfig()
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
            return new ServiceModelTimeTakenEndpointBehavior( config );

        }

        public override Type BehaviorType
        {
            get { return typeof(ServiceModelTimeTakenEndpointBehavior); }
        }



        [ConfigurationProperty("captureDumpAfterSeconds")]
        public long captureDumpAfterSeconds
        {
            get
            {
                return (long)this["captureDumpAfterSeconds"];
            }

        }

        [ConfigurationProperty("dumpCmd")]
        public string dumpCmd
        {
            get
            {
                return (string)this["dumpCmd"];
            }
        }

        [ConfigurationProperty("dumpLimit")]
        public int dumpLimit
        {
            get
            {
                return (int)this["dumpLimit"];
            }
        }

        [ConfigurationProperty("pollIntervalSeconds")]
        public int pollIntervalSeconds
        {
            get
            {
                return (int)this["pollIntervalSeconds"];
            }
        }

    }
}
