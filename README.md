# Sourcetrail Extension for Visual Studio
Currently supports VS2012, VS2013, VS2015 and VS2017.

## Links
* Project Home, News: [www.sourcetrail.com](https://www.sourcetrail.com/) 
* Documentation: [www.sourcetrail.com/documentation](https://www.sourcetrail.com/documentation/#VisualStudio) 
* Download, Reviews: [Visual Studio Marketplace ](https://marketplace.visualstudio.com/items?itemName=vs-publisher-1208751.SourcetrailExtension)
* Code, Issues: [GitHub](https://github.com/CoatiSoftware/vs-sourcetrail) 

## Features

### Set your Visual Studio cursor
![](https://github.com/CoatiSoftware/vs-sourcetrail/blob/master/images/vs_extension_use_in_sourcetrail.png)

This extension allows you to set your Visual Studio text cursor to the source code location currently viewed in Sourcetrail. If the viewed file is not open in Visual Studio, the extension will open and display it automatically.

### Acticate a Symbol in Sourcetrail
![](https://github.com/CoatiSoftware/vs-sourcetrail/blob/master/images/vs_extension_use_in_visual_studio.png)

Whenever you read some source code inside Visual Studio that you actually want to explore in Sourcetrail you can use this extension to activate the right-clicked symbol in Sourcetrail. 

### Create a Clang Compilation Database from a VS Solution
![](https://github.com/CoatiSoftware/vs-sourcetrail/blob/master/images/vs_extension_dialog.png)

As a Clang based tool Sourcetrail supports the [JSON Compilation Database](https://clang.llvm.org/docs/JSONCompilationDatabase.html) format for simplified project setup. This extension enables you to generate a JSON Compilation Database from your Visual Studio projects and solutions. 

__The great news is:__ This format is independent from the Sourcetrail tool, so you can also use the generated Compilation Database to run other Clang based tools.

## Building the Extension
Use Visual Studio to open the `SourcetrailExtension.sln` and build the project called `SourcetrailExtension`. 

__Hint:__ If the build fails with an error that reads `The "GetDeploymentPathFromVsixManifest" task failed unexpectedly.` try to run the `Reset the Visual Studio 2017 Experimental Instance` command first. 

## Running the Extension in Debug Mode
In oder to run the Sourcetrail Extension in debug mode, you need to adjust some settings. Open the `Properties` page of the `SourcetrailExtension` project and set the following values:
* Under `Debug` -> `Start action` select `Start external program` and point the path to `path\to\vs\Common7\IDE\devenv.exe`
* Under `Debug` -> `Start options` add `/rootsuffix Exp` as `Command line arguments`

## Running the Tests
The `SourcetrailExtensionTests` project contains both, unit tests and integration tests. When running an integration test it will automatically fire up a new instance of Visual Studio (called Experimental Instance) and simulate calls to the extension inside this instance. To make this work you need to point Visual Studio the the appropriate .testsettings file: 
* From the menu bar choose `Test` -> `Test Settings` -> `Select Test Settings File` and pick the `IntegrationTests.testsettings` file located at the root of this repository.

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
