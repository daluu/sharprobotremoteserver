

# Overview of usage ideas #

This page provides some ideas on what sharprobotremoteserver could be used for.

## Alternative to IronPython for .NET support with Robot Framewrok ##

More native .NET support than IronPython, less issues figuring out how to adapt calls to C#/VB.NET code into IronPython and debugging such calls. And IronPython support in Robot Framework is also experimental. But the remote server/library protocol is a standard specification for Robot Framework.

Useful for automation calls that block I/O or take time to execute or that could hang/block Robot Framework (if using IronPython for local test library). sharprobotremoteserver can execute such automation on another machine or in another process outside of Robot Framework. This would be a similar approach to use of JVMConnector for Robot Framework Java test libraries. Other alternative is to run Python remote server using IronPython, but we don't know how well that works.

Another benefit of this server is deploying test library/server in environment where you don't want to or can't install anything. With IronPython, you'd have to install the runtime to use .NET libraries, but maybe it can be compiled to a portable binary using SharpDevelop IDE (though not with Visual Studio yet).

## Starting base for Robot Framework .NET-based libraries ##

Since current [Robot Framework](http://www.robotframework.org) .NET support, via [IronPython](http://www.ironpython.net), is in experimental stage, and most users and developers of .NET (whether for [Robot Framework](http://www.robotframework.org) or not) are typically not well versed in [Python](http://www.python.org), this alternative might provide a better pathway to developing .NET-based test libraries for [Robot Framework](http://www.robotframework.org), at least initially until we have better .NET support.

Here are some ideas for [Robot Framework](http://www.robotframework.org) .NET test libraries that could be built (or better built with remote server over [IronPython](http://www.ironpython.net)):

  * Alternatives (or complementary libraries) to SeleniumLibrary, and Microsoft-centric web testing (e.g. Silverlight)
    * [WatiNLibrary](http://code.google.com/p/robotframework-watinlibrary/) (as alternative to [SeleniumLibrary](http://code.google.com/p/robotframework-seleniumlibrary/))
    * webAiiLibrary (http://www.telerik.com/products/web-testing-tools/webaii-framework-features.aspx)
    * InCisifLibrary (http://www.incisif.net/home.asp?ok=ok)
    * SWATLibrary (http://sourceforge.net/projects/ulti-swat/)
  * Microsoft and .NET centric (desktop) GUI automation (and web too)
    * WhiteLibrary (http://white.codeplex.com/)
    * UIAutomationLibrary (http://en.wikipedia.org/wiki/Microsoft_UI_Automation)
    * AdLabQuailLibrary (http://www.andreev.org/qa-testing-automation)
    * QAlibersLibrary (http://www.qaliber.net/)
    * RanorexLibrary (http://www.andreev.org/qa-testing-automation)
    * Silk4NetLibrary (http://www.borland.com/us/products/silk/silk4net.html)
    * SOAP/XML-RPC/REST web service test library? By implementing test library as a web service client that makes requests and validates responses.
    * Microsoft WCF-based web services test library? By implementing test library as a web service client that makes requests and validates responses. Especially useful for WCF or SOAP services that use Windows authentication that other SOAP tools/libraries don't support (e.g. SOAPUI).
    * Implement test libraries for .NET features that would otherwise not be available with or problematic for [ActivePython](http://www.activestate.com/activepython), or Windows version of [Python](http://www.python.org), or [IronPython](http://www.ironpython.net).

## Use as generic XML-RPC server ##

Generic remote server is a working implementation of an XML-RPC server that doesn't require IIS to run, and instead of implementing XML-RPC web service API methods in the server itself, can use server's .NET reflection abilities to implement the APIs in a separate .NET class library that is loaded by server at runtime/startup.

## Other ideas ##

  * Deploy test library/server in environment where you don't want to or can't install anything (unlike possibly IronPython). Just copy over binaries.
  * Pair remote server with custom test library to bypass Windows 7/Vista/Server 2008 UAC or need for "Run As Administrator" for test scripts/libraries/automation.
  * Pair remote server with a Hyper-V test library to start/stop/manage virtual machines. Similarly for Virtual Server 2005, etc.
  * You name it?