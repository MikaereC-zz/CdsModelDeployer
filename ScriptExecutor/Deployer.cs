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

        public void ExecuteFileList(IList<string> sqlScriptFiles, 
            IList<SearchReplacePair> replacements)
        {
            foreach (string file in sqlScriptFiles)
            {
                var sql = _fileUtility.GetFileContents(file);
                var updatedSql = _fileUtility.ReplaceTokens(sql, replacements);
                _sqlExecutor.ExecuteNonQuery(updatedSql);
            }
        }
    }
}
