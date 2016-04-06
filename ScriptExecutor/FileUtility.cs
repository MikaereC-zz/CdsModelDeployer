using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

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
            if (replacements == null)
            {
                return text;
            }
            string currentText = text;
            foreach (var replacement in replacements)
            {
                //currentText = currentText.Replace(replacement.SearchTerm, replacement.ReplacementTerm );
                currentText = Regex.Replace(currentText, replacement.SearchTerm, replacement.ReplacementTerm, RegexOptions.IgnoreCase);
            }
            return currentText;
        }
    }
}
