using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ООО_Техносервис_2
{
    internal class DataBaseClass
    {
        public static string USERS_ID = "null", Password = "null", APP_NAME = "ООО Техносервис";
        public static string ConnectionString = "Data Source = KANDREY1604\\MYSERVERSQL; " +
            "Initial Catalog = OOO_Tehnoservice_2; Persist Security Info = true; USER ID = {0}; Password = '{1}';";
        public SqlConnection connection = new SqlConnection(ConnectionString);
        public SqlCommand command = new SqlCommand();
        public DataTable dataTable = new DataTable();
        public SqlDependency dependency = new SqlDependency();

        public enum act { select, manipulation }

        public void sqlExequte(string query, act actionType)
        {
            command.Connection = connection;
            command.CommandText = query;
            command.Notification = null;

            switch (actionType)
            {
                case act.select:
                    dependency.AddCommandDependency(command);
                    SqlDependency.Start(connection.ConnectionString);
                    connection.Open();
                    dataTable.Load(command.ExecuteReader());
                    connection.Close();
                    break;
                case act.manipulation:
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    break;

            }
        }
    }
}
