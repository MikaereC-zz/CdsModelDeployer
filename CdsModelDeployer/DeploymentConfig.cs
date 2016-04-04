using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CdsModelDeployer
{
    public class DeploymentConfig
    {
        public string TargetSqlServerName { get; set; }
        public string IfDataMartDbName { get; set; }
        public string CdsModelDbName { get; set; }
        public string ScriptFolder { get; set; }
    }
}
