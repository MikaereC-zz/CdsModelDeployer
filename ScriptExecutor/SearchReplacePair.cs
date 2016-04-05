using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptExecutor
{
    public class SearchReplacePair
    {
        public string SearchTerm { get; set; }
        public string ReplacementTerm { get; set; }
        public bool IsDatabaseName { get; set; }
    }
}
