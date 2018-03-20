/*
 * Copyright 2018 Coati Software KG
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using CoatiSoftware.SourcetrailExtension.SolutionParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoatiSoftware.SourcetrailExtension.Tests
{
	[TestClass]
	public class CompilationDatabaseTests
	{
		[TestCategory("UnitTest"), TestMethod]
		public void TestComparingCompileCommandWithSelfByValueWorks()
		{
			CompileCommand command1 = new CompileCommand();
			command1.File = "test.cpp";
			command1.Directory = "./";
			command1.Command = "D test test.cpp";

			CompileCommand command2 = new CompileCommand();
			command2.File = "test.cpp";
			command2.Directory = "./";
			command2.Command = "D test test.cpp";

			Assert.IsTrue(command1 == command2);
		}

		[TestCategory("UnitTest"), TestMethod]
		public void TestComparingCompilationDatabaseWithSelfByValueWorks()
		{
			CompileCommand command = new CompileCommand();
			command.File = "test.cpp";
			command.Directory = "./";
			command.Command = "D test test.cpp";

			CompilationDatabase cdb1 = new CompilationDatabase();
			cdb1.AddCompileCommand(command);

			CompilationDatabase cdb2 = new CompilationDatabase();
			cdb2.AddCompileCommand(command);

			Assert.IsTrue(cdb1 == cdb2);
		}

		[TestCategory("UnitTest"), TestMethod]
		public void TestCompilationDatabaseRetainsEscapedQuotesWhenDeserializedAfterSerialization()
		{
			CompileCommand command = new CompileCommand();
			command.File = "test.cpp";
			command.Directory = "./";
			command.Command = "D DEFINE=\"value\" test.cpp";

			CompilationDatabase originalCompilationDatabase = new CompilationDatabase();
			originalCompilationDatabase.AddCompileCommand(command);
			string serialized = originalCompilationDatabase.SerializeToJson();

			CompilationDatabase deserializedCompilationDatabase = new CompilationDatabase();
			deserializedCompilationDatabase.DeserializeFromJson(serialized);

			Assert.IsTrue(deserializedCompilationDatabase == originalCompilationDatabase);
		}
	}
}