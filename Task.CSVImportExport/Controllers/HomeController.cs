
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using Task.CSVImportExport.BusinessLogic;
using Task.CSVImportExport.Models;

namespace Task.CSVImportExport.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Index(HttpPostedFileBase FileUpload)
		{

			if (!ModelState.IsValid)
			{
				return View();
			}
            List<CustomerInfo> customers = new List<CustomerInfo>();
            string filePath = string.Empty;
            if (FileUpload != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(FileUpload.FileName);
                FileUpload.SaveAs(filePath);

                //Read the contents of CSV file.
                string csvData = System.IO.File.ReadAllText(filePath);
                
                int i = 0;
                //Execute a loop over the rows.
                foreach (string row in csvData.Split('\n'))
                {
                    i++;
                    if (i == 1) continue;
                    if (!string.IsNullOrEmpty(row))
                    {
                        customers.Add(new CustomerInfo
                        {
                            CustomerName = row.Split(',')[0],
                            City = row.Split(',')[1],
                            State= row.Split(',')[2],
                            Country = row.Split(',')[3]
                        });
                    }
                }
                var xml = Serialize(customers);
                try
                {
                    ImportExcelData.UploadXML(xml);
                    ViewBag.Success = true;
                }
                catch(Exception)
                {
                    ViewBag.Failure = true;
                }
                
            }
            
            return View();
        }
        public static string Serialize(object dataToSerialize)
        {
            if (dataToSerialize == null) return null;

            using (StringWriter stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(dataToSerialize.GetType());
                serializer.Serialize(stringwriter, dataToSerialize);
                return stringwriter.ToString();
            }
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public void ExportExcel()
        {


            try
            {
                // Initialization.  
                string filePath = System.IO.Path.GetDirectoryName(
      System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", "");
                filePath = filePath + @"\File\TaskUpload.csv";

               

                List<CustomerInfo> customers = ImportExcelData.FetCustomers();
                using (var stream = System.IO.File.CreateText(filePath))
                {//adding headers
                    stream.WriteLine("CustomerName,City,State,Country");
                    foreach (var item in customers)
                    {
                        string name = item.CustomerName.ToString();
                        string city = item.City.ToString();
                        string state = item.State.ToString();
                        string country = item.Country.ToString();
                        string csvRow = string.Format("{0},{1},{2},{3}", name, city, state, country);

                        stream.WriteLine(csvRow);
                    }
                    
                }
                // Info.  
                var filebytes = ImportExcelData.GetFile(filePath);
                if (filebytes != null)
                {
                    ClearResponseAndOpenFile(filebytes, "TaskUpload");
                }
            }
            catch (Exception ex)
            {
                // Info  
                Console.Write(ex);
            }

        }
        public void DownloadFile()
        {
           

            try
            {
                // Initialization.  
                string filePath = System.IO.Path.GetDirectoryName(
      System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\","");
                filePath = filePath + @"\File\TaskUpload.csv";

                // Info.  
                var filebytes = ImportExcelData.GetFile(filePath);
                if(filebytes!=null)
                {
                    ClearResponseAndOpenFile(filebytes, "TaskUpload");
                }
            }
            catch (Exception ex)
            {
                // Info  
                Console.Write(ex);
            }
                
        }
       
        private void ClearResponseAndOpenFile(Byte[] bytes, string filename)
        {

            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/CSV";
            Response.AddHeader("content-disposition", "attachment; filename=\"" + filename + ".csv\"");
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Charset = "utf-8";
            //Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.BinaryWrite(bytes);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            Response.End();
            // ms.Close();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}