/*
 * Created by SharpDevelop.
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace RobotFramework
{
	/// <summary>
	/// RobotFramework example of .NET Implementation of a remote library.
	/// Based on the Python example at http://robotframework.googlecode.com/svn/trunk/tools/remoteserver/example/examplelibrary.py
	/// Compile into an assembly DLL and pass filename and "RobotFramework.ExampleRemoteLibrary"
	/// as arguments to the .NET implementation of remoteserver
	/// to start the server with this library.
	/// </summary>
	public class ExampleRemoteLibrary
	{
		public void strings_should_be_equal(string str1, string str2)
		{
			Console.WriteLine("Comparing '{0}' to '{1}'",str1,str2);
			if(str1 != str2) throw new Exception("Given strings are not equal");			
		}
		
		public int count_items_in_directory(string path)
		{
			string[] subDirs, dirFiles;
			//get list of sub directories & files under given directory path
			subDirs = Directory.GetDirectories(path);
			dirFiles = Directory.GetFiles(path);
			//# items = total from both lists
			return subDirs.Length + dirFiles.Length;
		}
		
	}
}