using AppBuilderConsole.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AppBuilderConsole.DAL
{
	public class ThingDataAccess
	{
		//private SqlConnection connection = null;
		private SqlConnection connection = null;
		private string constr = System.Configuration.ConfigurationManager.ConnectionStrings["AppBuilderConnectionString"].ToString();
		DataAccess _da;
		ThingPropertyDataAccess _tpda;
		string _procName;

		public Thing NewThing(Thing thing)
		{
			_da = new DataAccess();
			_procName = "NewThing";
			SqlParameter[] pars = new SqlParameter[3];  //= new SqlParam[size_of_type_attribute_list-1]

			pars[0] = new SqlParameter("@Name", thing.Name);
			pars[1] = new SqlParameter("@Description", thing.Description);
			pars[2] = new SqlParameter("@ThingTypeId", thing.ThingTypeID);
			thing.Id = _da.InsertForIdentity(constr, _procName, pars);
			
			return thing;
		}

		public Thing GetThingHierarchy(int thingId)  //collapsed onto this thing
		{
			//_tpda = new ThingPropertyDataAccess();
			Thing thing = GetThingByID(thingId);
			thing.PropertyList = GetAllThingProperties(thingId);
			return thing;
			//return GetFullThing(thing);			
		}

		/// <summary>
		/// Include parent objects and their associated properties
		/// </summary>
		/// <param name="thingId"></param>
		/// <returns></returns>
		public List<Thing> GetFullThingHierarchy(int thingId)
		{
			_tpda = new ThingPropertyDataAccess();
			List<Thing> thingList = new List<Thing>();
			Thing thing = GetThingByID(thingId);
			thing.PropertyList = _tpda.GetThingProperties(thingId);
			thingList.Add(thing);
			if (thingId == 1)
			{							
				return thingList;
			}
			
			//thingList.Add(GetFullThingHierarchy(thingId));
			thingList.AddRange(GetFullThingHierarchy(thing.ThingTypeID));

			//mainThing.PropertyList = GetAllThingProperties(thingId);

			return thingList;
			//return GetFullThing(thing);
		}

		//private List<Thing> GetThingHierarchyList(thingId)
		//{
		//	Thing thing = GetThingByID(thingId);
		//	List<ThingProperty> thingProperties = new List<ThingProperty>();
		//	List<Thing> thingList;
		//	if (thingId == 1)
		//	{

		//	}
		//	 = GetThingHierarchyList(thingId);
		//	thingList.AddRange()
		//	Thing mainThing = GetThingByID(thingId);
		//	return thingList;
		//}

		//public Thing GetFullThing(Thing currentThing)
		//{
		//	currentThing.PropertyList.AddRange(GetFullThingProperties(currentThing.Id));
		//	if (currentThing.Id == 1)  //Or base type thing -- base case
		//	{				
		//		return currentThing;
		//	}
		//	//Thing parentThing = GetThingByID(currentThing.ThingTypeID);
		//	if (currentThing.PropertyList == null)
		//	{
		//		currentThing.PropertyList = new List<ThingProperty>();
		//	}
		//	Thing parentThing = GetThingByID(currentThing.ThingTypeID);
		//	parentThing.PropertyList = new List<ThingProperty>();
		//	parentThing = GetFullThing(parentThing);
		//	currentThing.PropertyList.AddRange(GetFullThingProperties(parentThing.ThingTypeID));

		//}

		/// <summary>
		/// Recursive method to project all parent properties onto the current class
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		private List<ThingProperty> GetAllThingProperties(int id)
		{
			List<ThingProperty> allThingProperties;
			Thing currentThing = GetThingByID(id);
			_tpda = new ThingPropertyDataAccess();
			allThingProperties = _tpda.GetThingProperties(id);
			if (id == 1)
			{
				return allThingProperties;
			}

			//foreach (ThingProperty tProp in fullThingProperties)
			//{
			//	Thing thing = GetThingByID(tProp.OwnedThing.Id);
			//	thing.PropertyList = new List<ThingProperty>();
			//	tProp.OwnedThing = GetFullThing(thing);
			//}

			allThingProperties.AddRange(GetAllThingProperties(currentThing.ThingTypeID));
			return allThingProperties;
		}	


		//public string GetThingAndPropertyHierarchy(int thingId)
		//{
		//	_da = new DataAccess();
		//	_procName = "GetThingHierarchy";
		//	SqlParameter[] pars = new SqlParameter[1];  //GetSqlParametersFromObject()
		//												//= new SqlParam[size_of_type_attribute_list-1]
		//	pars[0] = new SqlParameter("@Id", thingId);
		//	Thing Thing = _da.GetObjectByParameters<Thing>(constr, _procName, pars);
		//	return thing;
		//}

		public Thing GetThingByID(int ThingID)
		{
			_da = new DataAccess();
			_procName = "GetThingByID";
			SqlParameter[] pars = new SqlParameter[1];  //GetSqlParametersFromObject()
				  //= new SqlParam[size_of_type_attribute_list-1]
			pars[0] = new SqlParameter("@Id", ThingID);
			Thing Thing = _da.GetObjectByParameters<Thing>(constr, _procName, pars);
			return Thing;			
		}

		public Thing GetThingByIDWithProperties(int ThingID)
		{
			_da = new DataAccess();
			_procName = "GetThingByID";
			SqlParameter[] pars = new SqlParameter[1];  //GetSqlParametersFromObject()
														//= new SqlParam[size_of_type_attribute_list-1]
			pars[0] = new SqlParameter("@Id", ThingID);
			Thing thing = _da.GetObjectByParameters<Thing>(constr, _procName, pars);

			thing.PropertyList = _tpda.GetThingProperties(thing.Id);
			return thing;			
		}

		public List<Thing> GetThingList()
		{
			_da = new DataAccess();
			_procName = "AllThings";
			List<Thing> things = _da.GetObjectList<Thing>(constr, _procName);
			return things;			
		}

		public void EditThing(Thing thing)
		{
			_da = new DataAccess();
			_procName = "UpdateThing";
			SqlParameter[] pars = new SqlParameter[4];  //= new SqlParam[size_of_type_attribute_list-1]

			pars[0] = new SqlParameter("@Id", thing.Id);
			pars[1] = new SqlParameter("@Name", thing.Name);
			pars[2] = new SqlParameter("@Description", thing.Description);
			pars[3] = new SqlParameter("@ThingTypeId", thing.ThingTypeID);

			int rowsAffected = _da.UpdateDeleteObject(constr, _procName, pars);	
		}		

		//public void SaveThingInheritance(int thingTypeID, int thingID)
		//{
		//	_da = new DataAccess();
		//	_procName = "AddThingInheritance";
		//	SqlParameter[] pars = new SqlParameter[2];  //= new SqlParam[size_of_type_attribute_list-1]

		//	pars[0] = new SqlParameter("@Name", thing.Name);
		//	pars[1] = new SqlParameter("@Description", thing.Description);
		//	thing.Id = _da.InsertForIdentity(constr, _procName, pars);

		//	return thing;

		//	//Tell the SqlCommand what query to execute and what SqlConnection to use.  
		//	using (SqlCommand command = new SqlCommand("AddThingInheritance", connection))  //
		//	{
		//		command.CommandType = CommandType.StoredProcedure;

		//		//Add SqlParameters to the SqlCommand  
		//		command.Parameters.AddWithValue("@parentId", thingTypeID);
		//		command.Parameters.AddWithValue("@childId", thingID);

		//		//try to open the connection
		//		try
		//		{
		//			connection.Open();
		//		}
		//		catch (Exception ex)
		//		{
		//			//There is a problem connecting to the instance of the SQL Server.  
		//			//For example, the connection string might be wrong,  
		//			//or the SQL Server might not be available to you. 
		//			string error = ex.GetBaseException().ToString();
		//		}

		//		//Execute the query.  
		//		try
		//		{
		//			command.ExecuteNonQuery();
		//		}

		//		catch (Exception ex)
		//		{
		//			//There was a problem executing the query. For examaple, your SQL statement  
		//			//might be wrong, or you might not have permission to create records in the  
		//			//specified table. 
		//			string error = ex.ToString();
		//		}

		//	}
			
		//}		

		/// <summary>
		/// ?needed
		/// </summary>
		/// <param name="Thing"></param>
		public void DisableThing(Thing Thing)
		{
			connection = new SqlConnection(constr);

			using (SqlCommand command = new SqlCommand("DisableThing", connection))
			{
				command.CommandType = CommandType.StoredProcedure;

				//Add SqlParameters to the SqlCommand  
				command.Parameters.AddWithValue("@ID", Thing.Id);

				//try to open the connection
				try
				{
					connection.Open();
				}
				catch (Exception ex)
				{
					//There is a problem connecting to the instance of the SQL Server.  
					//For example, the connection string might be wrong,  
					//or the SQL Server might not be available to you. 
					string error = ex.GetBaseException().ToString();
				}

				//Execute the query.  
				try
				{
					int rowsAffected = Int32.Parse(command.ExecuteNonQuery().ToString());

				}
				catch (Exception ex)
				{
					//There was a problem executing the query. For examaple, your SQL statement  
					//might be wrong, or you might not have permission to create records in the  
					//specified table. 
					string error = ex.ToString();
				}

				//is this necessary?
				finally
				{
					connection.Close();
				}
			}

		}
	}
}