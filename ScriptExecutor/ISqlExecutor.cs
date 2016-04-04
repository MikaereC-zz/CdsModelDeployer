using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptExecutor
{
    public interface ISqlExecutor
    {
        /// <summary>
        /// Returns true if the command succeed, false if exception occurred
        /// </summary>
        /// <param name="sqlCommandText"></param>
        /// <returns></returns>
        bool ExecuteNonQuery(string sqlCommandText);
        /// <summary>
        /// Executes a scalar and returns the result as a string
        /// </summary>
        /// <param name="sqlCommandText"></param>
        /// <returns></returns>
        string ExecuteScalar(string sqlCommandText);
        Exception Exception { get; }
    }
}
