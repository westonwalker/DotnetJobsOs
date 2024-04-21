using DotnetJobs.Application.Lib;

namespace DotnetJobs.Application.Helpers
{
	public static class LocationRestrictions
	{
		public static LocationRestriction? Find(string value)
		{
			var locationRestriction = GetAll().FirstOrDefault(x => x.Value == value);
			return locationRestriction;
		}

		public static List<LocationRestriction> GetAll()
		{
			return new List<LocationRestriction>()
			{
				new LocationRestriction()
				{
					Name = "Worldwide",
					Value = "worldwide",
					Icon = "🌎 ",
					Default = true
				},
				new LocationRestriction()
				{
					Name = "USA",
					Value = "usa",
					Icon = "",
				},
				new LocationRestriction()
				{
					Name = "UK",
					Value = "uk",
					Icon = "",
				},
				new LocationRestriction()
				{
					Name = "India",
					Value = "india",
					Icon = "",
				},
				new LocationRestriction()
				{
					Name = "North America",
					Value = "north_america",
					Icon = "",
				},
				new LocationRestriction()
				{
					Name = "Latin America",
					Value = "latin_america",
					Icon = "",
				},
				new LocationRestriction()
				{
					Name = "Europe",
					Value = "europe",
					Icon = "",
				},
				new LocationRestriction()
				{
					Name = "Canada",
					Value = "canada",
					Icon = "",
				},
				new LocationRestriction()
				{
					Name = "Asia",
					Value = "asia",
					Icon = "",
				},
				new LocationRestriction()
				{
					Name = "Africa",
					Value = "africa",
					Icon = "",
				},
				new LocationRestriction()
				{
					Name = "Oceania",
					Value = "oceania",
					Icon = "",
				},
				new LocationRestriction()
				{
					Name = "Middle East",
					Value = "middle_east",
					Icon = "",
				},
				new LocationRestriction()
				{
					Name = "Australia",
					Value = "australia",
					Icon = "",
				},
			};
		}
	}
}
