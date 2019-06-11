# Sourcetrail Extension for Visual Studio
All-in-one extension with support for VS2012, VS2013, VS2015, VS2017 and VS2019.

[![Build status](https://ci.appveyor.com/api/projects/status/jlathk3h7nw6a57k?svg=true)](https://ci.appveyor.com/project/mlangkabel/vs-sourcetrail)

## Links
* Project Home, News: [www.sourcetrail.com](https://www.sourcetrail.com/) 
* Documentation: [www.sourcetrail.com/documentation](https://www.sourcetrail.com/documentation/#VisualStudio) 
* Download, Reviews: [Visual Studio Marketplace ](https://marketplace.visualstudio.com/items?itemName=vs-publisher-1208751.SourcetrailExtension)
* Code, Issues: [GitHub](https://github.com/CoatiSoftware/vs-sourcetrail) 

## Features

### Set your Visual Studio cursor
![](https://raw.githubusercontent.com/CoatiSoftware/vs-sourcetrail/master/images/vs_extension_use_in_sourcetrail.png)

This extension allows you to set your Visual Studio text cursor to the source code location currently viewed in Sourcetrail. If the viewed file is not open in Visual Studio, the extension will open and display it automatically.

### Acticate a Symbol in Sourcetrail
![](https://raw.githubusercontent.com/CoatiSoftware/vs-sourcetrail/master/images/vs_extension_use_in_visual_studio.png)

Whenever you read some source code inside Visual Studio that you actually want to explore in Sourcetrail you can use this extension to activate the right-clicked symbol in Sourcetrail. 

### Create a Clang Compilation Database from a VS Solution
![](https://raw.githubusercontent.com/CoatiSoftware/vs-sourcetrail/master/images/vs_extension_dialog.png)

As a Clang based tool Sourcetrail supports the [JSON Compilation Database](https://clang.llvm.org/docs/JSONCompilationDatabase.html) format for simplified project setup. This extension enables you to generate a JSON Compilation Database from your Visual Studio projects and solutions. 

__The great news is:__ This format is independent from the Sourcetrail tool, so you can also use the generated Compilation Database to run other Clang based tools.

## Building the Extension
Use Visual Studio to open the `SourcetrailExtension.sln` and build the project called `SourcetrailExtension`. 

__Hint:__ If the build fails with an error that reads `The "GetDeploymentPathFromVsixManifest" task failed unexpectedly.` try to run the `Reset the Visual Studio 2017 Experimental Instance` command first. 

## Running the Tests
The `SourcetrailExtensionTests` project contains both, unit tests and integration tests. When running an integration test it will automatically fire up a new instance of Visual Studio (called Experimental Instance) and simulate calls to the extension inside this instance. To make this work you need to point Visual Studio the the appropriate .testsettings file: 
* From the menu bar choose `Test` -> `Test Settings` -> `Select Test Settings File` and pick the `IntegrationTests.testsettings` file located at the root of this repository.

## Contributing
We really appreciate every possible kind of contributions. Pull-requests are greatly appreciated. If you want to submit one, please try to keep it as small as possible, so that we don't have to consider too many independent changes in the reviews. Better issue many small pull requests than one big one. That would let us pull smaller changes more quickly.

Thanks to [Dakota Hawkins](https://github.com/dakotahawkins) for contributions.

## Troubleshooting

The first step to troubleshooting is to enable logging for the extension. This can be found in the settings for Sourcetrail in `Tools` -> `Options` -> `Sourcetrail`

### `Create Compilation Database` is greyed out
Check that the loaded solution contains at least one C/C++ project in it. 
If logging is enabled for Sourcetrail, there may be messages in the output window resembling this one:

`Error: Exception: The solution's source code database may not have been opened. Please make sure the solution is not open in another copy of Visual Studio, and that its database file is not read only.`

This indicates that Intellisense or the browse database is disabled. Sourcetrail requires the browse database. It can be enabled in `Tools` -> `Options` -> `Text Editor` -> `C/C++` -> `Advanced`.

## Project Structure
The `SourcetrailExtension` solution contains several different projects. This section should explain their purpose.

### SourcetrailExtension
This project build the extension and contains the source code for all the main functionality.

### SourcetrailExtensionTests
This project contains all the unit tests and integration tests for the SourcetrailExtension project

### SourcetrailExtensionUtility
This project contains some utility functionality (like the logging implementation) used by both, the `SourcetrailExtension` project and the `SourcetrailExtensionTests` project.

### VCProjectEngineWrapperFactories
Ok, now things are getting interesting. While extracting all the required information for generating a Clang Compilation Database the Sourcetrail Extension uses the `Microsoft.VisualStudio.VCProjectEngine` assembly. Everything would be fine if we'd just build the extension for one single version of Visual Studio. But we are not. 

#### The Issue
According to [this question on stackoverflow](https://stackoverflow.com/questions/44288050/typecast-fails-when-visual-studio-extension-uses-reference-to-older-assembly) the classes of the `Microsoft.VisualStudio.VCProjectEngine` assembly have different GUIDs for each version released (e.g. 11, 12, 14, 15). So if we would build the extension referencing to `VCProjectEngine` version 15 (which is VS 2017) and install the extension in another version of VS (e.g. VS 2015) every cast to `VCProjectEngine` types (e.g. `VCProject`) would fail. Internally VS 2015 would have a `VCProject` of version 14 but our code tries to cast it to a `VCProject` of version 15 and fails. 

#### The Solution
In order to build just one extension that supports all versions of Visual Studio we are using wrappers for all `VCProjectEngine` types that we are interested in. Each kind of wrapper (e.g. the `VCProjectWrapper`) has an interface that offers all methods required in the context of this extension. Each interface has several implementations (one for each VS version). When a wrapped object instance is needed during runtime, a factory class that knows all available wrappers tries to instantiate each of them and returns the wrapper object that has been instantiated successfully (the one where the cast to the wrapped type succeeded).

### VCProjectEngineWrapperInterfaces
This project contains the interface classes for the VCProjectEngine wrappers. This way other projects may use a wrapper without knowing exactly which version of the wrapper is used.

### VCProjectEngineWrapperVS20XX
The projects for the actual wrappers all reference the same source files, so we have as little code duplication as possible. We require different projects for the individual wrappers because each wrapper's project references another version of the `Microsoft.VisualStudio.VCProjectEngine` assembly.
