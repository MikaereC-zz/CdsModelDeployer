using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ScriptExecutor.Test
{
    [TestClass]
    public class FileUtilityTest
    {
        [TestMethod]
        [DeploymentItem("TestScript.sql")]
        public void GetFileContentsTest()
        {
            string fileName = "TestScript.sql";
            var target = new FileUtility();
            string actual = target.GetFileContents(fileName);
            Assert.IsTrue(actual.Contains("dbo.SomeTable"));
        }
    }
}
