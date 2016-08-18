using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViKing.Engine;

namespace Parser
{

    public delegate void ReportMessage(object sender, string message);
 
   public class OK
    {

        private string password;
        private string login;
        private string TKN = "";
        private string gtw = "";
        private int users = 0;
        private CookieCollection cookie = new CookieCollection();

        private Queue<string> groups = new Queue<string>();
        public Queue<string> Users { get; set; }

        public event Show onDataGet;

        public event ExceptionHandler onExceptionCatch;


        private Proxy proxy = new Proxy("127.0.0.1:8888", ProxyTypes.HTTP);

        public OK()
        {

        }

        public OK(string login, string password)
        {
            this.login = login;
            this.password = password;
            Users = new Queue<string>();
            needRun = false;
             
        }

        public void Exception(ExceptionArgs e)
        {
            if (onExceptionCatch != null)
            {
                onExceptionCatch(this, e);
            }
        }

        public string getToken(VkResponse result)
        {
            try
            {
                if (result.Headers["TKN"] != string.Empty)
                {
                    return result.Headers["TKN"];
                }

                return result.ContentUTF8.Substring(result.ContentUTF8.IndexOf("OK.tkn.set('") + 12, 32);
            }
            catch
            {
                return "";
            }
        }
        public bool Login()
        {
            string post = @"st.redirect=&st.asr=&st.posted=set&st.originalaction=http%3A%2F%2Fok.ru%2Fdk%3Fcmd%3DAnonymLogin%26st.cmd%3DanonymLogin&st.fJS=enabled&st.st.screenSize=1920+x+1080&st.st.browserSize=1010&st.st.flashVer=16.0.0&st.email=" + login + "&st.password=" + password + "&st.remember=on&st.iscode=false";
            VkResponse resp = VkRequest.Request("https://www.ok.ru/https", post, "POST", cookies: cookie, FollowRedirects: true);

            if (resp.ContentUTF8.Contains("Мы отправили"))
            {
                if (onExceptionCatch != null)
                    onExceptionCatch(this, new ExceptionArgs("BadAccount", login));
                return false;
            }

            TKN = resp.ContentUTF8.Substring(resp.ContentUTF8.IndexOf("OK.tkn.set('") + 12, 32);

            resp = VkRequest.Request("http://ok.ru", cookies: cookie);
            gtw = resp.ContentUTF8.Substring(resp.ContentUTF8.IndexOf("gwtHash:") + 9, 8);
       
            return true;
        }

        public void Search(string what)
        {

            needRun = true;
            HTTPHeaderItem[] items = new HTTPHeaderItem[1];
            items[0] = new HTTPHeaderItem("TKN", TKN);

            var result = VkRequest.Request("http://ok.ru/dk?st.cmd=searchResult&st.query=" + HttpUtility.UrlEncode(what), cookies: cookie, FollowRedirects: true, additionalHeaders: items);
            SearchLinks(result.ContentUTF8);

            int iterator = 2;
            TKN = getToken(result);
            items[0].Value = TKN;

            do
            {
                if (!needRun) return;
                int count = groups.Count;
                string query = "&fetch=false&st.page=" + iterator + "&st.loaderid=PortalSearchResultsLoader";
                result = VkRequest.Request("http://ok.ru/dk?cmd=PortalSearchResults&st.cmd=searchResult&st.query=" + HttpUtility.UrlEncode(what) + "&st.mode=Groups&st.grmode=Groups", query, "POST", cookies: cookie, FollowRedirects: true, additionalHeaders: items);
                iterator++;
                TKN = getToken(result);
                items[0].Value = TKN;
                
                SearchLinks(result.ContentUTF8);
                if (groups.Count == count) { break; }
            } while (result.ContentUTF8.Length > 0);

            foreach (string url in groups)
            {
                string groupLink = ""; string groupID = "";
                try
                {
                     groupLink = url.Substring(0, url.IndexOf("?"));
                     groupID = url.Remove(0, url.IndexOf("st.groupId"));
                    int id = groupID.IndexOf("&");
                    groupID = groupID.Substring(groupID.IndexOf("st.groupId") + 11, id - 11);
                }
                catch { continue; }

                    int i = 1;
                    while (true)
                    {
                        if (!needRun) return;
                        int nowC = users;
                        var res = VkRequest.Request("http://ok.ru" + groupLink + "/members?cmd=GroupMembersResultsBlock&gwt.requested=" + gtw + "&st.cmd=altGroupMembers&st.groupId=" + groupID, "fetch=false&st.page=" + i + "&st.loaderid=GroupMembersResultsBlockLoader", "POST", cookies: cookie, additionalHeaders: items);
                        TKN = getToken(res);
                        items[0].Value = TKN;
                        parsePeople(res.ContentUTF8);
                        Thread.Sleep(10000);
                        i++;
                        if (users == nowC) { break; }
                    }
               
            }

            needRun = false;

        }

