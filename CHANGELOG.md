# Changelog


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
