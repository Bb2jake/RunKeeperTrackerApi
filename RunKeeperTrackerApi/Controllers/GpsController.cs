using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RunKeeperTrackerApi.Business.GpsHelpers;
using RunKeeperTrackerApi.Models;

namespace RunKeeperTrackerApi.Controllers
{
	public class GpsController : Controller
	{
		private readonly IGpsHelper _gpsHelper;

		public GpsController(IGpsHelper gpsHelper)
		{
			_gpsHelper = gpsHelper;
		}

		public IEnumerable<ActivityRecord> GetGpsData(DateTime startTime, DateTime endTime, ActivityType activity)
		{
			return _gpsHelper.GetGpsData(startTime, endTime, activity);
		}
	}
}
