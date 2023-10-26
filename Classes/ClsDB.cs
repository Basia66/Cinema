using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Classes
{
    public static class ClsDB
    {
        public static SqlConnection Get_DB_Connection()
        {

            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string cn_String = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=" + projectDirectory + "\\Database.mdf;Integrated Security=True";

            SqlConnection cn_connection = new SqlConnection(cn_String);

            if (cn_connection.State != ConnectionState.Open) cn_connection.Open();
            return cn_connection;
        }

        public static DataTable Get_DataTable(string SQL_Text)
        {

            SqlConnection cn_connection = Get_DB_Connection();
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(SQL_Text, cn_connection);
            adapter.Fill(table);

            return table;
        }

        public static DataTable Get_DataTable(string SQL_Text, List<object[]> parameters)
        {
            SqlConnection cn_connection = Get_DB_Connection();
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(SQL_Text, cn_connection);
            foreach (object[] param in parameters)
            {
                if (param[1].GetType() == typeof(string))
                {
                    adapter.SelectCommand.Parameters.Add(
                        new SqlParameter
                        {
                            ParameterName = param[0].ToString(),
                            Value = param[1].ToString(),
                            SqlDbType = SqlDbType.VarChar,
                            Size = -1
                        });
                }
                else if (param[1].GetType() == typeof(int))
                {
                    adapter.SelectCommand.Parameters.Add(
                        new SqlParameter
                        {
                            ParameterName = param[0].ToString(),
                            Value = param[1],
                            SqlDbType = SqlDbType.Int
                        });
                }
            }

            adapter.Fill(table);
            return table;
        }

        public static void Update_Data(string SQL_Text, List<object[]> parameters)
        {
            SqlConnection cn_connection = Get_DB_Connection();
            //SqlDataAdapter adapter = new SqlDataAdapter(SQL_Text, cn_connection);

            SqlCommand cmd = new SqlCommand(SQL_Text, cn_connection);
            foreach (object[] param in parameters)
            {
                if (param[1].GetType() == typeof(string))
                {
                    cmd.Parameters.Add(
                        new SqlParameter
                        {
                            ParameterName = param[0].ToString(),
                            Value = param[1].ToString(),
                            SqlDbType = SqlDbType.VarChar,
                            Size = -1
                        });
                }
                else if (param[1].GetType() == typeof(int))
                {
                    cmd.Parameters.Add(
                        new SqlParameter
                        {
                            ParameterName = param[0].ToString(),
                            Value = param[1],
                            SqlDbType = SqlDbType.Int
                        });
                }
            }
            cmd.ExecuteNonQuery();
        }

        public static void Execute_SQL(string SQL_Text)
        {

            SqlConnection cn_connection = Get_DB_Connection();
            SqlCommand cmd_Command = new SqlCommand(SQL_Text, cn_connection);
            cmd_Command.ExecuteNonQuery();
        }
        public static void Close_DB_Connection()
        {

            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string cn_String = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=" + projectDirectory + "\\Database.mdf;Integrated Security=True";

            SqlConnection cn_connection = new SqlConnection(cn_String);

            if (cn_connection.State != ConnectionState.Closed) cn_connection.Close();
        }
    }
}