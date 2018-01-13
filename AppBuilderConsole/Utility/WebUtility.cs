﻿using AppBuilderConsole.DAL;
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

			bool success = WriteListAspx(thing, webFolderPath);
			success = WriteListAspxCodeBehind(thing, webFolderPath);

			success = WriteCreateAspx(thing, webFolderPath);
			success = WriteCreateAspxCodeBehind(thing, webFolderPath);

			success = WriteEditAspx(thing, webFolderPath);
			success = WriteEditAspxCodeBehind(thing, webFolderPath);

			return mapPath;
		}

		

		private bool WriteListAspx(Thing thing, string webFolderPath)
		{
			
			StringBuilder sb = new StringBuilder();
			int tabNum = 0;
			sb.Append(PageDirective(thing.Name, "List"));
			sb.Append(Doctype(tabNum));

			//sb.Append(HtmlTag(true, tabNum, "xmlns = \"http://www.w3.org/1999/xhtml\""));
			var tagDictionary = new Dictionary<string, string>();
			tagDictionary.Add("xmlns", "\"http://www.w3.org/1999/xhtml\"");
			sb.Append(HtmlTabTag(true, tabNum, "html", tagDictionary));

			tagDictionary = new Dictionary<string, string>();
			tagDictionary.Add("runat", "\"server\"");
			sb.Append(HtmlTabTag(true, ++tabNum, "head", tagDictionary));
			//sb.Append(HeadTag(true, ++tabNum, "runat = \"server\""));

			
			//headDictionary.Add("runat", "\"server\"");
			sb.Append(HtmlTabTag(true, ++tabNum, "title"));
			//sb.Append(TitleTag(true, ++tabNum));
			sb.Append(TextValue(thing.Name + " List", ++tabNum));

			sb.Append(HtmlTabTag(false, --tabNum, "title")); //title
															 

			sb.Append(HtmlTabTag(false, --tabNum, "head"));
			//sb.Append(HeadTag(false, --tabNum));

			sb.Append(HtmlTabTag(false, tabNum, "body"));
			//sb.Append(BodyTag(true, tabNum));
			sb.Append(HtmlTabTag(true, ++tabNum, "div"));

			tagDictionary = new Dictionary<string, string>();
			tagDictionary.Add("ID", "gv" + thing.Name);
			tagDictionary.Add("class", "gridviewstyle");			
			sb.Append(HtmlTabTag(true, ++tabNum, "asp:GridView", tagDictionary));

			
			sb.Append(HtmlTabTag(true, ++tabNum, "Columns"));

			sb.Append(AspTemplateFieldLabel("Id", tabNum));
			sb.Append(AspTemplateFieldLabel("Name", tabNum));
			sb.Append(AspTemplateFieldLabel("Description", tabNum));
			sb.Append(AspTemplateFieldLinkButton("Select", tabNum));



			sb.Append(HtmlTabTag(false, --tabNum, "Columns"));
			//Gridview closing tag
			sb.Append(HtmlTabTag(false, --tabNum, "asp:GridView"));

			sb.Append(HtmlTabTag(false, --tabNum, "div"));
			sb.Append(HtmlTabTag(false, --tabNum, "body"));
			
			sb.Append(HtmlTag(false, --tabNum));		
			
			string thingWebPath = _fileUtil.WriteFile(sb.ToString(), webFolderPath + "\\" + thing.Name + "List.aspx");

			return !String.IsNullOrEmpty(thingWebPath);
		}

		private string AspTemplateFieldLabel(string propertyName, int tabNum)
		{
			StringBuilder sb = new StringBuilder();
			Dictionary<string, string> tagDictionary = new Dictionary<string, string>();
			tagDictionary.Add("HeaderText", propertyName);
			sb.Append(HtmlTabTag(true, ++tabNum, "asp:TemplateField", tagDictionary));
			sb.Append(HtmlTabTag(true, ++tabNum, "ItemTemplate"));

			tagDictionary = new Dictionary<string, string>();
			tagDictionary.Add("runat", "server");
			tagDictionary.Add("ID", "lbl" + propertyName);
			tagDictionary.Add("Text", "'<%#Eval(\"" + propertyName + "\") %>'");
			tagDictionary.Add("Enabled", "false");
			sb.Append(HtmlTabTag(true, ++tabNum, "asp:Label", tagDictionary));
			sb.Append(HtmlTabTag(false, tabNum, "asp:Label"));

			sb.Append(HtmlTabTag(false, --tabNum, "ItemTemplate"));
			sb.Append(HtmlTabTag(false, --tabNum, "asp:TemplateField"));

			return sb.ToString();
		}

		private string AspTemplateFieldLinkButton(string propertyName, int tabNum)
		{
			StringBuilder sb = new StringBuilder();
			Dictionary<string, string> tagDictionary = new Dictionary<string, string>();
			tagDictionary.Add("HeaderText", propertyName);
			sb.Append(HtmlTabTag(true, ++tabNum, "asp:TemplateField", tagDictionary));
			sb.Append(HtmlTabTag(true, ++tabNum, "ItemTemplate"));

			tagDictionary = new Dictionary<string, string>();
			tagDictionary.Add("runat", "server");
			tagDictionary.Add("ID", "lbtn" + propertyName);
			tagDictionary.Add("Text", propertyName);
			sb.Append(HtmlTabTag(true, ++tabNum, "asp:LinkButton", tagDictionary));
			sb.Append(HtmlTabTag(false, tabNum, "asp:LinkButton"));

			sb.Append(HtmlTabTag(false, --tabNum, "ItemTemplate"));
			sb.Append(HtmlTabTag(false, --tabNum, "asp:TemplateField"));

			return sb.ToString();
		}

		private bool WriteListAspxCodeBehind(Thing thing, string webFolderPath)
		{
			//build string
			StringBuilder sb = new StringBuilder();

			sb.Append(Using(thing.Name + ".DAL;"));
			sb.Append(Using(thing.Name + ".Models;"));
			sb.Append(Using("System;"));
			sb.Append(Using("System.Collections.Generic;"));
			sb.Append(Using("System.Linq;"));
			sb.Append(Using("System.Web;"));
			sb.Append(Using("System.Web.UI;"));
			sb.Append(Using("System.Web.UI.WebControls;"));
			sb.Append(Carriage());

			sb.Append(Namespace(true, thing.Name));
			sb.Append(PartialClass(true, thing.Name + "List", "System.Web.UI.Page"));

			string regionString = "Control Events";
			sb.Append(Region(true, regionString));

			Dictionary<string, string> methodParams = new Dictionary<string, string>();
			methodParams.Add("object", "sender");
			methodParams.Add("EventArgs", "e");
			sb.Append(Method(true, 2, "protected", "void", "Page_Load", methodParams));

			sb.Append(NewLineTab(3));
			sb.Append("if (!IsPostBack)");
			sb.Append(NewLineTab(3));
			sb.Append("{");
			sb.Append(NewLineTab(4));
			sb.Append("BindGrid();");
			sb.Append(NewLineTab(3));
			sb.Append("}");

			sb.Append(Method(false, 2));

			sb.Append(Region(false, regionString));

			regionString = "Private Methods";
			sb.Append(Region(true, regionString));
			sb.Append(Method(true, 2, "private", "void", "BindGrid"));
			sb.Append(TextValue(thing.Name + "DataAccess TDA = new " + thing.Name + "DataAccess();", 3));
			sb.Append(TextValue("gv" + thing.Name + ".DataSource = TDA.GetRecipeList();", 3));
			sb.Append(TextValue("gv" + thing.Name + ".DataBind();", 3));
			sb.Append(Method(false, 2));
			sb.Append(Region(false, regionString));

			sb.Append(PartialClass(false));
			sb.Append(Namespace(false));

			string thingWebPath = _fileUtil.WriteFile(sb.ToString(), webFolderPath + "\\" + thing.Name + "List.aspx.cs");

			return !String.IsNullOrEmpty(thingWebPath);

		}

		private bool WriteCreateAspx(Thing thing, string webFolderPath)
		{
			StringBuilder sb = new StringBuilder();
			int tabNum = 0;
			sb.Append(PageDirective(thing.Name, "Create"));
			sb.Append(Doctype(tabNum));
			sb.Append(HtmlTag(true, tabNum, "xmlns = \"http://www.w3.org/1999/xhtml\""));
			sb.Append(HeadTag(true, ++tabNum, "runat = \"server\""));
			sb.Append(TitleTag(true, ++tabNum));
			sb.Append(TextValue(thing.Name + " Create", ++tabNum));
			sb.Append(TitleTag(false, --tabNum));
			sb.Append(HeadTag(false, --tabNum));
			sb.Append(BodyTag(true, tabNum));
			//sb.Append(GridViewTag(true, "gv" + thing.Name, ++tabNum));
			sb.Append(TextValue(thing.Name, ++tabNum));
			sb.Append(BodyTag(false, --tabNum));

			sb.Append(HtmlTag(false, --tabNum));

			string thingWebPath = _fileUtil.WriteFile(sb.ToString(), webFolderPath + "\\" + thing.Name + "Create.aspx");

			return !String.IsNullOrEmpty(thingWebPath);
		}

		private bool WriteCreateAspxCodeBehind(Thing thing, string webFolderPath)
		{
			//build string
			StringBuilder sb = new StringBuilder();

			sb.Append(Using(thing.Name + ".DAL;"));
			sb.Append(Using(thing.Name + ".Models;"));
			sb.Append(Using("System;"));
			sb.Append(Using("System.Collections.Generic;"));
			sb.Append(Using("System.Linq;"));
			sb.Append(Using("System.Web;"));
			sb.Append(Using("System.Web.UI;"));
			sb.Append(Using("System.Web.UI.WebControls;"));
			sb.Append(Carriage());

			sb.Append(Namespace(true, thing.Name));
			sb.Append(PartialClass(true, thing.Name + "Create", "System.Web.UI.Page"));

			string regionString = "Control Events";
			sb.Append(Region(true, regionString));

			Dictionary<string, string> methodParams = new Dictionary<string, string>();
			methodParams.Add("object", "sender");
			methodParams.Add("EventArgs", "e");
			sb.Append(Method(true, 2, "protected", "void", "Page_Load", methodParams));

			sb.Append(NewLineTab(3));
			sb.Append("if (!IsPostBack)");
			sb.Append(NewLineTab(3));
			sb.Append("{");
			sb.Append(NewLineTab(4));
			//sb.Append("BindGrid();");
			sb.Append(NewLineTab(3));
			sb.Append("}");

			sb.Append(Method(false, 2));

			sb.Append(Region(false, regionString));

			regionString = "Private Methods";
			sb.Append(Region(true, regionString));
			//sb.Append(Method(true, 2, "private", "void", "BindGrid"));
			//sb.Append(TextValue(thing.Name + "DataAccess TDA = new " + thing.Name + "DataAccess();", 3));
			//sb.Append(TextValue("gv" + thing.Name + ".DataSource = TDA.GetRecipeList();", 3));
			//sb.Append(TextValue("gv" + thing.Name + ".DataBind();", 3));
			//sb.Append(Method(false, 2));
			sb.Append(Region(false, regionString));

			sb.Append(PartialClass(false));
			sb.Append(Namespace(false));

			string thingWebPath = _fileUtil.WriteFile(sb.ToString(), webFolderPath + "\\" + thing.Name + "Create.aspx.cs");

			return !String.IsNullOrEmpty(thingWebPath);
		}

		private bool WriteEditAspx(Thing thing, string webFolderPath)
		{
			StringBuilder sb = new StringBuilder();
			int tabNum = 0;
			sb.Append(PageDirective(thing.Name, "Edit"));
			sb.Append(Doctype(tabNum));
			sb.Append(HtmlTag(true, tabNum, "xmlns = \"http://www.w3.org/1999/xhtml\""));
			sb.Append(HeadTag(true, ++tabNum, "runat = \"server\""));
			sb.Append(TitleTag(true, ++tabNum));
			sb.Append(TextValue(thing.Name + " Edit", ++tabNum));
			sb.Append(TitleTag(false, --tabNum));
			sb.Append(HeadTag(false, --tabNum));
			sb.Append(BodyTag(true, tabNum));
			//sb.Append(GridViewTag(true, "gv" + thing.Name, ++tabNum));
			sb.Append(TextValue(thing.Name, ++tabNum));

			sb.Append(BodyTag(false, --tabNum));

			sb.Append(HtmlTag(false, --tabNum));

			string thingWebPath = _fileUtil.WriteFile(sb.ToString(), webFolderPath + "\\" + thing.Name + "Edit.aspx");

			return !String.IsNullOrEmpty(thingWebPath);
		}

		private bool WriteEditAspxCodeBehind(Thing thing, string webFolderPath)
		{
			//build string
			StringBuilder sb = new StringBuilder();

			sb.Append(Using(thing.Name + ".DAL;"));
			sb.Append(Using(thing.Name + ".Models;"));
			sb.Append(Using("System;"));
			sb.Append(Using("System.Collections.Generic;"));
			sb.Append(Using("System.Linq;"));
			sb.Append(Using("System.Web;"));
			sb.Append(Using("System.Web.UI;"));
			sb.Append(Using("System.Web.UI.WebControls;"));
			sb.Append(Carriage());

			sb.Append(Namespace(true, thing.Name));
			sb.Append(PartialClass(true, thing.Name + "Edit", "System.Web.UI.Page"));

			string regionString = "Control Events";
			sb.Append(Region(true, regionString));

			Dictionary<string, string> methodParams = new Dictionary<string, string>();
			methodParams.Add("object", "sender");
			methodParams.Add("EventArgs", "e");
			sb.Append(Method(true, 2, "protected", "void", "Page_Load", methodParams));

			sb.Append(NewLineTab(3));
			sb.Append("if (!IsPostBack)");
			sb.Append(NewLineTab(3));
			sb.Append("{");
			sb.Append(NewLineTab(4));
			//sb.Append("BindGrid();");
			sb.Append(NewLineTab(3));
			sb.Append("}");

			sb.Append(Method(false, 2));

			sb.Append(Region(false, regionString));

			regionString = "Private Methods";
			sb.Append(Region(true, regionString));
			//sb.Append(Method(true, 2, "private", "void", "BindGrid"));
			//sb.Append(TextValue(thing.Name + "DataAccess TDA = new " + thing.Name + "DataAccess();", 3));
			//sb.Append(TextValue("gv" + thing.Name + ".DataSource = TDA.GetRecipeList();", 3));
			//sb.Append(TextValue("gv" + thing.Name + ".DataBind();", 3));
			//sb.Append(Method(false, 2));
			sb.Append(Region(false, regionString));

			sb.Append(PartialClass(false));
			sb.Append(Namespace(false));

			string thingWebPath = _fileUtil.WriteFile(sb.ToString(), webFolderPath + "\\" + thing.Name + "Edit.aspx.cs");

			return !String.IsNullOrEmpty(thingWebPath);
		}		

		private string PageDirective(string name, string crudType)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<%@ Page Language=\"C#\" AutoEventWireup=\"true\"");
			sb.Append("CodeBehind =\"" + name + crudType + ".aspx.cs\" Inherits=\"" + name + "." + name + crudType + "\" %>");
			return sb.ToString();
		}

		private string Doctype(int tabNum)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(NewLineTab(tabNum));
			sb.Append("< !DOCTYPE html >");
			return sb.ToString();
		}

		private string HtmlTag(bool openTag, int tabNum, string attribute = null)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(NewLineTab(tabNum));
			if (!openTag)
			{
				sb.Append("</ html >");
			}
			else
			{
				sb.Append("< html ");
				sb.Append(attribute);
				sb.Append(" >");
			}

			return sb.ToString();
		}

		private string HeadTag(bool openTag, int tabNum, string attribute = null)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(NewLineTab(tabNum));
			if (!openTag)
			{
				sb.Append("</ head >");
			}
			else
			{
				sb.Append("< head ");
				sb.Append(attribute);
				sb.Append(" >");
			}

			return sb.ToString();
		}

		private string TitleTag(bool openTag, int tabNum, string attribute = null)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(NewLineTab(tabNum));
			if (!openTag)
			{
				sb.Append("</ title >");
			}
			else
			{
				sb.Append("< title ");
				sb.Append(attribute);
				sb.Append(" >");
			}

			return sb.ToString();
		}

		private string BodyTag(bool openTag, int tabNum)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(NewLineTab(tabNum));
			if (!openTag)
			{
				sb.Append("</ body >");
			}
			else
			{
				sb.Append("< body > ");
			}

			return sb.ToString();
		}

		private string GridViewTag(bool openTag, int tabNum, string name = null )
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(NewLineTab(tabNum));
			if (openTag)
			{
				sb.Append("< asp:GridView ID=\"" + name + "\" runat=\"server\" DataKeyNames=\"Id\"");

				sb.Append(">");
			}
			else
			{
				sb.Append("</ asp:GridView >");				
			}

			return sb.ToString();
		}

		private string TextValue(string textValue, int tabNum)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(NewLineTab(tabNum));
			sb.Append(textValue);
			return sb.ToString();
		}

		

		private string Method(bool openMethod, int tabNum, string visibility = null, string returnType = null, 
				string name = null, Dictionary<string, string> methodParams= null)
		{
			StringBuilder sb = new StringBuilder();
			if (openMethod)
			{
				sb.Append(NewLineTab(0));
				sb.Append(NewLineTab(tabNum));

				sb.Append(visibility + " " + returnType + " " + name +"(");

				if(methodParams != null)
				{
					string joined = string.Join(",", methodParams.Select(x => x.Key + " " + x.Value).ToArray());
					sb.Append(joined);
				}				
				sb.Append(")");
				sb.Append(NewLineTab(tabNum));
				sb.Append("{");
			}
			else
			{
				sb.Append(NewLineTab(tabNum));
				sb.Append("} ");
			}

			return sb.ToString();
		}

		private string PageLoad(bool openBlock, int tabNum)
		{
			StringBuilder sb = new StringBuilder();

			if (openBlock)
			{
				sb.Append(NewLineTab(tabNum));
				sb.Append("protected void Page_Load(object sender, EventArgs e)");
				sb.Append(NewLineTab(tabNum));
				sb.Append("{");
				sb.Append(NewLineTab(++tabNum));
				sb.Append("if (!IsPostBack)");
				sb.Append(NewLineTab(tabNum));
				sb.Append("{");
				sb.Append(NewLineTab(++tabNum));
				sb.Append("BindGrid();");
				sb.Append(NewLineTab(--tabNum));
				sb.Append("}");
				sb.Append(NewLineTab(tabNum));
				sb.Append("}");
			}
			else
			{
				sb.Append(NewLineTab(tabNum));
				sb.Append("}");
			}
			
			return sb.ToString();
		}

		private string Region(bool openRegion, string name)
		{			
			StringBuilder sb = new StringBuilder();
			sb.Append(NewLineTab(0));
			if (openRegion)
			{
				sb.Append(NewLineTab(0));
				sb.Append("#region ");
				sb.Append(name);				
			}
			else
			{
				sb.Append(NewLineTab(0));
				sb.Append("#endregion ");
				sb.Append(name);
			}

			return sb.ToString();
		}

		private string PartialClass(bool openBlock, string className = null, string extendsClass = null)
		{
			StringBuilder sb = new StringBuilder();
			if (openBlock)
			{
				sb.Append(NewLineTab(1));
				sb.Append("public partial class ");
				sb.Append(className);
				sb.Append(": " + extendsClass);
				sb.Append(NewLineTab(1));
				sb.Append("{");				
			}
			else
			{
				sb.Append(NewLineTab(1));
				sb.Append("}");
			}
			
			return sb.ToString();
		}

		private string Using(string assembly)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("using ");
			sb.Append(assembly);
			sb.Append(NewLineTab(0));
			return sb.ToString();
		}

		private string Namespace(bool openBlock, string namespc = null)
		{
			StringBuilder sb = new StringBuilder();
			if (!openBlock)
			{
				sb.Append(NewLineTab(0));
				sb.Append("}");
			}
			else
			{
				sb.Append("namespace ");
				sb.Append(namespc);
				sb.Append(NewLineTab(0));
				sb.Append("{");
			}			
			return sb.ToString();
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

		private string HtmlBr()
		{
			return "</br>";
		}

		private string HtmlTabTag(bool openTag, int tabNum, string tagName = null, Dictionary<string, string> attributes = null)
		{			
			StringBuilder sb = new StringBuilder();
			
			if (openTag)
			{
				sb.Append(NewLine());
				for (int i = 0; i < tabNum; i++)
				{
					sb.Append("\t");
				}
				sb.Append("<" + tagName );

				if (attributes != null && attributes.Count > 0)
				{
					foreach (KeyValuePair<string, string> entry in attributes)
					{
						// do something with entry.Value or entry.Key
						sb.Append(" " + entry.Key + "=\"" + entry.Value + "\"");
					}
				}

				sb.Append(" > ");
			}
			else
			{
				if (tagName != null)
				{
					sb.Append(NewLine());
					for (int i = 0; i < tabNum; i++)
					{
						sb.Append("\t");
					}
					sb.Append("</" + tagName + ">");
				}
				else
				{
					sb.Append(" />");
				}				
			}			

			return sb.ToString();
		}

	}
}

