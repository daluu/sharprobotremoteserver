rem .NET command line compiler example for compiling remote server & example library
rem You may alternatively compile with SharpDevelop, Visual Studio, etc. instead
rem by creating new console application and/or class library projects with the source files

rem compile robotremoteserver (from local path)
rem assumes XML-RPC.NET library DLL is precompiled and in local path
rem FYI get XML-RPC.NET library from
rem www.xml-rpc.net
rem change .NET framework path to desired version as needed

%windir%\Microsoft.NET\Framework\v2.0.50727\csc.exe /out:robotremoteserver.exe /target:exe /reference:CookComputing.XmlRpcV2.dll robotremoteserver.cs

rem compile examplelibrary with generated XML documentation
%windir%\Microsoft.NET\Framework\v2.0.50727\csc.exe /out:examplelibrary.dll /target:library /doc:examplelibrary_doc.xml examplelibrary.cs