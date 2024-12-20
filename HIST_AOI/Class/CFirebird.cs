using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIST_AOI.Class
{
    public class CFirebird
    {
        private String m_server;
        private String m_user;
        private String m_pass;
        private String m_route;
        private String m_strConn;
        private FbConnection m_conn;
        private FbCommand m_sqlcmd;
        private int timeout;
        CGeikenResult geikenResult;
        private StringBuilder errorMessages;

        public CFirebird()
        {
            m_user = "SYSDBA";
            m_pass = "masterkey";
            Server = "localhost";
            //Route = "C:\\database\\NEOVIEWDB9.FDB";
            m_strConn = getStrConn();
            geikenResult = new CGeikenResult();
            m_sqlcmd = new FbCommand();
            timeout = 600;
        }
        public string Server { get => m_server; set => m_server = value; }
        public string Route { get => m_route; set => m_route = value; }
        public StringBuilder ErrorMessages { get => errorMessages; set => errorMessages = value; }

        public String getStrConn()
        {
            return ConfigurationManager.ConnectionStrings["Firebird"].ConnectionString;
        }
        
        public bool connect(ref String message)
        {
            bool result = false;
            try
            {
                m_conn = new FbConnection(getStrConn());
                m_sqlcmd.Connection = m_conn;
                m_conn.Open();
                result = true;
            }
            catch(Exception ex)
            {
                message = ex.Message;
                result = false;
            }
            return result;
        }
        public bool closeConn()
        {
            bool result = false;
            try
            {
                m_conn.Close();
                m_conn.Dispose();
                m_conn = null;
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }
        public List<CGeikenResult> getAOIPending(ref String message)
        {
            DataSet resultsQuery = new DataSet();
            List<CGeikenResult> results = new List<CGeikenResult>();
            try
            {

                String sql = "SELECT S.SERIAL_NUMBER, " +
                                    "PSS.NAME AS PROGRAM_NAME, " +
                                    "TR_AOI.SIDE, " +
                                    "TR_AOI.\"RESULT\" AS AOI_RESULT, " +
                                    "TR_AOI.COMPLETED_TIME AS AOI_COMPLETED_TIME, " +
                                    "D_AOI.NAME AS AOI_NAME, " +
                                    "TR_NEO.\"RESULT\" AS NEO_RESULT, " +
                                    "TR_NEO.COMPLETED_TIME AS NEO_COMPLETED_TIME, " +
                                    "D_NEO.NAME AS NEO_NAME, " +
                                    "tu.LOGIN_NAME, " +
                                    "CS.NAME AS REFERENCE, " +
                                    "RC_AOI.CONTENT AS AOI_DEFECT, " +
                                    "RC_NEO.CONTENT AS DEFECT_NAME " +
                                "FROM " +
                                    "SHEETS S " +
                                    "INNER JOIN TASK_RECORDS TR_AOI ON TR_AOI.SHEET_ID = S.ID " +
                                    "INNER JOIN TASK_RECORDS TR_NEO ON TR_AOI.ID = TR_NEO.PARENT " +
                                    "INNER JOIN TURBINE_USER tu ON TR_NEO.USER_ID = tu.USER_ID " +
                                    "INNER JOIN DEVICES D_AOI ON TR_AOI.DEVICE_ID = D_AOI.ID " +
                                    "INNER JOIN DEVICES D_NEO ON TR_NEO.DEVICE_ID = D_NEO.ID " +
                                    "INNER JOIN PROGRAM_SHEET_SPEC PSS ON TR_AOI.PROGRAM_SHEET_SPEC_ID = PSS.ID " +
                                    "LEFT OUTER JOIN TASK_POSITION TP_AOI ON TP_AOI.TASK_ID = TR_AOI.ID " +
                                    "LEFT OUTER JOIN TASK_POSITION TP_NEO ON TP_NEO.TASK_ID = TR_NEO.ID " +
                                    "LEFT OUTER JOIN COMPONENT_SPEC CS ON TP_AOI.COMPONENT_SPEC_ID = CS.ID " +
                                    "LEFT OUTER JOIN RESULT_CODES RC_NEO ON TP_NEO.RESULT_CODE_ID = RC_NEO.ID " +
                                    "LEFT OUTER JOIN RESULT_CODES RC_AOI ON TP_AOI.RESULT_CODE_ID = RC_AOI.ID " +
                                "WHERE " +
                                    "TR_AOI.TYPE_ID = 11 " +
                                    "AND TR_NEO.TYPE_ID = 12 " +
                                    "AND TR_AOI.LAST_AOI = 'T' " +
                                    "AND (RC_NEO.RESULT_TYPE = 'P' OR RC_NEO.RESULT_TYPE = 'F')" +
                                    "AND TR_NEO.COMPLETED_TIME > '2024/05/24 11:00:00' " +
                                    "Order by TR_NEO.COMPLETED_TIME desc";

                if (queryDataSet(sql, ref resultsQuery, ref message))
                {
                    DataTable tmp = resultsQuery.Tables[0];
                    for (int i = 0; i < tmp.Rows.Count; i++)
                    {
                        CGeikenResult row = new CGeikenResult();

                        row.Aoi_defect = tmp.Rows[i]["AOI_DEFECT"].ToString();
                        row.Aoi_name = tmp.Rows[i]["AOI_NAME"].ToString();
                        row.Aoi_result = tmp.Rows[i]["AOI_RESULT"].ToString();
                        row.Cod_ref_cmp = tmp.Rows[i]["REFERENCE"].ToString();
                        row.Date_aoi_test = DateTime.Parse(tmp.Rows[i]["AOI_COMPLETED_TIME"].ToString());
                        row.Date_neo_test = DateTime.Parse(tmp.Rows[i]["NEO_COMPLETED_TIME"].ToString());
                        row.Date_new_test = DateTime.Now;
                        row.Defect_name = tmp.Rows[i]["DEFECT_NAME"].ToString();
                        row.Neo_log_name = tmp.Rows[i]["LOGIN_NAME"].ToString();
                        row.Neo_name = tmp.Rows[i]["NEO_NAME"].ToString();
                        row.Neo_result = tmp.Rows[i]["NEO_RESULT"].ToString();
                        row.New_result_test = "";
                        row.Program_name = tmp.Rows[i]["PROGRAM_NAME"].ToString();
                        row.Serial_number = tmp.Rows[i]["SERIAL_NUMBER"].ToString();
                        row.Side = tmp.Rows[i]["SIDE"].ToString();
                        results.Add(row);

                    }
                }

            }
            catch(Exception ex)
            {
                message = ex.Message;
            }

            return results;

        }

        public bool queryDataSet(string strSql, ref DataSet results, ref String message)
        {
            bool res = false;
            if (!connect(ref message)) return false;

            m_sqlcmd.CommandTimeout = timeout;
            m_sqlcmd.CommandText = strSql;
            results = new DataSet();
            if (!strSql.Equals(""))
            {
                try
                {
                    FbDataAdapter dta = new FbDataAdapter(m_sqlcmd);
                    dta.Fill(results);
                    res = true;
                }
                catch (FbException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        ErrorMessages.Append("Index #" + i + "\n" +
                            "Message: " + ex.Message[i] + "\n");
                    }
                    Debug.Print(ErrorMessages.ToString());
                    Console.WriteLine(ErrorMessages.ToString());
                    res = false;
                }
            }
            closeConn();
            return res;
        }




    }
}
