# Changelog


## 2.0.3

**2019-08-19**

* Fixes
	* The create-compilation-database menu option stayed disabled if a solution was loaded directly on startup of VS


## 2.0.2

**2019-07-02**

* Fixes
	* The create-compilation-database event sent by Sourcetrail was handled on the background thread which was unable to trigger UI changes.
	
	
## 2.0.1

**2019-07-01**

* Fixes
	* The synchronization event sent by Sourcetrail was handled on the background thread which was unable to trigger UI changes.
	
	
## 2.0.0

**2019-06-11**

* Fixes
	* Switched to asynchronous initialization, which is required by the VS marketplace but forces dropping support for VS 2012 and VS 2013.
	
	
## 1.9.1

**2019-06-11**

* Features
	* Added support for Visual Studio 2019
* Fixes
	* Fixed options generator to remove all occurrences of "$(NOINHERIT)" and "$(INHERIT)" placeholders from the tool's additional options
	

## 1.9.0

**2019-02-19**

* Fixes
	* Added Newtonsoft.Json.dll to released VSIX package because this dependency is missing on some systems.
	* Set suggested name of exported compilation database to "compile_commands.json" because this is the standard name for this file which is expected by other clang based tools (e.g. clang-tidy)
	

## 1.8

**2018-09-14**

* Fixes
	* Fixes out of memory issue for large Visual Studio projects
	* Fixes trailing comma appended to generated compile commands


## 1.7

**2018-06-26**

* Fixes
	* Handles an exception that was thrown if the loaded project uses a legacy configuration type
	* Improves log information


## 1.6

**2018-06-05**

* Fixes
	* Fixes an exception that was thrown if the detected include paths were surrounded by quotes, which caused the generated compilation database to be empty.


## 1.5

**2018-05-04**

* Fixes
	* Sourcetrail Extension will now consider the file level option "Excluded From Build" and prevent the respective files from being added to the generated compilation database.


## 1.4

**2018-03-26**

* Features
	* New buttons that allow to auto-select all projects that reference/are referenced by the currently selected ones.
* Fixes
	* Fixes extension icon incompatibility with new Visual Studio Marketplace by removing largest icon layer size.
	* Fixes exception that caused exported Compilation Database to be empty by using an older version of referenced Newtonsoft.Json package.


## 1.3

**2018-02-16**

* Fixes
	* Fixes text encoding used in messages that synchronize Visual Studio and Sourcetrail.


## 1.2

**2017-10-31**

* Fixes
	* Fixes exceptions raised due to CR/LF characters appearing in extracted header search paths (which can be introduced when project properties are provided via .props file) by stripping whitespace from each item after splitting on semicolon.


## 1.1

**2017-10-17**

* Features
	* New option to allow compilation database generation to use `-isystem` for platform include diretories and `-I` for project additional include directories. A checkbox was added to optionally enable using `-isystem` for all include directories.


## 1.0.7

**2017-10-04**

* Features
	* New option to specify additional clang options for compilation database generation.
* Fixes
	* Fixes missing project properties in clang compilation databases ([issue #12](https://github.com/CoatiSoftware/vs-sourcetrail/issues/12), [issue #18](https://github.com/CoatiSoftware/vs-sourcetrail/issues/18)).


## 1.0.6

**2017-10-01**

* Fixes
	* Avoids writing a trailing comma to the clang compilation database ([issue #10](https://github.com/CoatiSoftware/vs-sourcetrail/issues/10)).


## 1.0.5

**2017-09-27**

* Features
	* Implemented support for Visual Studio Makefile projects ([issue #8](https://github.com/CoatiSoftware/vs-sourcetrail/issues/8)).
	* Improved compilation database creation performance.
	* Reduced memory usage while creating a compilation database for a large solution.
* Fixes
	* The extension did not respect the difference between Visual Studio project configurations and solution configurations. This caused the compilation database creation dialog to display a list of all available project configurations. If the user picked one that did not exist for every project, those projects that did not offer that configuration got ignored ([issue #9](https://github.com/CoatiSoftware/vs-sourcetrail/issues/4)).


## 1.0.4

**2017-07-26**

* Features
	* Implemented retrieving Include Directoried and Preprocessor Defines from Visual Studio Property Sheets in case a project uses the inherited values ([issue #5](https://github.com/CoatiSoftware/vs-sourcetrail/issues/5)).
* Fixes
	* Files that don't have a language specific `CompileAs` option will now get a language assigned based on their file extension ([issue #4](https://github.com/CoatiSoftware/vs-sourcetrail/issues/4)).


## 1.0.3

**2017-07-24**

* Features
	* Implemented conversion of [Forced Include](https://msdn.microsoft.com/en-us/library/8c5ztk84.aspx) files to the respective `-include` [Clang option](http://clang.llvm.org/docs/CommandGuide/clang.html#cmdoption-include) ([issue #1](https://github.com/CoatiSoftware/vs-sourcetrail/issues/1)).
* Fixes
	* Changed quote characters used around paths from `'` to `"` to be compatible with Clang 4.0.


## 1.0.2

**2017-07-20**

* Fixes
	* Fixed another compatibility issue with VS2012 and VS 2013 caused by referencing the wrong version of a Visual Studio assembly ([issue #2](https://github.com/CoatiSoftware/vs-sourcetrail/issues/2)).


## 1.0.1

**2017-07-19**

* Fixes
	* Fixed compatibility issues with VS 2012 and VS 2013.


## 1.0.0

**2017-07-06**

* This is the first official release that is made available on the [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=vs-publisher-1208751.SourcetrailExtension).
