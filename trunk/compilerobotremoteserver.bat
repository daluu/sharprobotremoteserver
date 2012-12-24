@echo off
rem .NET command line compiler example for compiling remote server & example library
rem You may alternatively compile with SharpDevelop, MonoDevelop, Visual Studio, etc. instead
rem by creating new console application and/or class library projects with the source files

rem change .NET framework path to desired target version as needed

rem compile robotremoteserver (from local path)
rem assumes XML-RPC.NET library DLL is precompiled and in local path
rem FYI get XML-RPC.NET library from www.xml-rpc.net
@echo on
%windir%\Microsoft.NET\Framework\v2.0.50727\csc.exe /out:robotremoteserver.exe /target:exe /reference:CookComputing.XmlRpcV2.dll robotremoteserver.cs
@echo off
rem compile examplelibrary with generated XML documentation
@echo on
%windir%\Microsoft.NET\Framework\v2.0.50727\csc.exe /out:examplelibrary.dll /target:library /doc:examplelibrary_doc.xml examplelibrary.cs