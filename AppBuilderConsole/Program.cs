﻿using AppBuilderConsole.DAL;
using AppBuilderConsole.Models;
using AppBuilderConsole.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBuilderConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			//DisplayWelcome();
			//DisplayConnString();
			int thingId = 2;
			//bool success = WriteThingProject(thingId);
			//DisplayStatus(success);
			bool success = WriteThingModels(thingId);
			DisplayStatus(success);
			success = WriteThingWeb(thingId);
			DisplayStatus(success);
			
		}

		private static bool WriteThingProject(int thingId)
		{
			string path = ConfigurationManager.AppSettings["WriteFilePath"].ToString();
			//int thingId = GetThingId();
			//ObjectGraphUtility util = new ObjectGraphUtility();
			ThingDataAccess TDA = new ThingDataAccess();
			List<Thing> fullThingList = TDA.GetFullThingHierarchy(thingId);

			ObjectGraphUtility util = new ObjectGraphUtility();
			string mapPath = util.WriteThingProjectModel(fullThingList, path);
			return !(String.IsNullOrEmpty(mapPath));

			//ObjectGraphUtility utility = new ObjectGraphUtility();
			//return serverMapPath;
		}

		private static bool WriteThingModels(int thingId)
		{
			ThingDataAccess TDA = new ThingDataAccess();
			List<Thing> fullThingList = TDA.GetFullThingHierarchy(thingId);
			//Thing thing = TDA.GetThingByID(thingId);
			Thing fullThing = fullThingList.FirstOrDefault();

			string path = ConfigurationManager.AppSettings["WriteFilePath"].ToString();

			string mainRepoProjectPath = path + "\\" + fullThing.Name;
			string mainCodeProjectPath = mainRepoProjectPath + "\\" + fullThing.Name;

			//Write the Domain model folder (delete if it already exists)
			//string modelsFolderPath = mainCodeProjectPath + "\\Models";
			//int thingId = GetThingId();
			//ObjectGraphUtility util = new ObjectGraphUtility();
			

			WebUtility util = new WebUtility(fullThing);
			string mapPath = util.WriteThingModels(fullThingList, mainCodeProjectPath);
			return !(String.IsNullOrEmpty(mapPath));
		}

		private static bool WriteThingWeb(int thingId)
		{
			string path = ConfigurationManager.AppSettings["WriteFilePath"].ToString();
			//int thingId = GetThingId();
			//ObjectGraphUtility util = new ObjectGraphUtility();
			ThingDataAccess TDA = new ThingDataAccess();
			Thing thing = TDA.GetThingByID(thingId);

			WebUtility util = new WebUtility(thing);
			string mapPath = util.WriteThingWebFormAspx(path);
			return !(String.IsNullOrEmpty(mapPath));

			//ObjectGraphUtility utility = new ObjectGraphUtility();
			//return serverMapPath;
		}

		private static void DisplayStatus(bool success)
		{
			//Console.WriteLine("\nWhat is your name? ");
			//var name = Console.ReadLine();
			//var date = DateTime.Now;
			Console.WriteLine($"\nStatus=, {success}!");
			Console.Write("\nPress any key to exit...");
			Console.ReadKey(true);
		}

		

		private string WriteThingProject()
		{
			int thingId = GetThingId();
			//ObjectGraphUtility util = new ObjectGraphUtility();
			ThingDataAccess TDA = new ThingDataAccess();
			List<Thing> fullThingList = TDA.GetFullThingHierarchy(thingId);

			ObjectGraphUtility util = new ObjectGraphUtility();
			string serverMapPath = util.WriteThingProjectModel(fullThingList, @"C:\CodeRepository");

			//ObjectGraphUtility utility = new ObjectGraphUtility();
			return serverMapPath;
		}

		private int GetThingId()
		{
			//return Int32.Parse(Page.Request.QueryString["thingId"]);
			return 0;
		}

		private static void DisplayConnString()
		{
			string connstring = ConfigurationManager.ConnectionStrings["AppBuilderConnectionString"].ToString();
			Console.WriteLine(connstring);
			Console.Write("\nPress any key to exit...");
			Console.ReadKey(true);
		}

		private static void DisplayWelcome()
		{
			Console.WriteLine("\nWhat is your name? ");
			var name = Console.ReadLine();
			var date = DateTime.Now;
			Console.WriteLine($"\nHello, {name}, on {date:d} at {date:t}!");
			Console.Write("\nPress any key to exit...");
			Console.ReadKey(true);
		}
	}
}
