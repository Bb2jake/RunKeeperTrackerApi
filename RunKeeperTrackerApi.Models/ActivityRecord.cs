using System;

namespace RunKeeperTrackerApi.Models
{
	public class ActivityRecord
	{
		public double? Time100M { get; set; }
		public double? Time500M { get; set; }
		public double? Time1K { get; set; }
		public double? Time5K { get; set; }
		public double? Time10K { get; set; }
		public string DateTime { get; set; }
	}
}
