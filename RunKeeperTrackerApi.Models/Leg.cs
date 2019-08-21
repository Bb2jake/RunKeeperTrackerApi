using System;

namespace RunKeeperTrackerApi.Models
{
	public class Leg
	{
		public double Distance { get; set; }
		public double ElevationChange { get; set; }
		public TimeSpan Time { get; set; }
	}
}
