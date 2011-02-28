/*
 * Copyright (c) 2011 David Luu
 * XML-RPC.NET Copyright (c) 2006 Charles Cook
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.IO;
using System.Net;		//used for XML-RPC server
using CookComputing.XmlRpc; //get from www.xml-rpc.net
using System.Reflection; //for the get_keyword methods and run_keyword method

namespace RobotFramework
{
	/// <summary>
	/// RobotFramework .NET implementation of generic remote library server.
	/// Based on RobotFramework spec at
	/// http://code.google.com/p/robotframework/wiki/RemoteLibrary
	/// http://robotframework.googlecode.com/svn/tags/robotframework-2.5.6/doc/userguide/RobotFrameworkUserGuide.html#remote-library-interface
	/// http://robotframework.googlecode.com/svn/tags/robotframework-2.5.6/doc/userguide/RobotFrameworkUserGuide.html#dynamic-library-api
	/// 
	/// Uses .NET reflection to serve the dynamically loaded remote library
	/// 
	/// Compile this into a console application, and then you can dynamically load a
	/// separately compiled .NET class (Robot Framework keyword) library to use at startup.
	/// </summary>
	class RemoteServer
	{
		public static void Main(string[] args)
		{
			//set default IP and port for RobotFramework XML-RPC server spec
			string host = "127.0.0.1"; //localhost
			string port = "8270";
			string remoteLibrary;
			string className;
			
			if(args.Length < 2)
			{
				displayUsage();
				System.Environment.Exit(0);				
			}
			remoteLibrary = args[0];
			className = args[1];			
			if(args.Length > 2) host = args[2];
			if(args.Length > 3) port = args[3];
			Console.WriteLine("");
			Console.WriteLine("Robot Framework remote library started at {0} on port {1}, on {2}",host,port,System.DateTime.Now.ToString());
			Console.WriteLine("");
			Console.WriteLine("Send XML-RPC method request 'stop_remote_server' to do so, or hit Ctrl + C, etc.");
			Console.WriteLine("");			
			
			HttpListener listener = new HttpListener();
			listener.Prefixes.Add("http://"+host+":"+port+"/");
			listener.Start();
			while (true)
			{
				HttpListenerContext context = listener.GetContext();
				XmlRpcListenerService svc = new XmlRpcMethods(remoteLibrary,className);
				svc.ProcessRequest(context);
			}
		}
		
		/// <summary>
		/// Display the usage information for robotremoteserver.
		/// </summary>
		public static void displayUsage()
		{
			Console.WriteLine("");
			Console.WriteLine("Usage Info:");
			Console.WriteLine("");
			Console.WriteLine("  robotremoteserver pathToLibraryAssemblyFile RemoteLibraryClassName");
			Console.WriteLine("    [address] [port]");
			Console.WriteLine("");
			Console.WriteLine("  Assembly file = DLL or EXE, etc. w/ keyword class methods to execute.");
			Console.WriteLine("  Class name = keyword class w/ methods to execute. Include namespace as needed.");
			Console.WriteLine("");
			Console.WriteLine("  Optionally specify IP address to bind remote server to.");
			Console.WriteLine("    Default of 127.0.0.1 (localhost).");
			Console.WriteLine("  Optionally specify port to bind remote server to. Default of 8270.");
			Console.WriteLine("");
			Console.WriteLine("Example:");
			Console.WriteLine("");
			Console.WriteLine("  robotremoteserver C:\\MyLibrary.dll MyNamespace.MyClass 192.168.0.10 8080");
			Console.WriteLine("");
			Console.WriteLine("");
		}
	}
	
	/// <summary>
	/// Class of XML-RPC methods for remote library (server)
	/// that conforms to RobotFramework remote library API
	/// </summary>
	public class XmlRpcMethods : XmlRpcListenerService
	{
		private Assembly library;
		private string libraryClass;
		
		/// <summary>
		/// Default constructure for XML-RPC method class
		/// </summary>
		public XmlRpcMethods()
		{
			library = null;
			libraryClass = null;
		}
		
		/// <summary>
		/// Ideal constructor for XML-RPC method class.
		/// To be able to load specified library (assembly) for use
		/// </summary>
		/// <param name="libraryFile">Path to .NET assembly (DLL) file that contains the remote library class to load.</param>
		/// <param name="libraryClassName">Name of remote library class to load, specified in the format of "NamespaceName.ClassName" without the quotes.</param>
		public XmlRpcMethods(string libraryFile, string libraryClassName)
		{
			library = Assembly.LoadFrom(libraryFile);
			libraryClass = libraryClassName;
		}
		
		/// <summary>
		/// Get a list of RobotFramework keywords available in remote library for use.
		/// 
		/// NOTE: Current implementation will return 
		/// extra unanticipated keywords from .NET remote class library, just ignore them
		/// for now, until we can optimize this .NET implementation.
		/// </summary>
		/// <returns>A string array of RobotFramework keywords available in remote library for use.</returns>
		[XmlRpcMethod]
		public string[] get_keyword_names()
  		{
			Type classType = library.GetType(libraryClass);			
			//MethodInfo[] mis = classType.GetMethods(BindingFlags.Public | BindingFlags.Static);
			//seem to have issue when trying to only get public & static methods, so get all instead
			MethodInfo[] mis = classType.GetMethods();
			
			//add one more for stop server that's part of the server
			string[] keyword_names = new string[mis.Length+1];
			int i = 0;			
			foreach(MethodInfo mi in mis)
			{
				keyword_names[i++] = mi.Name;
			}
			keyword_names[i] = "stop_remote_server";
			return keyword_names;
		}
		
		/// <summary>
		/// Run specified Robot Framework keyword from remote server.
		/// </summary>
		/// <param name="keyword">Keyword class library method to run for Robot Framework.</param>
		/// <param name="args">Arguments, if any, to pass to keyword method.</param>
		/// <returns></returns>
		[XmlRpcMethod]
		public keyword_results run_keyword(string keyword, object[] args)
  		{
			if(keyword == "stop_remote_server") stop_remote_server();
			Type classType = library.GetType(libraryClass);
			object libObj = Activator.CreateInstance(classType);
			MethodInfo mi = classType.GetMethod(keyword);
			keyword_results kr = new keyword_results();
			
			try
			{
				string retval = "";
				if(mi.ReturnType == typeof(System.Int32) ||
				   mi.ReturnType == typeof(System.String) ||
				   mi.ReturnType == typeof(System.Boolean))
				{
					//***case to handle basic data types
					//due to strict data typing by .NET, return value will
					//always be cast as a string in this implementation.
					//Until we can fix/optimize it to return value in any
					//one of the basic types: int, string, boolean, etc.
					retval = (string) mi.Invoke(libObj, args).ToString();
					if(mi.ReturnType == typeof(System.Boolean))
					{
						if(retval == "True")
							kr.status = "PASS";
						else //retval == "False"
							kr.status = "FAIL";						
					}
					else //return type ~int, string, etc. so always pass, if no exception
						kr.status = "PASS";					
				}
				else if(mi.ReturnType == typeof(System.Int32[]) ||
				   	mi.ReturnType == typeof(System.String[]) ||
				   	mi.ReturnType == typeof(System.Boolean[]))
				{
					//***case to handle array of basic data types
					//due to strict data typing by .NET, return value will
					//always be cast as a special delimited string value in this implementation.
					//Until we can fix/optimize it to return value in array of any
					//one of the basic types: int, string, boolean, etc.
					object[] tmpretval;
					tmpretval = (object[]) mi.Invoke(libObj, args);
					//return results in this string format: {item1, item2, ...}
					retval = "{";
					foreach(object obj in tmpretval)
					{
						retval = retval + (string) obj.ToString() + ",";
					}
					retval = retval + "}";
					kr.status = "PASS";
				}
				else
				{	
					//***case to handle keywords that don't return values, and all other cases
					//expect no return value from keyword, so always pass, if no exception					
					mi.Invoke(libObj, args);
					kr.status = "PASS";
				}
				kr.Return = retval;
				//due to limitation of .NET (I think) in not being able to redirect
				//standard (or stream) output from reflected/loaded library
				//output will always be empty with this implementation. Until we can
				//fix/optimize this deficiency.
				kr.output = "";
				kr.error = "";
				kr.traceback = "";						
				return kr;
			}
			catch(System.Exception ex)
			{
				kr.traceback = ex.StackTrace;
				kr.error = ex.Message;
				kr.output = ex.Message;
				kr.status = "FAIL";
				kr.Return = "";
				return kr;
			}					
		}
		
		/// <summary>
		/// As defined by Robot Framework spec, this keyword will remotely stop remote library server.
		/// To be called by Robot Framework remote library interface, or with any XML-RPC request.
		/// 
		/// NOTE: Currently will not return any XML-RPC response after being called, unlike the Python implementation.
		/// </summary>
		[XmlRpcMethod]
		public void stop_remote_server()
  		{
			//unfortunately, not able to send boolean/int value of 1 before server stops for some reason		
			System.Environment.Exit(0);
		}
		
		/// <summary>
		/// Get list of arguments for specified Robot Framework keyword.
		/// </summary>
		/// <param name="keyword">The keyword to get a list of arguments for.</param>
		/// <returns>A string array of arguments for the given keyword.</returns>
		[XmlRpcMethod]
		public string[] get_keyword_arguments(string keyword)
		{
			Type classType = library.GetType(libraryClass);
			MethodInfo mi = classType.GetMethod(keyword);
			ParameterInfo[] pis = mi.GetParameters();
			string[] args = new String[pis.Length];
			int i = 0;
			foreach(ParameterInfo pi in pis)
			{
				args[i++] = pi.Name;
			}
			return args;		
		}		
		//currently no implementation for get_keyword_documentation, until we can
		//figure out how to return documentation via .NET reflection, etc.
	}
	
	/// <summary>
	/// Robot Framework run_keyword return value data structure, based on spec at
	/// http://robotframework.googlecode.com/svn/tags/robotframework-2.5.6/doc/userguide/RobotFrameworkUserGuide.html#remote-library-interface
	/// 
	/// Due to strict data typing by .NET, return value will always be cast as a string in this implementation.
	/// Until we can fix/optimize it to return value in any one of the basic types: int, string, array of strings/ints, etc.
	/// </summary>
	public struct keyword_results
	{
		public string status; //Mandatory execution status. Either PASS or FAIL.
		public string output; //Possible output to write into the RobotFramework log file. Must be given as a single string but can contain multiple messages and different log levels in format *INFO* First message\n*INFO* Second\n*WARN* Another message.
		public string traceback; //Possible stack trace to write into the RobotFramework log file using DEBUG level when the execution fails.
		public string error; //Possible error message. Used only when the execution fails.
		[XmlRpcMember("return")]
		public string Return; //Possible return value. Must be one of the supported RobotFramework/Python data types.
		//due to strict data typing by .NET, return value will always be cast as a string in this implementation.
		//Until we can fix/optimize it to return value in any one of the basic types: int, string, array of strings/ints, etc.
	}
}