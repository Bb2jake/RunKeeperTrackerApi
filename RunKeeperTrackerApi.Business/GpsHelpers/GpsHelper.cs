using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using GeoCoordinatePortable;
using RunKeeperTrackerApi.Models;

namespace RunKeeperTrackerApi.Business.GpsHelpers
{
	public class GpsHelper : IGpsHelper
	{
		public IEnumerable<ActivityRecord> GetGpsData(DateTime startTime, DateTime endTime, ActivityType activity)
		{
			var dataDirectory = "C:\\Users\\JPoole\\source\\repos\\RunKeeperTrackerApi\\RunKeeperTrackerApi.Data\\Gps Data";
			var gpsFiles = Directory.GetFiles(dataDirectory, "*.gpx");

			// Filter by date and activity type here

			return gpsFiles.Select(filePath => ParseData(filePath, activity)).Where(a => a != null);
		}

		private ActivityRecord ParseData(string filePath, ActivityType activity)
		{
			var gpsData = XElement.Load(filePath);
			var xmlns = XNamespace.Get("http://www.topografix.com/GPX/1/1");
			if (!gpsData.Element(xmlns + "trk").Element(xmlns + "name").Value.Contains("Running")) return null;

			var coords = gpsData.Descendants(xmlns + "trkpt").Select(l => new Coordinates
			{
				Lat = double.Parse(l.Attribute("lat").Value),
				Lon = double.Parse(l.Attribute("lon").Value),
				Elevation = double.Parse(l.Element(xmlns + "ele").Value),
				Time = DateTime.Parse(l.Element(xmlns + "time").Value)
			}).ToList();

			var legs = new List<Leg>();
			for (var i = 0; i < coords.Count - 1; i++)
			{
				var coord1 = coords[i];
				var coord2 = coords[i + 1];

				legs.Add(new Leg
				{
					Distance = GetDistanceTraveled(coord1.Lat, coord1.Lon, coord2.Lat, coord2.Lon),
					ElevationChange = coord2.Elevation - coord1.Elevation,
					Time = coord2.Time - coord1.Time
				});
			}


			return new ActivityRecord
			{
				Time100M = GetBestTime(legs, 100),
				Time500M = GetBestTime(legs, 500),
				Time1K = GetBestTime(legs, 1000),
				Time5K = GetBestTime(legs, 5000),
				Time10K = GetBestTime(legs, 10000),
				DateTime = DateTime.ParseExact(filePath.Substring(filePath.LastIndexOf('\\') + 1, filePath.Length - (filePath.LastIndexOf('\\') + 1) - 4), "yyyy-MM-dd-HHmmss", null).ToShortDateString()
			};
		}

		private double? GetBestTime(IList<Leg> legs, double targetDistance)
		{
			var bestTime = TimeSpan.Zero;

			for (var i = 0; i < legs.Count; i++)
			{
				double distance = 0;
				var time = TimeSpan.Zero;
				for (var j = i; j < legs.Count; j++)
				{
					distance += legs[j].Distance;
					time += legs[j].Time;
					if (!(distance >= targetDistance)) continue;

					if (bestTime == TimeSpan.Zero || time < bestTime)
					{
						bestTime = time;
					}

					break;
				}
			}

			return bestTime == TimeSpan.Zero ? null : (double?) bestTime.TotalSeconds;
		}

		/// <summary>
		/// Gets distance traveled in meters
		/// </summary>
		/// <returns></returns>
		private static double GetDistanceTraveled(double lat1, double lon1, double lat2, double lon2)
		{
			var coord1 = new GeoCoordinate(lat1, lon1);
			var coord2 = new GeoCoordinate(lat2, lon2);
			return coord1.GetDistanceTo(coord2);
		}
	}
}
