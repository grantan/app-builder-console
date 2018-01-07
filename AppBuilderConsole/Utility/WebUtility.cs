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
			sb.Append("CodeBehind =\""+thing.Name+"List.aspx.cs\" Inherits=\""+ thing.Name + "." + thing.Name + "List\" %>");
			sb.Append(NewLineTab(0));
			sb.Append("< !DOCTYPE html >");
			sb.Append(NewLineTab(0));
			sb.Append("< html xmlns = \"http://www.w3.org/1999/xhtml\" > ");
			sb.Append(NewLineTab(0));
			sb.Append("< head runat = \"server\" > ");
			sb.Append(NewLineTab(1));
			sb.Append("< title >");
			sb.Append(NewLineTab(2));
			sb.Append(thing.Name + " List");
			sb.Append(NewLineTab(1));
			sb.Append("</ title >");
			sb.Append(NewLineTab(0));
			sb.Append("</ head >");
			sb.Append(NewLineTab(0));
			sb.Append("< body >");
			sb.Append(NewLineTab(1));
			sb.Append(thing.Name);
			sb.Append(NewLineTab(0));
			sb.Append("</ body >");
			sb.Append(NewLineTab(0));
			sb.Append("</ html >");

			//write main web thing page
			string thingWebPath = _fileUtil.WriteFile(sb.ToString(), webFolderPath + "\\" + thing.Name + "List.aspx");

			thingWebPath = WriteThingWebFormAspxCodeBehind(thing, webFolderPath);
			//write main web thing crud
			//string thingPath = _fileUtil.WriteFile(webFolderPath, mainRepoProjectPath + "\\.gitignore");


			//foreach (Thing projectThing in fullThingList)
			//{
			//	projectThing.PropertyList = _tpda.GetThingProperties(projectThing.Id);
			//	WriteThingWebFormAspx(projectThing, webFolderPath);
			//}

			return mapPath;
		}

		private string WriteThingWebFormAspxCodeBehind(Thing thing, string webFolderPath)
		{
			//build string
			StringBuilder sb = new StringBuilder();

			sb.Append("using " + thing.Name + ".DAL;");
			sb.Append(NewLineTab(0));
			sb.Append("using " + thing.Name + ".Models;");
			sb.Append(NewLineTab(0));
			sb.Append("using System;");
			sb.Append(NewLineTab(0));
			sb.Append("using System.Collections.Generic;");
			sb.Append(NewLineTab(0));
			sb.Append("using System.Linq;");
			sb.Append(NewLineTab(0));
			sb.Append("using System.Web;");
			sb.Append(NewLineTab(0));
			sb.Append("using System.Web.UI;");
			sb.Append(NewLineTab(0));
			sb.Append("using System.Web.UI.WebControls;");
			sb.Append(NewLineTab(0));
			sb.Append(Carriage());

			sb.Append("namespace " + thing.Name);
			sb.Append(NewLineTab(0));
			sb.Append("{");
			sb.Append(NewLineTab(1));
			sb.Append("public partial class " + thing.Name+ "List : System.Web.UI.Page");
			sb.Append(NewLineTab(1));
			sb.Append("{");
			sb.Append(NewLineTab(2));
			sb.Append("protected void Page_Load(object sender, EventArgs e)");
			sb.Append(NewLineTab(2));
			sb.Append("{");
			sb.Append(NewLineTab(3));
			sb.Append("if (!IsPostBack)");
			sb.Append(NewLineTab(3));
			sb.Append("{");
			sb.Append(NewLineTab(4));
			sb.Append("BindGrid();");
			sb.Append(NewLineTab(3));
			sb.Append("}");
			sb.Append(NewLineTab(2));
			sb.Append("}");
			sb.Append(NewLineTab(1));
			sb.Append("}");
			sb.Append(NewLineTab(0));
			sb.Append("}");			

			string thingWebPath = _fileUtil.WriteFile(sb.ToString(), webFolderPath + "\\" + thing.Name + "List.aspx.cs");

			return webFolderPath;

		}

		private string NewLineTab(int num)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("\n");
			sb.Append(Tab(num));
			return sb.ToString();
		}

		private string NewLine()
		{
			return "\n";
		}

		private string Carriage()
		{
			return "\r";
		}

		private string Tab(int num)
		{
			StringBuilder sb = new StringBuilder();
			for(int i=0; i<num; i++)
			{
				sb.Append("\t");
			}
			return sb.ToString();
		}
	}
}

