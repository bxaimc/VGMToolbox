using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace VGMToolbox.dbutil
{
    public class SqlLiteUtil
    {
        public static DataTable GetSimpleDataTable(string pDbPath,
            string pTableName)
        {
            SQLiteConnection conn;
            SQLiteCommand cmd = new SQLiteCommand();
            SQLiteDataAdapter da;

            DataTable dt = null;
            DataSet ds = new DataSet();

            try
            {
                conn = new SQLiteConnection(String.Format("Data Source={0};Version=3;Read Only=True;", pDbPath));
                conn.Open();

                cmd.CommandText = String.Format("SELECT * FROM {0}", pTableName);
                cmd.Connection = conn;

                da = new SQLiteDataAdapter(cmd);
                ds.Reset();
                da.Fill(ds);
                dt = ds.Tables[0];

                conn.Close();

                return dt;
            }
            catch (Exception ex)
            {
                return null;                
            }
        }
    }
}
