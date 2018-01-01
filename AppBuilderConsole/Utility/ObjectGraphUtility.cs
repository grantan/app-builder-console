using AppBuilderConsole.DAL;
using AppBuilderConsole.Models;
//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace AppBuilderConsole.Utility
{
	public class ObjectGraphUtility
	{
		private ThingDataAccess _tda;
		private ThingPropertyDataAccess _tpda;
		private FileIOUtility _fileUtil;

		public ObjectGraphUtility()
		{
			_tda = new ThingDataAccess();
			_tpda = new ThingPropertyDataAccess();
			_fileUtil = new FileIOUtility();
		}

		public string GetThingHierarchy(int thingId)
		{
			//Inflate your parent, inflate its parent, etc
			string returnString = null;

			Thing currentThing = _tda.GetThingByID(thingId);

			List<ThingProperty> inheritedProperties = GetInheritedPropertyList(currentThing);
			currentThing.PropertyList.AddRange(inheritedProperties);
			return returnString;
		}

		private List<ThingProperty> GetInheritedPropertyList(Thing thing) 
		{
			//Thing parentThing = _tda.GetThingByID(thing.ThingTypeID);
			return null;
		}

		/// <summary>
		/// Creates a repository (if necessary) and project folders (if necessary) for a ThingWebsite at the location specified by the mapPath parameter
		/// AUTOMATE THIS from main thing program
		/// </summary>
		/// <param name="thing"></param>
		/// <param name="mapPath"></param>
		/// <returns></returns>
		public string WriteThingProjectModel(List<Thing> fullThingList, string mapPath)
		{
			//main project folder
			string mainRepoPath = _fileUtil.WriteFolderIfNotExists(mapPath);

			Thing fullThing = fullThingList.FirstOrDefault();

			//make a folder (git) for repo
			string mainRepoProjectPath = _fileUtil.WriteFolderIfNotExists(mainRepoPath + "\\" + fullThing.Name);

			string webConfigContent = GetBasicWebConfigContent();
			string webConfigPath = _fileUtil.WriteFile(webConfigContent, mainRepoProjectPath + "\\web.config");
			//TODO put together the .gitignore file here from configurable xml or something
			//string gitIgnoreContent = "*.suo";
			string gitIgnoreContent = GetBasicGitIgnoreContent();
			string gitIgnorePath = _fileUtil.WriteFile(gitIgnoreContent, mainRepoProjectPath + "\\.gitignore");

			//make a folder (asp.net) for website code and resources
			string mainCodeProjectPath = _fileUtil.WriteFolderIfNotExists(mainRepoProjectPath + "\\" + fullThing.Name);

			//Write the Domain model folder (delete if it already exists)
			string modelsFolderPath = _fileUtil.WriteFolder(mainCodeProjectPath + "\\Models");

			foreach (Thing projectThing in fullThingList)
			{
				projectThing.PropertyList = _tpda.GetThingProperties(projectThing.Id);
				WriteThingModelCSharp(projectThing, modelsFolderPath);
			}

			return mapPath;
		}

		/// <summary>
		/// Write the class structure in C# recursively
		/// </summary>
		/// <param name="thing"></param>
		/// <param name="modelsFolderPath"></param>
		private void WriteThingModelCSharp(Thing fullThing, string modelsFolderPath)
		{
			//string jsonModel = JsonConvert.SerializeObject(fullThing, Formatting.Indented);

			//You need all the parent things to be able to inherit from them in code

			//Read each line of the string (or json) and build a series of model string that you can write into C# files
			StringBuilder sb = new StringBuilder();

			string mainThingName = fullThing.Name;
			string filePath = modelsFolderPath + "\\" + mainThingName + ".cs";
			if (fullThing.Id == 1) //base thing
			{
				if (!_fileUtil.FileExists(filePath))
				{
					sb.Append("public class " + mainThingName);
					sb.Append("\r\n{ ");
					sb.Append("\r\n");
					sb.Append(WriteThing(fullThing));
					sb.Append("\r\n}");
					
					_fileUtil.WriteFile(sb.ToString(), filePath);

					foreach (ThingProperty prop in fullThing.PropertyList)
					{
						Thing propThing = _tda.GetThingByID(prop.OwnedThing.Id);
						propThing.PropertyList = _tpda.GetThingProperties(propThing.Id);
						WriteThingModelCSharp(propThing, modelsFolderPath);
					}
				}				
			}
			
			else 
			{
				if (!_fileUtil.FileExists(filePath))
				{
					Thing parentThing = _tda.GetThingByID(fullThing.ThingTypeID);
					
					string parentThingName = parentThing.Name;
					sb.Append("public class " + mainThingName + " : " + parentThingName);
					sb.Append("\r\n{ ");
					sb.Append("\r\n");
					sb.Append(WriteThing(fullThing));

					sb.Append("\r\n}");

					_fileUtil.WriteFile(sb.ToString(), filePath);


					foreach (ThingProperty prop in fullThing.PropertyList)
					{
						Thing propThing = _tda.GetThingByID(prop.OwnedThing.Id);
						propThing.PropertyList = _tpda.GetThingProperties(propThing.Id);
						sb.Append("\t");
						WriteThingModelCSharp(propThing, modelsFolderPath);
					}

					parentThing.PropertyList = _tpda.GetThingProperties(parentThing.Id);
					WriteThingModelCSharp(parentThing, modelsFolderPath);
				}
			}

		}

		private string WriteThing(Thing fullThing)
		{
			StringBuilder sb = new StringBuilder();

			//List the properties of this thing
			//List<ThingProperty> props = _tpda.GetThingProperties(fullThing.Id);
			foreach (ThingProperty prop in fullThing.PropertyList)
			{
				sb.Append("\t");
				if (prop.IsList)
				{
					sb.Append("public List<" + prop.OwnedThing.Name + "> " + prop.PropertyName);
				}
				else
				{
					sb.Append("public " + prop.OwnedThing.Name + " " + prop.PropertyName);
				}

				sb.Append(" { get; set; } \r\n");
				
			}

			return sb.ToString();

		}

		private string GetBasicWebConfigContent()
		{
			StringBuilder sb = new StringBuilder();
			string webConfigContent = "<? xml version = \"1.0\" encoding = \"utf-8\" ?> < !-- For more information on how to configure your ASP.NET application, please visit https://go.microsoft.com/fwlink/?LinkId=169433 -->< configuration >";
			return webConfigContent;
		}

		/// <summary>
		/// TODO This could read a basic .gitignore file or copy the file
		/// </summary>
		/// <returns></returns>
		private string GetBasicGitIgnoreContent()
		{
			StringBuilder sb = new StringBuilder();
			string gitIgnoreContent = "*.suo";

			return gitIgnoreContent;
		}

		public string WriteGitRepoFolder(string mapPath)
		{

			return mapPath;
		}
	}
}