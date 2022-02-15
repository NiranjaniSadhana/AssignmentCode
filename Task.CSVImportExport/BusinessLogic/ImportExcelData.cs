using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Task.CSVImportExport.Models;

namespace Task.CSVImportExport.BusinessLogic
{
    public static class  ImportExcelData
    {
        public static byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }
        public static void UploadXML(string xml)
        {
            
            
            string constr = ConfigurationManager.ConnectionStrings["taskconnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("InsertCustomerXML"))
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@xml", xml);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }

        internal static List<CustomerInfo> FetCustomers()
        {
            var resp = new List<CustomerInfo>();
            string constr = ConfigurationManager.ConnectionStrings["taskconnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Customers"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);


                            foreach (DataRow row in dt.Rows)
                            {
                                var cus = new CustomerInfo
                                {
                                    CustomerName = row["CustomerName"].ToString(),
                                    City = row["City"].ToString(),
                                    State = row["State"].ToString(),
                                    Country = row["Country"].ToString()
                                };
                                resp.Add(cus);   
                                
                            }

                        }
                    }
                }
            }
            return resp;
        }
    }
}