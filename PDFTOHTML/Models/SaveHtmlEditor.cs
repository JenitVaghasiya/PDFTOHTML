using System.Web.Mvc;

namespace PDFTOHTML.Models
{
    public class SaveHtmlEditor
    {
        [AllowHtml]
        public string html { get; set; }
        public string filename { get; set; }
    }
}