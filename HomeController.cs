using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Text;
using System.Data;
using AfternoonTea.Models;
using HtmlAgilityPack;
namespace AfternoonTea.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            //string content = GetWebContent();
            string strUrl = "http://rate.bot.com.tw/Pages/Static/UIP003.zh-TW.htm";
            //Html AgilityPack
            WebClient client = new WebClient();
            currency currency = new currency();
            using (MemoryStream ms = new MemoryStream(client.DownloadData(strUrl)))
            {
                HtmlDocument doc = new HtmlDocument();
                doc.Load(ms, Encoding.UTF8);
                // 取得所有符合條件的nodes
                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//table[2]/tr[@class='color0' or @class='color1' ]");///tr[contains(@class='color0')] td[@class='titleLeft']

                currency.dtCurrency = new DataTable();
                currency.dtCurrency.Columns.Add("幣別");
                currency.dtCurrency.Columns.Add("買入");
                currency.dtCurrency.Columns.Add("賣出");

                currency.NewTime = doc.DocumentNode.SelectSingleNode("//div[@class='entry-content']/table/tr/td[2]").InnerText.Trim().Replace("\r\n", "").Replace("&nbsp;","");
               
                List<CheckBoxes> CurrencyType = new List<CheckBoxes>();
                foreach (HtmlNode node in nodes)
                {
                    
                    DataRow r = currency.dtCurrency.NewRow();
                    r[0] = node.SelectSingleNode("td[@class='titleLeft']").InnerText.Replace("&nbsp;","");
                   
                    if (r[0].ToString() == "美金 (USD)" || r[0].ToString() == "英鎊 (GBP)" || r[0].ToString() == "日圓 (JPY)" || r[0].ToString() == "人民幣(CNY)")
                    {
                        CurrencyType.Add(new CheckBoxes()
                        {
                            Text = r[0].ToString(),
                            Checked = true
                        });
                    }else
                    {
                        CurrencyType.Add(new CheckBoxes()
                        {
                            Text = r[0].ToString(),
                            Checked = false
                        });
                    }
                    r[1] = node.SelectSingleNode("td[3]").InnerText;
                    r[2] = node.SelectSingleNode("td[4]").InnerText;
                    

                    currency.dtCurrency.Rows.Add(r);
                    currency.CurrencyType = CurrencyType;
                    //currency.Add(node.OuterHtml.TrimStart());
                    //switch (node.OuterHtml.TrimStart())
                    //{
                    //    case "美金USD":
                    //        currency.Add(node.OuterHtml.TrimStart(), true);
                    //        break;
                    //        case ""
                    //}



                }
              
            }
            //ViewBag.content = content;

           

            return View(currency);
        }

     
        [HttpGet]
        public ActionResult Blog(string url )//string title="", string copy="", string content=""
        {
            string title = "";
            string copy = "";
            string content = "";
            if(url=="")
            {
                url = "http://duuro.blog.fc2.com/blog-entry-553.html";
            }
            string strUrl = url;
            WebClient client = new WebClient();
            using (MemoryStream ms = new MemoryStream(client.DownloadData(strUrl)))
            {
                HtmlDocument doc = new HtmlDocument();
                doc.Load(ms, Encoding.UTF8);
                HtmlNode noteTitle = doc.DocumentNode.SelectSingleNode("//div[@class='content']/h2[@class='entry_header']");
                title = noteTitle.InnerText;

                HtmlNode noteCopy = doc.DocumentNode.SelectSingleNode("//div[@class='content']/div[@class='entry_body']/span");
                copy = noteCopy.InnerHtml;

                HtmlNode noteContent = doc.DocumentNode.SelectSingleNode("//div[@id='more']/span");
                content = noteContent.InnerHtml;

            }
            ViewBag.book = title;
            ViewBag.copy = copy;
            ViewBag.content = content;

            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetBlog(string url = "http://duuro.blog.fc2.com/blog-entry-553.html")
        {
            string title = "";
            string copy = "";
            string content = "";
            string strUrl = url;
            WebClient client = new WebClient();
            using (MemoryStream ms = new MemoryStream(client.DownloadData(strUrl)))
            {
                HtmlDocument doc = new HtmlDocument();
                doc.Load(ms, Encoding.UTF8);
                HtmlNode noteTitle = doc.DocumentNode.SelectSingleNode("//div[@class='content']/h2[@class='entry_header']");
                title = noteTitle.InnerText;

                HtmlNode noteCopy = doc.DocumentNode.SelectSingleNode("//div[@class='content']/div[@class='entry_body']/span");
                copy = noteCopy.InnerHtml;

                HtmlNode noteContent = doc.DocumentNode.SelectSingleNode("//div[@id='more']/span");
                content = noteContent.InnerHtml;

            }

            //ViewBag.book = title;
            //ViewBag.copy = copy;
            //ViewBag.content = content;
            //return new JsonResult
            //{
            //    Data = new { title = title, copy = copy,content=content }
            //};
            return View(title,copy,content);
        }

        [HttpPost]
        public ActionResult Donload(string source = "http://duuro.blog.fc2.com/blog-entry-553.html")
        {
            string fileName = "";
            string content = "";
            WebClient client = new WebClient();
            
            using (MemoryStream ms = new MemoryStream(client.DownloadData(source)))
            {
                HtmlDocument doc = new HtmlDocument();
                doc.Load(ms, Encoding.UTF8);
                HtmlNode noteTitle = doc.DocumentNode.SelectSingleNode("//div[@class='content']/h2[@class='entry_header']");
                fileName = noteTitle.InnerText;

                HtmlNode noteCopy = doc.DocumentNode.SelectSingleNode("//div[@class='content']/div[@class='entry_body']/span");
                content = noteCopy.InnerHtml.Replace("<br>","\r\n") + "<hr/>";

                HtmlNode noteContent = doc.DocumentNode.SelectSingleNode("//div[@id='more']/span");
                content += noteContent.InnerHtml.Replace("<br>", "\r\n");

                
            }

            Response.AppendHeader("Accept-Language", "zh-tw");
            Response.AppendHeader("content-disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(fileName + ".txt", System.Text.Encoding.UTF8));
            Response.ContentType = "application/vnd.Text";
            Response.Write(content);
            Response.End();
            return View();
        }

        
    }
    public class CheckBoxListInfo
    {
        public string Value { get; private set; }
        public string DisplayText { get; private set; }
        public bool IsChecked { get; private set; }
        public CheckBoxListInfo(string value, string displayText, bool isChecked)
        {
            this.Value = value;
            this.DisplayText = displayText;
            this.IsChecked = isChecked;
        }
    }
}