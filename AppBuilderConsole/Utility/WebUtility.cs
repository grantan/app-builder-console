using AppBuilderConsole.DAL;
using AppBuilderConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBuilderConsole.Utility
{
	class WebUtility
	{
		private ThingDataAccess _tda;
		private ThingPropertyDataAccess _tpda;
		private FileIOUtility _fileUtil;

		public WebUtility()
		{
			_tda = new ThingDataAccess();
			_tpda = new ThingPropertyDataAccess();
			_fileUtil = new FileIOUtility();
		}

		public string WriteThingWebFormAspx(Thing thing, string mapPath)
		{
			string mainRepoProjectPath = mapPath + "\\" + thing.Name;
			string mainCodeProjectPath = mainRepoProjectPath + "\\" + thing.Name;

			//Write the Domain model folder (delete if it already exists)
			string webFolderPath = _fileUtil.WriteFolder(mainCodeProjectPath + "\\Web");

			//build string
			StringBuilder sb = new StringBuilder();
			//string header = "<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ThingList.aspx.cs" Inherits="AppBuilder.ThingList" %>
			sb.Append("<%@ Page Language=\"C#\" AutoEventWireup=\"true\"");
			sb.Append("CodeBehind =\""+thing.Name+"Home.aspx.cs\" Inherits=\""+ thing.Name + "." + thing.Name + "Home\" %>");
			sb.Append("\n");
			sb.Append("< !DOCTYPE html >");
			sb.Append("\n");
			sb.Append("< html xmlns = \"http://www.w3.org/1999/xhtml\" > ");
			sb.Append("\n");
			sb.Append("< head runat = \"server\" > ");
			sb.Append("\n");
			sb.Append("< title >");
			sb.Append("\n");
			sb.Append(thing.Name);
			sb.Append("\n");
			sb.Append("</ title >");
			sb.Append("\n");
			sb.Append("</ head >");
			sb.Append("\n");
			sb.Append("< body >");
			sb.Append("\n");
			sb.Append(thing.Name);
			sb.Append("\n");
			sb.Append("</ body >");
			sb.Append("\n");
			sb.Append("</ html >");

			//write main web thing page
			string thingWebPath = _fileUtil.WriteFile(sb.ToString(), webFolderPath + "\\" + thing.Name + "Home.aspx");


			//write main web thing crud
			//string thingPath = _fileUtil.WriteFile(webFolderPath, mainRepoProjectPath + "\\.gitignore");


			//foreach (Thing projectThing in fullThingList)
			//{
			//	projectThing.PropertyList = _tpda.GetThingProperties(projectThing.Id);
			//	WriteThingWebFormAspx(projectThing, webFolderPath);
			//}

			return mapPath;
		}
	}
}

