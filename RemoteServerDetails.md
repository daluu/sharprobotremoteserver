

# Installation #

## Prerequisites ##

  * .NET Framework (version 2.0 and above, but not tested & compiled against all versions. May work with Mono as well, but untested.)
  * [XML-RPC.NET](http://www.xml-rpc.net) library (compiled DLL binary)
  * Example .NET remote library, bundled with the generic server, or your own .NET remote (class) library, for use with [Robot Framework](http://www.robotframework.org).
  * If using with [Robot Framework](http://www.robotframework.org) locally, running server on localhost, then need to install [Robot Framework](http://www.robotframework.org).
  * [SharpDevelop](http://www.sharpdevelop.net), msbuild, or a version of [Visual Studio](http://www.microsoft.com/visualstudio/en-us/) may be required to compile [XML-RPC.NET](http://www.xml-rpc.net) library.

## Installation procedure ##

As the remote server acts as a remote library for [Robot Framework](http://www.robotframework.org), no installation is needed. But the prerequisites have to be met first.

  1. Download and compile [XML-RPC.NET](http://www.xml-rpc.net) library, if needed.
  1. Download the [generic remote server zip file package](http://sharprobotremoteserver.googlecode.com/files/sharprobotremoteserver-1.2.zip).
  1. Extract to desired location.
  1. Copy over [XML-RPC.NET](http://www.xml-rpc.net) library DLL to extracted location, if needed.

You may add additional .NET class library DLLs in extracted location to start the remote server with those libraries instead of the provided example library.

# Usage instructions #

Run **robotremoteserver** passing it path to .NET class library DLL that contains the implementation of keywords (e.g. actual remote library), and name of class to use in the form of **NamespaceName.ClassName** without quotes. You can also run server without arguments to get this usage info.

Server also takes optional parameters for keyword documentation, IP address, port, and whether to allow remotely stopping server or they will default to the [Robot Framework](http://www.robotframework.org) defaults of localhost/127.0.0.1, 8270, and to allow stopping server, respectively. For keyword documentation, you have to supply an XML documentation file for the .NET class assembly that can be generated during code compilation. If not provided, documentation will simply be blank when called by Robot Framework.

See [RemoteServerDetails#Testing\_the\_example\_remote\_library](RemoteServerDetails#Testing_the_example_remote_library.md) for an example of usage.

# Testing the example remote library #

Run robotremoteserver with the examplelibrary like follows:

> `robotremoteserver.exe --library examplelibrary.dll --class RobotFramework.ExampleRemoteLibrary --doc examplelibrary_doc.xml`

Then run the [example tests](http://robotframework.googlecode.com/svn/trunk/tools/remoteserver/example/remote_tests.html) for remote libraries/servers available from [Robot Framework](http://www.robotframework.org) project.

To test stop remote server functionality, you may wish to add a test case, test step, or test case/suite teardown like this:

| Test Case | Action | Argument |
|:----------|:-------|:---------|
| Stop Server Test | Run Keyword | Stop Remote Server |

# .NET remote library interface with the generic remote server #

The generic remote server uses .NET reflection to access the actual remote library during runtime using late-binding technique. Alternatively, you may wish to integrate the library code into the remote server and make it non-generic rather than use dynamic loading/late-binding.

The remote server includes keyword **stop\_remote\_server** so you don't have to implement that in the remote library.

Remote library methods should conform to [Robot Framework](http://www.robotframework.org) keyword API specification, meaning: methods should be named as **method\_name()** rather than MethodName() or methodName(); the underscore represents a space; the method is made available as a keyword in [Robot Framework](http://www.robotframework.org) named **Method Name**. Alternatively, they might not have to follow this convention, but you would have to modify the remote server to be able to translate the Robot Framework keyword naming convention to the actual .NET method naming convention when XML-RPC calls are made to **run\_keyword**.

Additionally, the library's use of data types in keyword arguments or return values should conform to the [XML-RPC](http://www.xmlrpc.com/spec) protocol and what is supported by [Robot Framework](http://www.robotframework.org).

## Designing new custom test libraries in .NET ##

For this case, you need only follow the .NET remote library interface guidelines when creating your test library for it to be callable from [Robot Framework](http://www.robotframework.org).

## Re-using existing .NET libraries for Robot Framework ##

For this case, you would need to write a wrapper class library that provides the remote library interface on the front end and makes the actual calls to the desired .NET class library (or web service) on the back end, for it to be callable from [Robot Framework](http://www.robotframework.org).

## Using .NET libraries that reference web services ##

If loading a .NET library into remote server, and that library references a web service, then in order for the binding to work correctly, you may have to copy the library's or web service client/consumer's app.config (or app.exe.config) data and save it as robotremoteserver.exe.config.

An alternative solution would be to modify the remote server or the remote (class) library to use dynamic binding at runtime rather than the app.config (or appName.exe.config) binding method.

## Documenting your .NET remote library ##

You can generate Robot Framework HTML documentation (or maybe even documentation that is logged in test logs during test runs) for your test library by doing the following sequence of steps:

  1. Add the desired .NET XML (summary) code comments for classes and class methods to your test class library.
  1. Compile the class library with the documentation option to generate an accompanying XML documentation file. If compiling from command line, that would be the "/doc:pathToFile" option.
  1. Load the test library and its documentation file with remote server.
  1. Run the [libdoc.py](http://code.google.com/p/robotframework/wiki/SupportingTools#Library_Documentation_Generator_(libdoc.py)) support tool from Robot Framework to generate the Robot Framework HTML documentation for the test library, as follows:

> `python libdoc.py --name MyLibraryName --argument localhost:8270 Remote`

You can alternatively use IronPython instead of Python here as well.

# Known Issues #

  * **get\_keyword\_names** may return extra .NET library methods that weren't intended to be used by [Robot Framework](http://www.robotframework.org) (e.g. internal methods), so just ignore them. Bear in mind that if you don't debug the remote library, you won't notice this issue anyways as [Robot Framework](http://www.robotframework.org) doesn't explicitly dump a list of available keywords for the end user to see, except for library documentation generation with libdoc.py. ([issue 5](http://code.google.com/p/sharprobotremoteserver/issues/detail?id=5))

  * Remote server currently not designed with "web services" as a remote library in mind, but can be modified to load a "web service" reference as the remote library rather than a local class library assembly DLL. Alternative is to build a wrapper class library that calls/references the web service and then load the wrapper class library with remote server. That way you don't have to modify the remote server code. But with this approach, you may have to create/add a robotremoteserver.exe.config file to specify bindings to the web service, etc. unless you modify the remote server code or implement the wrapper class library to use dynamic binding rather than binding through app.config (or appName.exe.config), etc.

# Tips for Debugging #

  * You can use a REST client (like popular browser extensions) to pass correct [XML-RPC](http://www.xmlrpc.com/spec) requests (an HTTP POST with data payload in XML-RPC format) and validate correct XML response from remote library.

  * You can use [Robot Framework](http://www.robotframework.org) tests to make the XML-RPC calls and validate correct responses, etc. if you are not good with XML-RPC messaging.

  * For library DLL related issues, you can build other .NET tools (e.g. console app or WinForms app) to load the same library being used with remote server and test to make sure library works correctly before loading into server at runtime.