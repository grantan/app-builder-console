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
		private Thing _thing;

		public WebUtility(Thing thing)
		{
			_tda = new ThingDataAccess();
			_tpda = new ThingPropertyDataAccess();
			_fileUtil = new FileIOUtility();
			_thing = thing;
		}

		public string WriteThingModels(List<Thing> fullThingList, string projectPath)
		{
			//Write the Domain model folder (delete if it already exists)
			string modelsFolderPath = _fileUtil.WriteFolder(projectPath + "\\Models");
			foreach (Thing projectThing in fullThingList)
			{
				projectThing.PropertyList = _tpda.GetThingProperties(projectThing.Id);
				WriteThingModelCSharp(projectThing, modelsFolderPath);
			}

			return modelsFolderPath;
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
					sb.Append(WriteCodeUsingStatements());
					sb.Append(Namespace(true, _thing.Name, "Models"));
					sb.Append("\r\n");
					sb.Append(Tab(1));
					sb.Append("public class " + mainThingName);
					sb.Append("\r\n");
					sb.Append(Tab(1));
					sb.Append("{ ");
					sb.Append("\r\n");
					//sb.Append(Tab(1));
					sb.Append(WriteThing(fullThing));

					sb.Append("\r\n");
					sb.Append(Tab(1));
					sb.Append("}");
					sb.Append(Namespace(false));

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
					sb.Append(WriteCodeUsingStatements());
					sb.Append(Namespace(true, _thing.Name, "Models"));
					sb.Append("\r\n");
					sb.Append(Tab(1));
					sb.Append("public class " + mainThingName + " : " + parentThingName);
					sb.Append("\r\n");
					sb.Append(Tab(1));
					sb.Append("{ ");
					sb.Append("\r\n");
					sb.Append(WriteThing(fullThing));

					sb.Append("\r\n");
					sb.Append(Tab(1));
					sb.Append("}");
					sb.Append(Namespace(false));
					_fileUtil.WriteFile(sb.ToString(), filePath);


					foreach (ThingProperty prop in fullThing.PropertyList)
					{
						Thing propThing = _tda.GetThingByID(prop.OwnedThing.Id);
						propThing.PropertyList = _tpda.GetThingProperties(propThing.Id);
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
				sb.Append(Tab(2));
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

		public string WriteThingWebFormAspx(string mapPath)
		{
			string mainRepoProjectPath = mapPath + "\\" + _thing.Name;
			string mainCodeProjectPath = mainRepoProjectPath + "\\" + _thing.Name;

			//Write the Domain model folder (delete if it already exists)
			string webFolderPath = _fileUtil.WriteFolder(mainCodeProjectPath + "\\Web");

			bool success = WriteListAspx(webFolderPath);
			success = WriteListAspxCodeBehind(webFolderPath);

			success = WriteCreateAspx(webFolderPath);
			success = WriteCreateAspxCodeBehind(webFolderPath);

			success = WriteEditAspx(webFolderPath);
			success = WriteEditAspxCodeBehind(webFolderPath);

			return mapPath;
		}		

		private bool WriteListAspx(string webFolderPath)
		{			
			StringBuilder sb = new StringBuilder();
			int tabNum = 0;
			sb.Append(PageDirective(_thing.Name, "List"));
			sb.Append(Doctype(tabNum));

			//sb.Append(HtmlTag(true, tabNum, "xmlns = \"http://www.w3.org/1999/xhtml\""));
			var tagDictionary = new Dictionary<string, string>();
			tagDictionary.Add("xmlns", "http://www.w3.org/1999/xhtml");
			sb.Append(HtmlTabTag(true, tabNum, "html", tagDictionary));

			tagDictionary = new Dictionary<string, string>();
			tagDictionary.Add("runat", "server");
			sb.Append(HtmlTabTag(true, ++tabNum, "head", tagDictionary));
			//sb.Append(HeadTag(true, ++tabNum, "runat = \"server\""));

			
			//headDictionary.Add("runat", "\"server\"");
			sb.Append(HtmlTabTag(true, ++tabNum, "title"));
			//sb.Append(TitleTag(true, ++tabNum));
			sb.Append(TextValue(_thing.Name + " List", ++tabNum));

			sb.Append(HtmlTabTag(false, --tabNum, "title")); //title
															 

			sb.Append(HtmlTabTag(false, --tabNum, "head"));
			//sb.Append(HeadTag(false, --tabNum));

			sb.Append(HtmlTabTag(true, tabNum, "body"));

			tagDictionary = new Dictionary<string, string>();
			tagDictionary.Add("runat", "server");
			tagDictionary.Add("id", "form1");
			sb.Append(HtmlTabTag(true, ++tabNum, "form", tagDictionary));
			//sb.Append(BodyTag(true, tabNum));
			sb.Append(HtmlTabTag(true, ++tabNum, "div"));

			List<string> columnList = new List<string>();   // = GetThingPropertyColumns()
			columnList.Add("Id");
			columnList.Add("Name");
			columnList.Add("Description");

			sb.Append(AspGridView(tabNum, columnList));			

			sb.Append(HtmlTabTag(false, tabNum, "div"));

			sb.Append(HtmlTabTag(true, tabNum, "div"));

			tagDictionary = new Dictionary<string, string>();
			tagDictionary.Add("id", "btnAdd" + _thing.Name);
			tagDictionary.Add("runat", "server");
			tagDictionary.Add("Text", "Add " + _thing.Name);
			tagDictionary.Add("OnClick", "btnAdd" + _thing.Name + "_Click");
			sb.Append(HtmlTabTagSelfClose(++tabNum, "asp:Button", tagDictionary));
			
			sb.Append(HtmlTabTag(false, --tabNum, "div"));

			sb.Append(HtmlTabTag(false, --tabNum, "form"));

			sb.Append(HtmlTabTag(false, --tabNum, "body"));
			
			sb.Append(HtmlTabTag(false, --tabNum, "html"));		
			
			string thingWebPath = _fileUtil.WriteFile(sb.ToString(), webFolderPath + "\\" + _thing.Name + "List.aspx");

			return !String.IsNullOrEmpty(thingWebPath);
		}

		private string AspGridView(int tabNum, List<string> columnList)
		{
			StringBuilder sb = new StringBuilder();
			Dictionary<string, string> tagDictionary = new Dictionary<string, string>();
			tagDictionary.Add("ID", "gv" + _thing.Name);
			tagDictionary.Add("runat", "server");
			tagDictionary.Add("autogeneratecolumns", "false");
			tagDictionary.Add("emptydatatext", "No data in the data source. Click the Add button to add a record.");
			sb.Append(HtmlTabTag(true, ++tabNum, "asp:GridView", tagDictionary));

			sb.Append(HtmlTabTag(true, ++tabNum, "Columns"));

			foreach (string col in columnList)
			{
				sb.Append(AspTemplateFieldLabel(col, tabNum));
			}
			
			sb.Append(AspTemplateFieldLinkButton("Select", tabNum));

			sb.Append(HtmlTabTag(false, tabNum, "Columns"));
			sb.Append(HtmlTabTag(false, --tabNum, "asp:GridView"));

			return sb.ToString();
		}

		private bool WriteListAspxCodeBehind(string webFolderPath)
		{
			//build string
			StringBuilder sb = new StringBuilder();

			sb.Append(WriteWebUsingStatements());		

			sb.Append(Namespace(true, _thing.Name, "Web"));
			sb.Append(PartialClass(true, _thing.Name + "List", "System.Web.UI.Page"));

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

			methodParams = new Dictionary<string, string>();
			methodParams.Add("object", "sender");
			methodParams.Add("EventArgs", "e");
			sb.Append(Method(true, 2, "protected", "void", "gv" + _thing.Name + "_SelectedIndexChanged", methodParams));

			sb.Append(NewLineTab(3));
			sb.Append("//Determine the RowIndex of the Row whose Button was clicked.");
			sb.Append(NewLineTab(3));
			sb.Append("int rowIndex = ((sender as LinkButton).NamingContainer as GridViewRow).RowIndex;");
			sb.Append(NewLineTab(3));
			sb.Append("//Get the value of column from the DataKeys using the RowIndex.");
			sb.Append(NewLineTab(3));
			sb.Append("int id = Convert.ToInt32(gv" + _thing.Name + ".DataKeys[rowIndex].Values[0]);");
			sb.Append(NewLineTab(3));
			sb.Append("Page.Response.Redirect(\"Edit"+ _thing.Name + ".aspx?id=\" + id); ");
			
			sb.Append(Method(false, 2));


			sb.Append(Region(false, regionString));

			regionString = "Private Methods";
			sb.Append(Region(true, regionString));
			sb.Append(Method(true, 2, "private", "void", "BindGrid"));
			sb.Append(TextValue(_thing.Name + "DataAccess TDA = new " + _thing.Name + "DataAccess();", 3));
			sb.Append(TextValue("gv" + _thing.Name + ".DataSource = TDA.Get"+ _thing.Name + "List();", 3));
			sb.Append(TextValue("gv" + _thing.Name + ".DataBind();", 3));
			sb.Append(Method(false, 2));
			sb.Append(Region(false, regionString));

			sb.Append(PartialClass(false));
			sb.Append(Namespace(false));

			string thingWebPath = _fileUtil.WriteFile(sb.ToString(), webFolderPath + "\\" + _thing.Name + "List.aspx.cs");

			return !String.IsNullOrEmpty(thingWebPath);

		}

		private string WriteCodeUsingStatements()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(Using("System;"));
			sb.Append(Using("System.Collections.Generic;"));
			sb.Append(Using("System.Linq;"));
			sb.Append(Using("System.Web;"));
			
			sb.Append(Carriage());
			return sb.ToString();
		}

		private string WriteWebUsingStatements()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(Using(_thing.Name + ".DAL;"));
			sb.Append(Using(_thing.Name + ".Models;"));
			sb.Append(Using("System;"));
			sb.Append(Using("System.Collections.Generic;"));
			sb.Append(Using("System.Linq;"));
			sb.Append(Using("System.Web;"));
			sb.Append(Using("System.Web.UI;"));
			sb.Append(Using("System.Web.UI.WebControls;"));
			sb.Append(Carriage());
			return sb.ToString();
		}

		private bool WriteCreateAspx(string webFolderPath)
		{
			StringBuilder sb = new StringBuilder();
			int tabNum = 0;
			sb.Append(PageDirective(_thing.Name, "Create"));
			sb.Append(Doctype(tabNum));
			var tagDictionary = new Dictionary<string, string>();
			tagDictionary.Add("xmlns", "\"http://www.w3.org/1999/xhtml\"");
			sb.Append(HtmlTabTag(true, tabNum, "html", tagDictionary));
			//sb.Append(HtmlTag(true, tabNum, "xmlns = \"http://www.w3.org/1999/xhtml\""));
			tagDictionary = new Dictionary<string, string>();
			tagDictionary.Add("runat", "server");
			sb.Append(HtmlTabTag(true, ++tabNum, "head", tagDictionary));
			//sb.Append(HeadTag(true, ++tabNum, "runat = \"server\""));
			sb.Append(TitleTag(true, ++tabNum));
			sb.Append(TextValue(_thing.Name + " Create", ++tabNum));
			sb.Append(TitleTag(false, --tabNum));
			sb.Append(HeadTag(false, --tabNum));
			sb.Append(BodyTag(true, tabNum));
			//sb.Append(GridViewTag(true, "gv" + thing.Name, ++tabNum));
			sb.Append(TextValue(_thing.Name, ++tabNum));
			sb.Append(BodyTag(false, --tabNum));

			sb.Append(HtmlTag(false, --tabNum));

			string thingWebPath = _fileUtil.WriteFile(sb.ToString(), webFolderPath + "\\" + _thing.Name + "Create.aspx");

			return !String.IsNullOrEmpty(thingWebPath);
		}

		private bool WriteCreateAspxCodeBehind(string webFolderPath)
		{
			//build string
			StringBuilder sb = new StringBuilder();

			sb.Append(WriteWebUsingStatements());

			sb.Append(Namespace(true, _thing.Name, "Web"));
			sb.Append(PartialClass(true, _thing.Name + "Create", "System.Web.UI.Page"));

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

			string thingWebPath = _fileUtil.WriteFile(sb.ToString(), webFolderPath + "\\" + _thing.Name + "Create.aspx.cs");

			return !String.IsNullOrEmpty(thingWebPath);
		}

		private bool WriteEditAspx(string webFolderPath)
		{
			StringBuilder sb = new StringBuilder();
			int tabNum = 0;
			sb.Append(PageDirective(_thing.Name, "Edit"));
			sb.Append(Doctype(tabNum));
			sb.Append(HtmlTag(true, tabNum, "xmlns = \"http://www.w3.org/1999/xhtml\""));
			sb.Append(HeadTag(true, ++tabNum, "runat = \"server\""));
			sb.Append(TitleTag(true, ++tabNum));
			sb.Append(TextValue(_thing.Name + " Edit", ++tabNum));
			sb.Append(TitleTag(false, --tabNum));
			sb.Append(HeadTag(false, --tabNum));
			sb.Append(BodyTag(true, tabNum));
			//sb.Append(GridViewTag(true, "gv" + thing.Name, ++tabNum));
			sb.Append(TextValue(_thing.Name, ++tabNum));

			sb.Append(BodyTag(false, --tabNum));

			sb.Append(HtmlTag(false, --tabNum));

			string thingWebPath = _fileUtil.WriteFile(sb.ToString(), webFolderPath + "\\" + _thing.Name + "Edit.aspx");

			return !String.IsNullOrEmpty(thingWebPath);
		}

		private bool WriteEditAspxCodeBehind(string webFolderPath)
		{
			//build string
			StringBuilder sb = new StringBuilder();

			sb.Append(WriteWebUsingStatements());

			sb.Append(Namespace(true, _thing.Name, "Web"));
			sb.Append(PartialClass(true, _thing.Name + "Edit", "System.Web.UI.Page"));

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

			string thingWebPath = _fileUtil.WriteFile(sb.ToString(), webFolderPath + "\\" + _thing.Name + "Edit.aspx.cs");

			return !String.IsNullOrEmpty(thingWebPath);
		}		

		private string PageDirective(string name, string crudType)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<%@ Page Language=\"C#\" AutoEventWireup=\"true\" ");
			sb.Append("CodeBehind=\"" + name + crudType + ".aspx.cs\" Inherits=\"" + name + ".Web." + name + crudType + "\" %>");
			return sb.ToString();
		}

		private string Doctype(int tabNum)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(NewLineTab(tabNum));
			sb.Append("<!DOCTYPE html>");
			return sb.ToString();
		}

		private string HtmlTag(bool openTag, int tabNum, Dictionary<string, string> attributes)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(NewLineTab(tabNum));
			if (!openTag)
			{
				sb.Append("</html>");
			}
			else
			{
				if (attributes != null && attributes.Count > 0)
				{
					sb.Append("<html ");
					foreach (KeyValuePair<string, string> entry in attributes)
					{
						// do something with entry.Value or entry.Key
						sb.Append(entry.Key + " =\"" + entry.Value + "\" ");
					}
				}
				else
				{
					sb.Append("<html");
				}

				sb.Append(">");
			}

			return sb.ToString();
		}

		private string HtmlTag(bool openTag, int tabNum, string attribute = null)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(NewLineTab(tabNum));
			if (!openTag)
			{
				sb.Append("</html>");
			}
			else
			{
				if (attribute != null)
				{
					sb.Append("<html ");
					sb.Append(attribute);
				}
				else
				{
					sb.Append("<html");
				}
				
				sb.Append(">");
			}

			return sb.ToString();
		}

		private string HeadTag(bool openTag, int tabNum, string attribute = null)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(NewLineTab(tabNum));
			if (!openTag)
			{
				sb.Append("</head>");
			}
			else
			{
				if (attribute != null)
				{
					sb.Append("<head ");
					sb.Append(attribute);
				}
				else
				{
					sb.Append("<head");
				}
				
				sb.Append(">");
			}

			return sb.ToString();
		}

		private string TitleTag(bool openTag, int tabNum, string attribute = null)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(NewLineTab(tabNum));
			if (!openTag)
			{
				sb.Append("</title>");
			}
			else
			{
				if (attribute != null)
				{
					sb.Append("<title ");
					sb.Append(attribute);
				}
				else
				{
					sb.Append("<title");
				}
					
				sb.Append(">");
			}

			return sb.ToString();
		}

		private string BodyTag(bool openTag, int tabNum)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(NewLineTab(tabNum));
			if (!openTag)
			{
				sb.Append("</body>");
			}
			else
			{
				sb.Append("<body> ");
			}

			return sb.ToString();
		}

		private string GridViewTag(bool openTag, int tabNum, string name = null )
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(NewLineTab(tabNum));
			if (openTag)
			{
				sb.Append("<asp:GridView ID=\"" + name + "\" runat=\"server\" DataKeyNames=\"Id\"");

				sb.Append(">");
			}
			else
			{
				sb.Append("</asp:GridView>");				
			}

			return sb.ToString();
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
			tagDictionary.Add("Text", "<%# Eval(\"" + propertyName + "\") %>");
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
			tagDictionary.Add("OnCommand", "gv" + _thing.Name + "_SelectedIndexChanged");
			tagDictionary.Add("CommandArgument", "<%# Eval(\"Id\") %>");
			tagDictionary.Add("CommandName", "Select");
			sb.Append(HtmlTabTagSelfClose(++tabNum, "asp:LinkButton", tagDictionary));
			
			sb.Append(HtmlTabTag(false, --tabNum, "ItemTemplate"));
			sb.Append(HtmlTabTag(false, --tabNum, "asp:TemplateField"));

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

		private string Namespace(bool openBlock, string namespc = null, string childDirectory = null)
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
				sb.Append(namespc + "." + childDirectory);
				//sb.Append(namespc + ".Web");
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
						if(entry.Value.Contains("%# Eval"))
						{
							sb.Append(" " + entry.Key + "=\'" + entry.Value + "\'");
						}
						else
						{
							sb.Append(" " + entry.Key + "=\"" + entry.Value + "\"");
						}
						
					}
				}

				sb.Append(">");
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

		private string HtmlTabTagSelfClose(int tabNum, string tagName = null, Dictionary<string, string> attributes = null)
		{
			StringBuilder sb = new StringBuilder();
						
			sb.Append(NewLine());
			for (int i = 0; i < tabNum; i++)
			{
				sb.Append("\t");
			}
			sb.Append("<" + tagName);

			if (attributes != null && attributes.Count > 0)
			{
				foreach (KeyValuePair<string, string> entry in attributes)
				{
					// do something with entry.Value or entry.Key
					sb.Append(" " + entry.Key + "=\"" + entry.Value + "\"");
				}
			}

			sb.Append(" /> ");		

			return sb.ToString();
		}

	}
}

