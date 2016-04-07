using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptExecutor
{
    public interface IFileUtility
    {
        string GetFileContents(string filePath);

        string ReplaceTokens(string text, IList<SearchReplacePair> replacements);

        void ArchiveFile(string archiveFolder, string filePath);
    }
}
