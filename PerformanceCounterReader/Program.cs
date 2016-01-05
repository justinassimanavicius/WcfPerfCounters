using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace PerformanceCounterReader
{
	class Program
	{
		private static void Main(string[] args)
		{

			var counters = GetCounters(args[0]);

			Console.WriteLine("<prtg>");
			foreach (var perfCounter in counters)
			{
				var split = perfCounter.Id.Split('\\');
				var split2 = split[1].Split('(');

				var categoryName = split2[0];
				var counterName = split[2];
				var instanceName = split2[1].Substring(0, split2[1].Length - 1);

				if (PerformanceCounterCategory.InstanceExists(instanceName, categoryName))
				{
					var myCounter = new PerformanceCounter
					{
						CategoryName = categoryName,
						CounterName = counterName,
						InstanceName = instanceName
					};

					long raw = myCounter.RawValue;

					Console.WriteLine("<result>");
					Console.WriteLine("<channel>" + perfCounter.Name + "</channel>");
					Console.WriteLine("<value>" + raw + "</value>");
					Console.WriteLine("</result>");
				}
			}
			Console.WriteLine("</prtg>");
		}



		private static IEnumerable<PerfCounter> GetCounters(string path)
		{
			List<PerfCounter> cards = null;

			var serializer = new XmlSerializer(typeof(List<PerfCounter>));

			using (var reader = new StreamReader(path))
			{
				cards = (List<PerfCounter>) serializer.Deserialize(reader);
			}
			return cards;
		}
	}
}
