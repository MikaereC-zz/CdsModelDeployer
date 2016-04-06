using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

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

        [TestMethod]
        public void ReplaceTokensTest()
        {
            SearchReplacePair srp1 = new SearchReplacePair()
            {
                SearchTerm = "~ReplaceMe1~",
                ReplacementTerm = "InsertedTerm1"
            };

            SearchReplacePair srp2 = new SearchReplacePair()
            {
                SearchTerm = "~ReplaceMe2~",
                ReplacementTerm = "InsertedTerm2"
            };

            List<SearchReplacePair> list = new List<SearchReplacePair>();
            list.Add(srp1);
            list.Add(srp2);

            string text = "select * from ~ReplaceMe1~ union select * from ~ReplaceMe2~";
            string expected = "select * from InsertedTerm1 union select * from InsertedTerm2";
            var target = new FileUtility();
            var actual = target.ReplaceTokens(text, list);

            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void ReplaceTokens_CaseInsensitive()
        {
            SearchReplacePair srp1 = new SearchReplacePair()
            {
                SearchTerm = "~ReplaceMe1~",
                ReplacementTerm = "InsertedTerm1"
            };

            SearchReplacePair srp2 = new SearchReplacePair()
            {
                SearchTerm = "~ReplaceMe2~",
                ReplacementTerm = "InsertedTerm2"
            };

            List<SearchReplacePair> list = new List<SearchReplacePair>();
            list.Add(srp1);
            list.Add(srp2);

            string text = "select * from ~REPLACEMe1~ union select * from ~replaceme2~";
            string expected = "select * from InsertedTerm1 union select * from InsertedTerm2";
            var target = new FileUtility();
            var actual = target.ReplaceTokens(text, list);

            Assert.AreEqual(expected, actual);

        }
        [TestMethod]
        public void ReplaceTokensTest_NullListReturnsOriginalText()
        {

            List<SearchReplacePair> list = null;

            string expected = "select * from ~ReplaceMe1~ union select * from ~ReplaceMe2~";
            var target = new FileUtility();
            var actual = target.ReplaceTokens(expected, list);

            Assert.AreEqual(expected, actual);

        }
    }
}
