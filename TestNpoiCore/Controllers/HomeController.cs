using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.XWPF.Extractor;
using NPOI.XWPF.UserModel;

namespace TestNpoiCore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PostearDoc(IFormFile file)
        {
            string texto;
            if (file.ContentType == "application/pdf")
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    byte[] que = ms.ToArray();
                    PdfReader pdfReader = new PdfReader(que);

                    byte[] contenidoPageUno = pdfReader.GetPageContent(1);

                    PrTokeniser tokenizer = new PrTokeniser(new RandomAccessFileOrArray(contenidoPageUno));

                    List<string> strList = new List<string>();
                    texto = String.Empty;

                    while (tokenizer.NextToken())
                    {
                        if (tokenizer.TokenType == PrTokeniser.TK_STRING)
                        {
                            strList.Add(tokenizer.StringValue);
                            texto = texto + tokenizer.StringValue;
                        }
                    }

                    pdfReader.Close();
                }
                
            }
            else
            {
                XWPFDocument doc = new XWPFDocument(file.OpenReadStream());
                XWPFWordExtractor extractor = new XWPFWordExtractor(doc);
                texto = extractor.Text;
            }



            return Json(new {texto});
        }
    }
}