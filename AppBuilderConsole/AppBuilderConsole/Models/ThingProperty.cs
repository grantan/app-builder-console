using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppBuilderConsole.Models
{
	public class ThingProperty
	{
		public int ThingPropertyId { get; set; }
		//public Thing OwnerThing { get; set; }
		public Thing OwnedThing { get; set; }
		public string PropertyName { get; set; }	
		public string PropertyDescription { get; set; }
		public bool IsList { get; set; }
		public int SequenceOrder { get; set; }
	}
}