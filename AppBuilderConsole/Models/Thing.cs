using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppBuilderConsole.Models
{
	public class Thing
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int ThingTypeID { get; set; }
		public List<ThingProperty> PropertyList { get; set; }
	}
}