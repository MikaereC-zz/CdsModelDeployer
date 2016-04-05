using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptExecutor
{
    public class Deployer
    {
        private ISqlExecutor _sqlExecutor;
        private IFileUtility _fileUtility;
        public Deployer(ISqlExecutor sqlExecutor, IFileUtility fileUtility)
        {
            _sqlExecutor = sqlExecutor;
            _fileUtility = fileUtility;
        }

        public StringBuilder ExecuteFileList(IList<string> sqlScriptFiles, 
            IList<SearchReplacePair> replacements)
        {
            var sb = new StringBuilder();
            foreach (string file in sqlScriptFiles)
            {
                sb.AppendLine("**********Start of File: " + file);
                var sql = _fileUtility.GetFileContents(file);
                var updatedSql = _fileUtility.ReplaceTokens(sql, replacements);
                if (sql != updatedSql)
                {
                    sb.AppendLine("**NB: Script was updated !**");
                }
                sb.AppendLine(updatedSql);
                sb.AppendLine("**********End of File: " + file);
                _sqlExecutor.ExecuteNonQuery(updatedSql);
            }
            return sb;
        }
    }
}
