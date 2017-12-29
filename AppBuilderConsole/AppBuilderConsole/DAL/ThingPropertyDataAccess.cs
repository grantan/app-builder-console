using AppBuilderConsole.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AppBuilderConsole.DAL
{
	public class ThingPropertyDataAccess
	{
		//private SqlConnection connection = null;
		private SqlConnection connection = null;
		private string constr = System.Configuration.ConfigurationManager.ConnectionStrings["AppBuilderConnectionString"].ToString();
		DataAccess _da;
		ThingDataAccess _tda;
		string _procName;

		public ThingProperty GetThingProperty(int id)
		{
			_da = new DataAccess();
			_procName = "GetThingProperty";
			SqlParameter[] pars = new SqlParameter[1];  //GetSqlParametersFromObject()
														//= new SqlParam[size_of_type_attribute_list-1]
			pars[0] = new SqlParameter("@id", id);
			ThingPropertyDTO thingPropertyDTO = _da.GetObjectByParameters<ThingPropertyDTO>(constr, _procName, pars);
			ThingProperty thingProperty = new ThingProperty();
			thingProperty.ThingPropertyId = thingPropertyDTO.ThingPropertyId;
			thingProperty.PropertyName = thingPropertyDTO.PropertyName;
			thingProperty.PropertyDescription = thingPropertyDTO.PropertyDescription;
			thingProperty.IsList = thingPropertyDTO.IsList;
			thingProperty.SequenceOrder = thingPropertyDTO.SequenceOrder;
			ThingDataAccess tda = new ThingDataAccess();
			Thing ownedThing = tda.GetThingByID(thingPropertyDTO.OwnedThingId);
			thingProperty.OwnedThing = ownedThing;

			return thingProperty;			
		}

		public Thing GetOwnerByThingPropertyId(int thingPropertyId)
		{			
			_da = new DataAccess();
			_procName = "GetOwnerByThingPropertyId";
			SqlParameter[] pars = new SqlParameter[1];  //GetSqlParametersFromObject()
														//= new SqlParam[size_of_type_attribute_list-1]
			pars[0] = new SqlParameter("@ThingPropertyId", thingPropertyId);
			Thing Thing = _da.GetObjectByParameters<Thing>(constr, _procName, pars);
			return Thing;					
		}

		public void UpdateThingProperty(ThingProperty thingProperty)
		{
			_da = new DataAccess();
			_procName = "UpdateThingProperty";
			SqlParameter[] pars = new SqlParameter[6];  //= new SqlParam[size_of_type_attribute_list-1]

			pars[0] = new SqlParameter("@thingPropertyId", thingProperty.ThingPropertyId);
			pars[1] = new SqlParameter("@propertyId", thingProperty.OwnedThing.Id);
			pars[2] = new SqlParameter("@name", thingProperty.PropertyName);
			pars[3] = new SqlParameter("@description", thingProperty.PropertyDescription);
			pars[4] = new SqlParameter("@isList", thingProperty.IsList);
			pars[5] = new SqlParameter("@sequenceOrder", thingProperty.SequenceOrder);

			int rowsAffected = _da.UpdateDeleteObject(constr, _procName, pars);			
		}

		public void DeleteThingProperty(int id)
		{
			_da = new DataAccess();
			_procName = "DeleteThingProperty";
			SqlParameter[] pars = new SqlParameter[1];  //GetSqlParametersFromObject()
														//= new SqlParam[size_of_type_attribute_list-1]
			pars[0] = new SqlParameter("@thingPropertyId", id);
			int rowsAffected =_da.UpdateDeleteObject(constr, _procName, pars);
		}

		public void InsertThingProperty(int ownerThingId, int propertyThingId, string name, string description, bool isList, int order)
		{
			_da = new DataAccess();
			_procName = "AddThingProperty";
			SqlParameter[] pars = new SqlParameter[6];  //= new SqlParam[size_of_type_attribute_list-1]

			pars[0] = new SqlParameter("@ownerId", ownerThingId);
			pars[1] = new SqlParameter("@propertyId", propertyThingId);
			pars[2] = new SqlParameter("@name", name);
			pars[3] = new SqlParameter("@description", description);
			pars[4] = new SqlParameter("@isList", isList);
			pars[5] = new SqlParameter("@sequenceOrder", order);
			int thingPropertyId = _da.InsertForIdentity(constr, _procName, pars);
		}

		public List<ThingProperty> GetThingProperties(int ownerThingId)
		{
			_da = new DataAccess();
			_tda = new ThingDataAccess();
			_procName = "GetOwnerPropertyList";
			SqlParameter[] pars = new SqlParameter[1];  //= new SqlParam[size_of_type_attribute_list-1]

			pars[0] = new SqlParameter("@ownerId", ownerThingId);
			List<ThingPropertyDTO> thingPropertyDTOs = _da.GetObjectList<ThingPropertyDTO>(constr, _procName, pars);
			List<ThingProperty> thingProperties = new List<ThingProperty>();

			foreach (ThingPropertyDTO dto in thingPropertyDTOs)
			{
				ThingProperty thingProperty = new ThingProperty();
				thingProperty.ThingPropertyId = dto.ThingPropertyId;
				thingProperty.PropertyName = dto.PropertyName;
				thingProperty.PropertyDescription = dto.PropertyDescription;
				thingProperty.IsList = dto.IsList;
				thingProperty.SequenceOrder = dto.SequenceOrder;
				thingProperty.OwnedThing = _tda.GetThingByID(dto.OwnedThingId);
				thingProperties.Add(thingProperty);
			}
			return thingProperties;						
		}

	}
}