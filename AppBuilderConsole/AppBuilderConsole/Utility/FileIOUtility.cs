using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace AppBuilderConsole.Utility
{
	public class FileIOUtility
	{
		public string WriteFile(string txt, string mapPath)
		{
			//WriteFolderIfNotExists(mapPath);

			using (StreamWriter _testData = new StreamWriter(mapPath, false))
			{
				_testData.WriteLine(txt); // Write the file.
			}

			return mapPath;
		}

		public string WriteFolderIfNotExists(string mapPath)
		{
			if (!System.IO.Directory.Exists(mapPath))
			{
				System.IO.Directory.CreateDirectory(mapPath);
			}

			return mapPath;
		}

		public string WriteFolder(string mapPath)
		{
			if (System.IO.Directory.Exists(mapPath))
			{
				ClearFolder(mapPath);
			}

			System.IO.Directory.CreateDirectory(mapPath);

			return mapPath;
		}

		private void ClearFolder(string mapPath)
		{
			DirectoryInfo dir = new DirectoryInfo(mapPath);

			foreach (FileInfo fi in dir.GetFiles())
			{
				fi.Delete();
			}

			foreach (DirectoryInfo di in dir.GetDirectories())
			{
				ClearFolder(di.FullName);
				di.Delete();
			}
		}


		public bool FileExists(string filePath)
		{
			return System.IO.File.Exists(filePath);			
		}
	}
}