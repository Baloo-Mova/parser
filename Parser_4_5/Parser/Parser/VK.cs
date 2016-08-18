using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using ViKing.Engine;
using HtmlAgilityPack;
using System.Collections;
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Parser
{
    public delegate void Show(object e);
    public delegate void Change();
    class VK
    {
        private string login;
        private string password;
        private int CountSearched;

        public bool loggined = false;
        public int Count { get; set; }
        public bool needRun = true;

        public event Show onCountKnown;
        public event Show onContactRecieve;
        public event ExceptionHandler onExceptionCatch;
        public Queue Groups { get; set; }
        public Queue PeopleVK { get; set; }

        private CookieCollection coll = new CookieCollection();


        HTTPHeaderItem[] header = new HTTPHeaderItem[1]{
            new HTTPHeaderItem("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.155 Safari/537.36 OPR/31.0.1889.174")
        };
        public string query = "";



            
        public VK(string l,string p)
        {
            this.login = l;
            this.password = p;
            Groups = new Queue();
            PeopleVK = new Queue();
          
        }

        
        public bool Login()
        {
            try
            {
                var resp = VkRequest.Request("https://vk.com", cookies: coll, FollowRedirects: true, additionalHeaders: header);

                string ip_h = "";
                string lg_h = "";
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(resp.ContentUTF8);
                try
                {
                    var inputs = from input in doc.DocumentNode.Descendants("input")
                                 where input.Attributes["name"].Value == "ip_h"
                                 select input;

                    foreach (var input in inputs)
                    {
                        ip_h = input.Attributes["value"].Value;
                    }
                    }
                catch { }
                try{
                    var inp = from input in doc.DocumentNode.Descendants("input")
                              where input.Attributes["name"].Value == "lg_h"
                              select input;
                    foreach (var ins in inp)
                    {
                        lg_h = ins.Attributes["value"].Value;
                    }
                }
                catch { }

                string post = "act=login&role=al_frame&expire=&captcha_sid=&captcha_key=&_origin=https%3A%2F%2Fvk.com&lg_h="+lg_h+"&ip_h=" + ip_h + "&email=" + login + "&pass=" + password;
                var respns = VkRequest.Request("https://login.vk.com/?act=login", post, "POST", FollowRedirects: true, cookies: coll, additionalHeaders: header);

                if (respns.Content.Contains("onLoginFailed"))
                {
                    loggined = false;
                    return false;
                }


                var resp1 = VkRequest.Request("https://vk.com", cookies: coll, FollowRedirects: true, additionalHeaders: header);

                if (resp1.Content.Contains("Чтобы подтвердить, что Вы действительно являетесь владельцем страницы"))
                {
                    string start = resp1.Content.Substring(resp1.Content.IndexOf("hash: '") + 7, 18);
                    string code = generate(this.login);
                    var newResp = VkRequest.Request("http://vk.com/login.php?act=security_check", "al=1&al_page=3&code=" + code + "&hash=" + start + "&to=", method: "POST", cookies: coll, additionalHeaders: header, FollowRedirects: true);


                    if (!newResp.Content.Contains("У вас осталось"))
                    {
                        loggined = true;
                        return true;
                    }
                    else
                    {
                        code = generate2(this.login);
                        newResp = VkRequest.Request("http://vk.com/login.php?act=security_check", "al=1&al_page=4&code=" + code + "&hash=" + start + "&to=", method: "POST", cookies: coll, additionalHeaders: header, FollowRedirects: true);

                    }

                }

            }
            catch { }
            loggined = true;
            return true;

        }


        private string generate(string inn)
        {
            string d = "";
            string answer = "";
            try
            {
                for (int i = inn.Length; i > 0; i--)
                {
                    if (i < inn.Length - 1 && d.Length < 8)
                    {
                        d += inn[i];
                    }
                }

                for (int i = d.Length - 1; i > 0; i--)
                {
                    answer += d[i];
                }
            }
            catch { }
            return answer;
        }

        private string generate2(string inn)
        {
            string d = "";
            string answer = "";
            try
            {
                for (int i = inn.Length; i > 0; i--)
                {
                    if (i < inn.Length - 1 && d.Length < 9)
                    {
                        d += inn[i];
                    }
                }

                for (int i = d.Length - 1; i > 0; i--)
                {
                    answer += d[i];
                }
            }
            catch { }
            return answer;
        }

        public void ParsePersons()
        {
            try
            {
                foreach (string link in PeopleVK)
                {
                    if (!needRun)
                    { return; }

                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    string sss = getPage(link);
                    try
                    {
                        doc.LoadHtml(sss);
                    }
                    catch { }

                    string s = doc.DocumentNode.InnerText;

                    SortedSet<string> phones = Searcher.findPhone(s);
                    SortedSet<string> mails = Searcher.findMail(s);
                    SortedSet<string> skype = Searcher.findSkype(sss);

                    ContactData data = new ContactData();
                    data.phones = generateString(phones);
                    data.mails = generateString(mails);
                    data.FIO = Searcher.getFIO(sss);
                    data.city = Searcher.getCity(sss);
                    data.link = link;
                    data.query = query;
                    data.skypes = generateString(skype);
                    data.type = ParserType.VK;
                    Thread.Sleep(2500);
                    onContactRecieve(data);
                    Count++;
                } 

                PeopleVK.Clear();
            }
            catch { }
        }

        public void Parse(List<string> Ppl)
        {
            try
            {
                foreach (string link in Ppl)
                {
                    if (!needRun)
                    { return; }
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    string sss = getPage(link);
                    try
                    {
                        doc.LoadHtml(sss);
                    }
                    catch { }

                    string s = doc.DocumentNode.InnerText;

                    SortedSet<string> phones = Searcher.findPhone(s);
                    SortedSet<string> mails = Searcher.findMail(s);
                    SortedSet<string> skype = Searcher.findSkype(sss);

                    ContactData data = new ContactData();
                    data.phones = generateString(phones);
                    data.mails = generateString(mails);
                    data.FIO = Searcher.getFIO(sss);
                    data.city = Searcher.getCity(sss);
                    data.link = link;
                    data.query = query;
                    data.skypes = generateString(skype);
                    data.type = ParserType.VK;
                    Thread.Sleep(2500);
                    onContactRecieve(data);
                    Count++;
                }

            }
            catch { }
        }

        public void parseGroups()
        {
            try
            {
                HTTPHeaderItem[] headers = new HTTPHeaderItem[2];
                Array.Copy(header, headers, 1);
                headers[1] = new HTTPHeaderItem("X-Requested-With", "XMLHttpRequest");

                string html = ""; string search = "";

                foreach (string link in Groups)
                {

                    int i = 0; int count = 0;
                    try
                    {
                        if (!needRun)
                        { return; }
                        //Получение данных о группе
                        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                        string sss = getPage(link);
                        try
                        {
                            doc.LoadHtml(sss);
                        }
                        catch { }

                        string s = doc.DocumentNode.InnerText;

                        SortedSet<string> phones = Searcher.findPhone(s);
                        SortedSet<string> mails = Searcher.findMail(s);
                        SortedSet<string> skype = Searcher.findSkype(sss);


                        ContactData data = new ContactData();
                        data.phones = generateString(phones);
                        data.mails = generateString(mails);
                        data.link = link;
                        data.query = query;
                        data.skypes = generateString(skype);
                        data.type = ParserType.VK;
                        onContactRecieve(data);
                        Count++;
                        Thread.Sleep(1000);

                        //Получение людей из группы


                        html = getPage(link);
                        search = html.Substring(html.IndexOf("&c[group]=") + 10, 10);
                        search = search.Substring(0, search.IndexOf("\""));
                        string search_link = "https://vk.com/search?c[section]=people&c[group]=" + search;
                        html = getPage(search_link);

                        count = getResultCountForPeoples(html);
                        getPersons(html);

                        ParsePersons();

                        i = 10;
                    }
                    catch
                    {

                    }
                    while (i <= count)
                    {
                        i += 20;
                        if (i > 1000)
                        {
                            break;
                        }
                        try
                        {
                            if (!needRun)
                            { return; }
                            string data_s = "al=1&c%5Bgroup%5D=" + search + "&c%5Bname%5D=1&c%5Bphoto%5D=1&c%5Bsection%5D=people&offset=" + i;
                            html = VkRequest.Request("https://vk.com/al_search.php", data_s, "POST", FollowRedirects: true, cookies: coll, additionalHeaders: headers).Content;
                            Thread.Sleep(1000);

                        }
                        catch { }
                        getPersons(html);
                        ParsePersons();
                    }



                }

                Groups.Clear();
            }
            catch { }
        }

        public void searchAllInGroup(string link)
        {
            try
            {
                string html = "";
                string search_link = ""; string search = "";
                HTTPHeaderItem[] headers = new HTTPHeaderItem[2];
                Array.Copy(header, headers, 1);
                headers[1] = new HTTPHeaderItem("X-Requested-With", "XMLHttpRequest");
                try
                {
                    html = getPage(link);
                    search = html.Substring(html.IndexOf("&c[group]=") + 10, 10);
                    search = search.Substring(0, search.IndexOf("\""));
                    search_link = "https://vk.com/search?c[section]=people&c[group]=" + search;
                }
                catch { return; }
                html = getPage(search_link);

                int count = getResultCountForPeoples(html);
                List<string> list = getPersonsForOne(html);

                Parse(list);

                int i = 20;

                while (i <= count)
                {
                    i += 20;
                    if (i > 1000)
                    {
                        break;
                    }
                    try
                    {
                        if (!needRun)
                        { return; }
                        string data_s = "al=1&c%5Bgroup%5D=" + search + "&c%5Bname%5D=1&c%5Bphoto%5D=1&c%5Bsection%5D=people&offset=" + i;
                        html = VkRequest.Request("https://vk.com/al_search.php", data_s, "POST", FollowRedirects: true, cookies: coll, additionalHeaders: headers).Content;
                        Thread.Sleep(1200);

                    }
                    catch { }
                    List<string> lt = getPersonsForOne(html);

                    Parse(lt);
                }
            }
            catch { }
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
        public void SearchInGroups(string query)
        {
            
            if (!needRun)
                { return; }

                this.query = query;

                string data = "https://vk.com/search?c%5Bq%5D=" + System.Web.HttpUtility.UrlEncode(query) + "&c%5Bsection%5D=communities";
                HTTPHeaderItem[] headers = new HTTPHeaderItem[2];
                Array.Copy(header, headers, 1);
                headers[1] = new HTTPHeaderItem("X-Requested-With", "XMLHttpRequest");

                string answer = ""; 

                try
                {
                    answer = VkRequest.Request(data, FollowRedirects: true, cookies: coll, additionalHeaders: headers).Content;
                }
                catch
                {

                }

                CountSearched = getResultCount(answer);

                getGroups(answer);
                parseGroups();

                int i = 20;

                while (i <= CountSearched)
                {
                    i += 10;

                    if (i > 990)
                    {
                        break;
                    }

                    try
                    {
                        if (!needRun)
                        { return; }
                        data = "al=1&c%5Bq%5D=" + query + "&c%5Bsection%5D=communities&future=1&offset=" + i;
                        answer = VkRequest.Request("https://vk.com/al_search.php", data, "POST", FollowRedirects: true, cookies: coll, additionalHeaders: headers).Content;
                        Thread.Sleep(1000);

                    }
                    catch
                    {


                    }

                    getGroups(answer);
                    parseGroups();
                }
          
        }


        public string getPage(string url)
        {
            try
            {
                return VkRequest.Request(url, cookies: coll).Content;
            }
            catch {
                Thread.Sleep(1000);
                try
                {
                    return VkRequest.Request(url, cookies: coll).Content;
                }
                catch { Thread.Sleep(2000); return VkRequest.Request(url, cookies: coll).Content; }
            }
        }

        private int getResultCount(string html){

            int countReturn = 0;
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var count = doc.DocumentNode.Descendants("div");

                foreach (var div in count)
                {
                    if (div.Attributes["class"] != null && div.Attributes["class"].Value == "summary")
                    {
                        string tmpC = div.InnerText.Replace("Найдено ", "");
                        tmpC = tmpC.Substring(0, tmpC.IndexOf(" сообще"));
                        try
                        {
                            tmpC.Trim();
                            countReturn = Convert.ToInt32(tmpC);
                        }
                        catch (Exception)
                        {
                            needRun = false;
                            return 0;
                        }
                    }
                }
            }
            catch { }
            return countReturn;
        }

        private int getResultCountForPeoples(string html)
        {
            int countReturn = 0;
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var count = doc.DocumentNode.Descendants("div");

                foreach (var div in count)
                {
                    if (div.Attributes["class"] != null && div.Attributes["class"].Value == "summary")
                    {
                        string tmpC = div.InnerText.Replace(" ", "");

                        string pattern = "(\\d+)";
                        Regex regex = new Regex(pattern);
                        Match match = regex.Match(tmpC);
                        if (match.Success)
                        {
                            tmpC = match.Groups[1].Value;
                        }
                        try
                        {
                            countReturn = Convert.ToInt32(tmpC);
                        }
                        catch { }
                    }
                }
            }
            catch { }
            return countReturn;
        }

        private void getPersons(string html)
        {
            try
            {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            
                var divs = doc.DocumentNode.Descendants("div");

                foreach (var div in divs)
                {
                    if (div.Attributes["class"] != null && div.Attributes["class"].Value == "labeled name")
                    {
                        HtmlAgilityPack.HtmlDocument temp = new HtmlAgilityPack.HtmlDocument();
                        temp.LoadHtml(div.InnerHtml);

                        var a = temp.DocumentNode.Descendants("a");

                        foreach (var aa in a)
                        {
                            if (aa.Attributes["href"].Value.Contains("/"))
                            {
                                PeopleVK.Enqueue("https://vk.com" + aa.Attributes["href"].Value);
                            }
                        }
                    }

                }

            }
            catch(Exception ex)
            {
                if (this.onExceptionCatch != null)
                {
                    onExceptionCatch(this, new ExceptionArgs(ex.Message, "getPersons"));
                }
            }
        }

        public List<string> getPersonsForOne(string html)
        {
            try
            {
            List<string> s = new List<string>();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

          
                var divs = doc.DocumentNode.Descendants("div");

                foreach (var div in divs)
                {
                    if (div.Attributes["class"] != null && div.Attributes["class"].Value == "labeled name")
                    {
                        HtmlAgilityPack.HtmlDocument temp = new HtmlAgilityPack.HtmlDocument();
                        temp.LoadHtml(div.InnerHtml);

                        var a = temp.DocumentNode.Descendants("a");

                        foreach (var aa in a)
                        {
                            if (aa.Attributes["href"].Value.Contains("/"))
                            {
                               s.Add("https://vk.com" + aa.Attributes["href"].Value);
                            }
                        }
                    }

                }
                return s;
            }
            catch(Exception ex)
            {
                if (this.onExceptionCatch != null)
                {
                    onExceptionCatch(this, new ExceptionArgs(ex.Message, "getPersonsForOne"));
                }
            }

            return new List<string>();
            
        }

        private void getGroups(string html)
        {
            try
            {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            
                var divs = doc.DocumentNode.Descendants("div");

                foreach (var div in divs)
                {
                    if (div.Attributes["class"] != null && div.Attributes["class"].Value == "labeled title")
                    {
                        HtmlAgilityPack.HtmlDocument temp = new HtmlAgilityPack.HtmlDocument();
                        temp.LoadHtml(div.InnerHtml);

                        var a = temp.DocumentNode.Descendants("a");

                        foreach (var aa in a)
                        {
                            if (aa.Attributes["href"].Value.Contains("/"))
                            {
                                Groups.Enqueue("https://vk.com" + aa.Attributes["href"].Value);
                            }
                        }
                    }

                }

            }catch(Exception ex){
                if (this.onExceptionCatch != null)
                {
                    onExceptionCatch(this, new ExceptionArgs(ex.Message, "getGroups"));
                }
            }
        }

        
        
    }
}
