rem .NET command line compiler example for compiling remote server & example library
rem You may alternatively compile with SharpDevelop, Visual Studio, etc. instead
rem compile robotremoteserver from local path
rem assumes XML-RPC.NET library DLL is compiled and in local path
rem change .NET framework path to desired version as needed
%windir%\Microsoft.NET\Framework\v2.0.50727\csc.exe /out:robotremoteserver.exe /target:exe /reference:CookComputing.XmlRpcV2.dll robotremoteserver.cs
rem compile examplelibrary
%windir%\Microsoft.NET\Framework\v2.0.50727\csc.exe /out:examplelibrary.dll /target:library examplelibrary.cs