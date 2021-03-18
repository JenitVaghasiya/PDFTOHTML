using PDFTOHTML.Models;
using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PDFTOHTML.Controllers
{
    public class PdfToHtmlController : Controller
    {
        // GET: PdfToHtml 
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var getfileextension = System.IO.Path.GetExtension(file.FileName);

                if (string.IsNullOrEmpty(getfileextension) || getfileextension != ".pdf")
                {
                    ViewBag.Message = "Only pdf file allow to convert.";
                }
                else
                {
                    try
                    {

                        //Get the File Name. Remove space characters from File Name.
                        string fileName = file.FileName.Replace(" ", string.Empty);

                        var filename = System.IO.Path.GetFileNameWithoutExtension(fileName);

                        var tickervalue = DateTime.Now.Ticks;
                        var newpdffilename = string.Format("{0}{1}{2}", filename, tickervalue, ".pdf");
                        var newhtmlfilename = string.Format("{0}{1}{2}", filename, tickervalue, ".html");

                        //Save the PDF file.
                        string inputPath = Server.MapPath("~/htmlfiles/") + newpdffilename;
                        string outputhtmlPath = Server.MapPath("~/htmlfiles/") + newhtmlfilename;

                        file.SaveAs(inputPath);

                        #region Convert Html using Sautinsoft

                        SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();

                        f.EmbeddedImagesFormat = SautinSoft.PdfFocus.eImageFormat.Auto;
                        f.HtmlOptions.IncludeImageInHtml = true;
                        f.HtmlOptions.InlineCSS = true;
                        f.HtmlOptions.RenderMode = SautinSoft.PdfFocus.CHtmlOptions.eHtmlRenderMode.Fixed;
                        f.HtmlOptions.UseNumericCharacterReference = true;
                        f.HtmlOptions.KeepCharScaleAndSpacing = true;
                        f.OpenPdf(inputPath);

                        if (f.PageCount > 0)
                        {
                            int res = f.ToHtml(outputhtmlPath);
                        }

                        #endregion

                        ViewBag.Message = "File uploaded and converted";
                        ViewBag.generatedfile = newhtmlfilename;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    }
                }
            }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return View();
        }

        public ActionResult EditHtmlData(string filename)
        {
            if (string.IsNullOrEmpty(filename) || string.IsNullOrWhiteSpace(filename))
            {
                return RedirectToAction("Index");
            }

            string filepath = Server.MapPath("~/htmlfiles/") + filename;

            if (System.IO.File.Exists(filepath))
            {
                using (WebClient client = new WebClient())
                {
                    string html = client.DownloadString(filepath);

                    ViewBag.ConvertedHtml = html;
                    ViewBag.ConvertedFormatFile = filename;
                }
            }
            else
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        public ActionResult EditHtmlEditorData(string filename)
        {
            if (string.IsNullOrEmpty(filename) || string.IsNullOrWhiteSpace(filename))
            {
                return RedirectToAction("Index");
            }

            string filepath = Server.MapPath("~/htmlfiles/") + filename;

            using (WebClient client = new WebClient())
            {
                string html = client.DownloadString(filepath);

                ViewBag.ConvertedHtml = html;
                ViewBag.ConvertedFormatFile = filename;
            }

            return View();
        }

        public ActionResult SaveEditedHtml(SaveHtmlEditor request)
        {
            var responseobject = new object();

            try
            {
                if (string.IsNullOrEmpty(request.filename) || string.IsNullOrWhiteSpace(request.filename))
                {
                    return RedirectToAction("Index");
                }

                string filepath = Server.MapPath("~/htmlfiles/") + request.filename;

                using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(request.html);

                        sw.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                responseobject = new { success = true, error = ex.Message };
            }

            return Json(responseobject, JsonRequestBehavior.AllowGet);
        }

        public ActionResult successfullyconverted(string name)
        {
            ViewBag.convertedfilename = name;

            return View();
        }

    }
}