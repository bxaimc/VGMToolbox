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
            string pTableName, string pOrderByField)
        {
            StringBuilder sqlCommand = new StringBuilder();
            
            SQLiteConnection conn;
            SQLiteCommand cmd = new SQLiteCommand();
            SQLiteDataAdapter da;

            DataTable dt = null;
            DataSet ds = new DataSet();

            try
            {
                conn = new SQLiteConnection(String.Format("Data Source={0};Version=3;Read Only=True;", pDbPath));
                conn.Open();

                sqlCommand.AppendFormat("SELECT * FROM {0}", pTableName);

                if (!String.IsNullOrEmpty(pOrderByField))
                {
                    sqlCommand.AppendFormat(" ORDER BY {0}", pOrderByField);
                }

                cmd.Connection = conn;
                cmd.CommandText = sqlCommand.ToString();                
                                
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

        public static DataTable GetSimpleDataItem(string pDbPath,
            string pTableName, string pItemField, string pItemId)
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

                cmd.CommandText = String.Format("SELECT * FROM {0} WHERE {1} = '{2}'",
                    pTableName, pItemField, pItemId);
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
