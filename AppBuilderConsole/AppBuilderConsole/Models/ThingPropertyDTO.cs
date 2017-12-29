using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppBuilderConsole.Models
{
	public class ThingPropertyDTO
	{
		public int ThingPropertyId { get; set; }	
		public string PropertyName { get; set; }
		public string PropertyDescription { get; set; }
		public bool IsList { get; set; }
		public int SequenceOrder { get; set; }
		public int OwnedThingId { get; set; }
	}
}