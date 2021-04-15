using PDFTOHTML.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace PDFTOHTML.Controllers
{
    public class PdfToHtmlController : Controller
    {
        SamplePDFToHTMLEntities _context = new SamplePDFToHTMLEntities();

        // GET: PdfToHtml 
        public ActionResult Index()
        {
            var AllDataList = _context.tblBriefs.OrderByDescending(o => o.CreatedDate).ToList();

            return View(AllDataList);
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

                        var brieffile = "brief_1/";
                        var lastrec = _context.tblBriefs.OrderByDescending(o => o.Id).FirstOrDefault();

                        if (lastrec != null)
                        {
                            brieffile = "brief_" + (lastrec.Id + 1) + "/";
                        }

                        var brieffilepath = Server.MapPath("~/htmlfiles/") + brieffile;

                        if (!Directory.Exists(brieffilepath))
                        {
                            Directory.CreateDirectory(brieffilepath);
                        }

                        string inputPath = brieffilepath + newpdffilename;
                        string outputhtmlPath = brieffilepath + newhtmlfilename;

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

                        #region save data in brief table
                        tblBrief tbl = new tblBrief()
                        {
                            FileName = Path.GetFileNameWithoutExtension(newhtmlfilename),
                            FilePath = "~/htmlfiles/" + brieffile,
                            CreatedDate = DateTime.UtcNow
                        };

                        _context.tblBriefs.Add(tbl);
                        _context.SaveChanges();
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

            var AllDataList = _context.tblBriefs.OrderByDescending(o => o.CreatedDate).ToList();

            return View(AllDataList);
        }

        public ActionResult EditHtmlData(int id)
        {
            var findcurrrec = _context.tblBriefs.Where(w => w.Id == id).FirstOrDefault();

            if (findcurrrec == null || id <= 0)
            {
                return RedirectToAction("Index");
            }

            string filename = string.Format("{0}.{1}", findcurrrec.FileName, "html");
            string filepath = Server.MapPath(findcurrrec.FilePath) + filename;

            if (System.IO.File.Exists(filepath))
            {
                using (WebClient client = new WebClient())
                {
                    string html = client.DownloadString(filepath);

                    ViewBag.ConvertedHtml = html;
                    ViewBag.ConvertedFormatFile = filename;
                    ViewBag.ConvertedFilePath = findcurrrec.FilePath;
                    ViewBag.FileId = findcurrrec.Id;

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

            var findcurrrec = _context.tblBriefs.Where(w => w.FileName == Path.GetFileNameWithoutExtension(filename)).FirstOrDefault();

            if (findcurrrec == null)
            {
                return RedirectToAction("Index");
            }

            string filepath = Server.MapPath(findcurrrec.FilePath) + filename;

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

                var filname = Path.GetFileNameWithoutExtension(request.filename);

                var findcurrrec = _context.tblBriefs.Where(w => w.FileName == filname).FirstOrDefault();

                if (findcurrrec == null)
                {
                    return RedirectToAction("Index");
                }

                string filepath = Server.MapPath(findcurrrec.FilePath) + request.filename;

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
            var filname = Path.GetFileNameWithoutExtension(name);
            var findcurrrec = _context.tblBriefs.Where(w => w.FileName == filname).FirstOrDefault();

            if (findcurrrec == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.convertedfilename = name;
            var savedfilepath = findcurrrec.FilePath;
            savedfilepath = savedfilepath.Substring(1, savedfilepath.Length - 1);
            ViewBag.convertfilepath = savedfilepath;

            return View();
        }

        [HttpPost]
        public ActionResult FileUploadWithAnch()
        {
            string savedfilepath = string.Empty;
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    string filepath = Request.Form["filepath"];
                    int fileid = 0;
                    int.TryParse(Request.Form["FileId"], out fileid);

                    var findcurrrec = _context.tblBriefs.Where(w => w.Id == fileid).FirstOrDefault();

                    if (findcurrrec == null || fileid <= 0)
                    {
                        return Json(new { success = false, message = "Invalid request. Please try again." });
                    }

                    for (int i = 0; i < files.Count; i++)
                    {

                        HttpPostedFileBase file = files[i];

                        var getfileextension = System.IO.Path.GetExtension(file.FileName);

                        if (string.IsNullOrEmpty(getfileextension) || getfileextension != ".pdf")
                        {
                            return Json(new { success = false, message = "Only pdf file allow to convert." });
                        }
                        else
                        {
                            string fname;

                            // Checking for Internet Explorer  
                            if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                            {
                                string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                fname = testfiles[testfiles.Length - 1];
                            }
                            else
                            {
                                fname = file.FileName;
                            }

                            var tickervalue = DateTime.Now.Ticks;
                            var newpdffilename = string.Format("{0}{1}{2}", Path.GetFileNameWithoutExtension(fname), tickervalue, ".pdf");

                            savedfilepath = string.Format("{0}{1}", findcurrrec.FilePath, newpdffilename);
                            // Get the complete folder path and store the file inside it.  
                            fname = Path.Combine(Server.MapPath(findcurrrec.FilePath), newpdffilename);
                            file.SaveAs(fname);
                            savedfilepath = savedfilepath.Substring(1, savedfilepath.Length - 1);
                        }
                    }
                    // Returns message that successfully uploaded  
                    return Json(new { success = true, message = "File Uploaded Successfully!", filename = savedfilepath });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error occurred. Error details: " + ex.Message });
                }
            }
            else
            {
                return Json(new { success = false, message = "No files selected." });
            }
        }

    }
}