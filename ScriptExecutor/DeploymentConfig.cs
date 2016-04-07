using ScriptExecutor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptExecutor
{
    public class DeploymentConfig
    {
        public string TargetSqlServerName { get; set; }
        public string CdsModelDbName { get; set; }
        public string ScriptFolder { get; set; }
        public string ScriptArchiveFolder { get; set; }
        public List<SearchReplacePair> Replacements { get; set; }
    }
}
