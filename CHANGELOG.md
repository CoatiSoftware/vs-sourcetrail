# Changelog


## 1.0.4

**2017-07-26**

* Features
	* implemented retrieving Include Directoried and Preprocessor Defines from Visual Studio Property Sheets in case a project uses the inherited values ([issue #5](https://github.com/CoatiSoftware/vs-sourcetrail/issues/5)).
* Fixes
	* files that don't have a language specific `CompileAs` option will now get a language assigned based on their file extension ([issue #4](https://github.com/CoatiSoftware/vs-sourcetrail/issues/4)).


## 1.0.3

**2017-07-24**

* Features
	* implemented conversion of [Forced Include](https://msdn.microsoft.com/en-us/library/8c5ztk84.aspx) files to the respective `-include` [Clang option](http://clang.llvm.org/docs/CommandGuide/clang.html#cmdoption-include) ([issue #1](https://github.com/CoatiSoftware/vs-sourcetrail/issues/1)).
* Fixes
	* changed quote characters used around paths from `'` to `"` to be compatible with Clang 4.0


## 1.0.2

**2017-07-20**

* Fixes
	* fixed another compatibility issue with VS2012 and VS 2013 caused by referencing the wrong version of a Visual Studio assembly ([issue #2](https://github.com/CoatiSoftware/vs-sourcetrail/issues/2))


## 1.0.1

**2017-07-19**

* Fixes
	* fixed compatibility issues with VS 2012 and VS 2013


## 1.0.0

**2017-07-06**

* first official release that is made available on the [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=vs-publisher-1208751.SourcetrailExtension)
