using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViKing.Engine;

namespace Parser
{
    class GoogleSearcher
    {
        public Queue Sites { get; set; }

        GProxy proxy = new GProxy();
        HTTPHeaderItem[] header = new HTTPHeaderItem[1]{
            new HTTPHeaderItem("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/38.0.2125.111 Safari/537.36 OPR/25.0.1614.68")
        };
        CookieCollection cook = new CookieCollection();
        public event Change ChangeProxy;
        public event Show onDataGet;

        public event ExceptionHandler onExceptionCatch;

        public int countSites = 0;
        Proxy p;
        public GoogleSearcher()
        {
                 Sites = new Queue();
        }

        public bool needRun = true;
        public void Search(string query)
        { 
                   string s = getSite("https://www.google.ru/search?client=opera&q=" + query + "&sourceid=opera&ie=UTF-8&oe=UTF-8");
                   SearchLinks(s);

                   int i = 20;
                   while (needRun)
                   {
                       if (i > 990)
                       {
                           break;
                       }

                       try
                       {

                           int now = Sites.Count;
                           Thread.Sleep(5000);
                           string next_l = "https://www.google.ru/search?client=opera&q=" + query + "&sourceid=opera&ie=UTF-8&oe=UTF-8&start=" + i;
                           break;
                           i += 10;
                           string text = getSite(next_l);
                           SearchLinks(text);
                           if (Sites.Count - now == 0)
                           {
                               break;
                           }
                       }
                       catch(Exception ex)
                       {
                           if (this.onExceptionCatch != null)
                           {
                               onExceptionCatch(this, new ExceptionArgs(ex.Message, "Search"));
                           }
                       }
                   }
                  
                foreach (string site in Sites)
                {
                    if (!needRun)
                    {
                        return;
                    }

                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    
                    try
                    {
                        string html = getHtml(site);
                        int length = html.Length;

                       
                        List<string> moreInfo = getMoreLinks(html, site);
                        if (html != "")
                        {
                           
                                doc.LoadHtml(html);
                            string str = doc.DocumentNode.InnerText;
                            SortedSet<string> phones = Searcher.findPhone(str);
                            SortedSet<string> mails = Searcher.findMail(str);
                            SortedSet<string> skype = Searcher.findSkype(html);
                            ContactData data = new ContactData();
                            data.phones = generateString(phones) + moreInfo[2];
                            data.mails = generateString(mails) + moreInfo[1];
                            data.link = site;
                            data.query = HttpUtility.UrlDecode(query);
                            data.skypes = generateString(skype) + moreInfo[0];
                            data.type = ParserType.Google;
                            onDataGet(data);
                            Thread.Sleep(1000);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (this.onExceptionCatch != null)
                        {
                            onExceptionCatch(this, new ExceptionArgs(ex.Message, "Search"));
                        }
                    }

                }
                Sites.Clear();
            
        }

        public List<string> getMoreLinks(string html,string site)
        {
           
            Uri s = new Uri(site);
            List<string> list = new List<string>();
            
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);
                try
                {
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//a"))
                    {
                        if (node.Attributes["href"] != null)
                        {
                            list.Add(node.Attributes["href"].Value);
                        }
                    }
                }
                 catch(Exception ex)
                       {
                           if (this.onExceptionCatch != null)
                           {
                               onExceptionCatch(this, new ExceptionArgs(ex.Message, "getMoreLinks"));
                           }
                       }
                string skype = ""; string mail = ""; string phone = "";
                SortedSet<string> ph = new SortedSet<string>();
                SortedSet<string> sk = new SortedSet<string>();
                SortedSet<string> ma = new SortedSet<string>();
                foreach (string part in list)
                {
                    try
                    {
                        string link = "";
                        if (part == "")
                        {
                            continue;
                        }
                        if (part[0] == '/')
                        {
                            link = "http://" + s.Host + part;
                        }
                        if (part[0] != '/')
                        {
                            link = "http://" + s.Host + "/" + part;
                        }
                        if (part.StartsWith("http"))
                        {
                            link = part;
                        }
                        if (part.EndsWith(".jpg") || part.EndsWith(".jpeg") || part.EndsWith(".png") ||
                            part.EndsWith(".wav") || part.EndsWith(".mp3") || part.EndsWith(".ogg") ||
                            part.EndsWith(".mp4") || part.EndsWith(".mpeg4") || part.EndsWith(".xml") || part.EndsWith(".doc")
                            || part.EndsWith(".docx") || part.EndsWith(".xls") || part.EndsWith(".pdf") || part.EndsWith(".zip") || part.EndsWith(".rar")
                            || part.EndsWith(".webm") || part.EndsWith(".gif") || part.EndsWith(".tiff"))
                            continue;
                        string xHtml = getHtml(link);


                        doc.LoadHtml(xHtml);

                        string str = doc.DocumentNode.InnerText;
                        SortedSet<string> phones = Searcher.findPhone(str);
                        SortedSet<string> mails = Searcher.findMail(str);
                        SortedSet<string> skypes = Searcher.findSkype(xHtml);

                        foreach (string ss in phones)
                        {
                            ph.Add(ss);
                        }
                        foreach (string ss in skypes)
                        {
                            sk.Add(ss);
                        }
                        foreach (string ss in mails)
                        {
                            ma.Add(ss);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (this.onExceptionCatch != null)
                        {
                            onExceptionCatch(this, new ExceptionArgs(ex.Message, "getMoreLinks"));
                        }
                    }
                }
                skype += generateString(sk);
                mail += generateString(ma);
                phone += generateString(ph);
                return new List<string>() { skype, mail, phone };
           
        }

        public string generateString(SortedSet<string> list)
        {
            string s = "";
            foreach (string ss in list)
            {
                s += ss + " ,";
            }
            return s;
        }

        public string getHtml(string link) {

            try
            {
                var resp = VkRequest.Request(link, additionalHeaders: header,FollowRedirects:true);
                return resp.ContentUTF8;
            }
            catch (Exception ex)
            {
                if (this.onExceptionCatch != null)
                {
                    onExceptionCatch(this, new ExceptionArgs(ex.Message, "getMoreLinks"));
                }
                return "";
            }
        
        }

        public string getSite(string link)
        {
            VkResponse rest;
            try
            {
                if (p == null)
                {
                    p = proxy.getProxy();
                }

              

                rest = VkRequest.Request(link, cookies: cook, additionalHeaders: header, proxy: p, FollowRedirects: true);
            }
            catch
            {
                    p = proxy.getProxy();
                    return getSite(link);
            }
            if (!rest.ContentUTF8.Contains("'captcha'"))
            { 
                return rest.ContentUTF8;
            }
            else
            {
                p = proxy.getProxy();
                return getSite(link);
            }
        }

        public void SearchLinks(string html)
        {
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var links = doc.DocumentNode.Descendants("h3");
                foreach (var temp in links)
                {
                    if (temp.Attributes["class"] != null && temp.Attributes["class"].Value == "r")
                    {
                        HtmlAgilityPack.HtmlDocument doc2 = new HtmlAgilityPack.HtmlDocument();
                        doc2.LoadHtml(temp.InnerHtml);
                        var next = doc2.DocumentNode.Descendants("a");
                        foreach (var temp2 in next)
                        {
                            if (temp2.Attributes["href"] != null)
                            {
                                if (!Sites.Contains(temp2.Attributes["href"].Value))
                                {
                                    Sites.Enqueue(temp2.Attributes["href"].Value);
                                    countSites++;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.onExceptionCatch != null)
                {
                    onExceptionCatch(this, new ExceptionArgs(ex.Message, "SearchLinks"));
                }
            }

        }

    }
}
