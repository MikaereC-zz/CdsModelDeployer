using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System.Collections.Generic;

namespace ScriptExecutor.Test
{
    

    [TestClass]
    public class DeployerTest
    {
        private ISqlExecutor _sqlExecutorMock;
        private IFileUtility _fileUtilityMock;
        private Deployer _target;
        [TestInitialize]
        public void TestInitialise()
        {
            _sqlExecutorMock = MockRepository.GenerateStrictMock<ISqlExecutor>();
            _fileUtilityMock = MockRepository.GenerateStrictMock<IFileUtility>();
            _target = new Deployer(_sqlExecutorMock, _fileUtilityMock);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _sqlExecutorMock.VerifyAllExpectations();
            _fileUtilityMock.VerifyAllExpectations();
        }
        [TestMethod]
        public void ExecuteFileListTest()
        {
            string file1 = "file1";
            string file2 = "file2";
            string file3 = "file3";
            List<SearchReplacePair> replacements = new List<SearchReplacePair>();

            var fileList = new List<string>();
            fileList.Add(file1);
            fileList.Add(file2);
            fileList.Add(file3);

            string fileContents1 = "fileContents1";
            string fileContents2 = "fileContents2";
            string fileContents3 = "fileContents3";

            string replacedContents1 = "replacedContents1";
            string replacedContents2 = "replacedContents2";
            string replacedContents3 = "replacedContents3";

            //Setup expectations
            _fileUtilityMock.Expect(m => m.GetFileContents(file1)).Return(fileContents1);
            _fileUtilityMock.Expect(m => m.ReplaceTokens(fileContents1, replacements)).Return(replacedContents1);
            _sqlExecutorMock.Expect(m => m.ExecuteNonQuery(replacedContents1)).Return(true);

            _fileUtilityMock.Expect(m => m.GetFileContents(file2)).Return(fileContents2);
            _fileUtilityMock.Expect(m => m.ReplaceTokens(fileContents2, replacements)).Return(replacedContents2);
            _sqlExecutorMock.Expect(m => m.ExecuteNonQuery(replacedContents2)).Return(true);

            _fileUtilityMock.Expect(m => m.GetFileContents(file3)).Return(fileContents3);
            _fileUtilityMock.Expect(m => m.ReplaceTokens(fileContents3, replacements)).Return(replacedContents3);
            _sqlExecutorMock.Expect(m => m.ExecuteNonQuery(replacedContents3)).Return(true);

            _target.ExecuteFileList(fileList, replacements);
        }
    }
}
