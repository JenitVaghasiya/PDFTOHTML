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

                        #region Convert Html using Aspose
                        //// Open the source PDF document
                        //Document pdfDocument = new Document(inputPath);

                        //// Instantiate HTML Save options object
                        //HtmlSaveOptions newOptions = new HtmlSaveOptions();

                        //// Enable option to embed all resources inside the HTML
                        //newOptions.PartsEmbeddingMode = HtmlSaveOptions.PartsEmbeddingModes.EmbedAllIntoHtml;

                        //// This is just optimization for IE and can be omitted
                        //newOptions.LettersPositioningMethod = LettersPositioningMethods.UseEmUnitsAndCompensationOfRoundingErrorsInCss;
                        //newOptions.RasterImagesSavingMode = HtmlSaveOptions.RasterImagesSavingModes.AsEmbeddedPartsOfPngPageBackground;
                        //newOptions.FontSavingMode = HtmlSaveOptions.FontSavingModes.SaveInAllFormats;
                        //newOptions.PartsEmbeddingMode = HtmlSaveOptions.PartsEmbeddingModes.EmbedAllIntoHtml;
                        //newOptions.LettersPositioningMethod = LettersPositioningMethods.UseEmUnitsAndCompensationOfRoundingErrorsInCss;
                        //newOptions.SplitIntoPages = false;// force write HTMLs of all pages into one output document


                        //newOptions.ConvertMarkedContentToLayers = true;

                        //// Save the file into MS document format
                        //pdfDocument.Save(outputhtmlPath, newOptions);
                        #endregion

                        #region Convert Html using PdfToHtml
                        //System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

                        ////Set the PDF File Path and HTML File Path as arguments.
                        //startInfo.Arguments = string.Format("{0} {1}", inputPath, outputhtmlPath);

                        ////Set the Path of the PdfToHtml exe file.
                        //startInfo.FileName = Server.MapPath("~/pdftohtml/pdftohtml.exe");

                        ////Hide the Command window.
                        //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        //startInfo.CreateNoWindow = true;

                        ////Execute the PdfToHtml exe file.
                        //using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(startInfo))
                        //{
                        //    process.WaitForExit();
                        //}
                        #endregion

                        #region Convert Html using Sautinsoft

                        SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();

                        //// Path (must exist) to a directory to store images after converting. Notice also to the property "ImageSubFolder".
                        //f.HtmlOptions.ImageFolder = Path.GetDirectoryName(outputhtmlPath);

                        //// A folder (will be created by the component) without any drive letters, only the folder as "myfolder".
                        //f.HtmlOptions.ImageSubFolder = String.Format("{0}_images", Path.GetFileNameWithoutExtension(inputPath));

                        // Auto - the same image format as in the source PDF;
                        // 'Jpeg' to make the document size less; 
                        // 'PNG' to keep the highest quality, but the highest size too.
                        f.EmbeddedImagesFormat = SautinSoft.PdfFocus.eImageFormat.Auto;

                        // How to store images: Inside HTML document as base64 images or as linked separate image files.
                        f.HtmlOptions.IncludeImageInHtml = true;

                        f.HtmlOptions.InlineCSS = true;

                        f.HtmlOptions.RenderMode = SautinSoft.PdfFocus.CHtmlOptions.eHtmlRenderMode.Fixed;

                        f.HtmlOptions.UseNumericCharacterReference = true;

                        f.HtmlOptions.KeepCharScaleAndSpacing = true;
                         
                        // Set <title>...</title>
                        //f.HtmlOptions.Title = String.Format("This HTML was converted from {0}.", Path.GetFileName(inputPath));

                        f.OpenPdf(inputPath);

                        if (f.PageCount > 0)
                        {
                            int res = f.ToHtml(outputhtmlPath);

                            // Open the result for demonstration purposes.
                            //if (res == 0)
                            //    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(outputhtmlPath) { UseShellExecute = true });
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