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
			bool exists = System.IO.Directory.Exists(mapPath);
			if (!exists)
			{
				System.IO.Directory.CreateDirectory(mapPath);
			}

			return mapPath;
		}

		public bool FileExists(string filePath)
		{
			return System.IO.File.Exists(filePath);			
		}
	}
}