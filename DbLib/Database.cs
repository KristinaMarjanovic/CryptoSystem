using System.Data;
using System.Data.SqlClient;

namespace Db
{
    public class Database
    {
        private string _connectionString { get; }

        public Database(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public DataSet ExecStoredProc(string procedureName, SqlParameter[] sqlParameterCollection)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand sqlComm = new SqlCommand(procedureName, conn))
                {
                    sqlComm.Parameters.AddRange(sqlParameterCollection);

                    sqlComm.CommandType = CommandType.StoredProcedure;

                    using (SqlDataAdapter da = new SqlDataAdapter(sqlComm))
                        da.Fill(ds);
                }

            }
            return ds;
        }

    }
}
