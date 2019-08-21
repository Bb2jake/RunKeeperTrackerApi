using System;
using System.Collections.Generic;
using RunKeeperTrackerApi.Models;

namespace RunKeeperTrackerApi.Business.GpsHelpers
{
	public interface IGpsHelper
	{
		IEnumerable<ActivityRecord> GetGpsData(DateTime startTime, DateTime endTime, ActivityType activity);
	}
}
