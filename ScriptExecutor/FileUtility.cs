using System;
using System.Collections.Generic;
using System.IO;

namespace ScriptExecutor
{
    public class FileUtility : IFileUtility
    {
        public string GetFileContents(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public string ReplaceTokens(string text, IList<SearchReplacePair> replacements)
        {
            throw new NotImplementedException();
        }
    }
}