        public void parsePeople(string data)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            try
            {
                doc.LoadHtml(data);
                foreach (HtmlNode div in doc.DocumentNode.SelectNodes("//a[contains(@class,'photoWrapper')]"))
                {
                    if (div.Attributes["href"] != null)
                    {
                        string Value = div.Attributes["href"].Value;
                        Value = Value.Remove(0, Value.IndexOf("st.friendId") + 12);
                        Value = Value.Substring(0, Value.IndexOf("&"));


                        if (!Users.Contains(Value))
                        {
                            Users.Enqueue(Value);
                            users++;
                            ContactData dat = new ContactData();
                            dat.link = Value;
                            dat.type = ParserType.OK;
                            onDataGet(dat);
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void getPeople(string link)
        {
            while (true)
            {
                int count = Users.Count;
                HTTPHeaderItem[] items = new HTTPHeaderItem[1];
                if (TKN == String.Empty)
                {
                    var res = VkRequest.Request("https://www.ok.ru", cookies: cookie);
                    TKN = getToken(res);
                }
                items[0] = new HTTPHeaderItem("TKN", TKN);
                var response = VkRequest.Request("https://www.ok.ru" + link + "/members", "", "POST", cookies: cookie, additionalHeaders: items);



                if (count == Users.Count)
                {
                    break;
                }
            }
        }

        public string  sendMessage(string message, string userID)
        {
            try
            { 

                HTTPHeaderItem[] items = new HTTPHeaderItem[1];
                if (TKN == String.Empty)
                {
                    var res = VkRequest.Request("https://www.ok.ru", cookies: cookie);
                    TKN = getToken(res);
                }
                items[0] = new HTTPHeaderItem("TKN", TKN);

                var resp = VkRequest.Request("http://ok.ru/profile/" + userID + "?cmd=ToolbarMessages&st.cmd=friendMain&gwt.requested="+gtw+"&st.friendId=" + userID, "tlb.act=act.send.msg&d.fr.id=" + userID + "&d.msg=" + HttpUtility.UrlEncode(message) + "&d.dleot=0&d.aPh=&refId=msg-send-comment-" + TimeStamp(), "POST", cookies: cookie, additionalHeaders: items);
                return resp.ContentUTF8;
            }
            catch
            {
                return "NotSended";
            }
           

        }
        public string TimeStamp()
        {
            return (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString();
        }
        public void SearchLinks(string html)
        {
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                foreach (HtmlNode div in doc.DocumentNode.SelectNodes("//div[contains(@class,'ucard-b_img')]"))
                {
                    HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
                    doc1.LoadHtml(div.InnerHtml);
                    var next = doc1.DocumentNode.Descendants("a");
                    foreach (var temp2 in next)
                    {
                        if (temp2.Attributes["href"] != null)
                        {
                            if (!groups.Contains(temp2.Attributes["href"].Value) && !temp2.Attributes["href"].Value.Contains("/dk"))
                            {
                                groups.Enqueue(temp2.Attributes["href"].Value);

                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

        } 
        public bool needRun { get; set; }
    }
}
