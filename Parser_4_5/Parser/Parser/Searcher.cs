using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Parser
{
    public static class Searcher
    {
        //((\+8|\+7|7|8|\+38|38)[-. ]?)?\(?([0-9]{3,4})\)?[-. ]?([0-9]{2,3})[-. ]?([0-9]{2,3})[-. ]?([0-9]{2,3}) - Телефон
        //([a-zA-Z]([-.w]*[0-9a-zA-Z_])*@([0-9a-zA-Z][-w]*[0-9a-zA-Z].)+[a-zA-Z]{2,9})*\.[a-z]{2,6} - Мейл
        //([1-9])+(?:-?\d){4,}  - ICQ
        public static SortedSet<string> findPhone(string html)
        {
            SortedSet<string> list = new SortedSet<string>();
            try
            {
                string pattern = "((\\+8|\\+7|7|8|\\+38|38)[-. ]?)?\\(?([0-9]{3,4})\\)?[-. ]?([0-9]{2,3})[-. ]?([0-9]{1,3})[-. ]?([0-9]{2,3})";
                Regex regex = new Regex(pattern);
                Match match = regex.Match(html);

                while (match.Success)
                {
                    if (match.Groups[0].Value.Length > 10)
                    {//Количество символов в номере если ниже 10 много хлама
                        if (!match.Groups[0].Value.StartsWith("1") && !match.Groups[0].Value.StartsWith("2") && !match.Groups[0].Value.StartsWith("4") && !match.Groups[0].Value.StartsWith("5") && !match.Groups[0].Value.StartsWith("6"))
                            list.Add(match.Groups[0].Value);
                    }

                    match = match.NextMatch();
                }
            }
            catch { }
            return list;
        }
        

        public static string getFIO(string html)
        {
            string tmpC = "";
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);

                var count = doc.DocumentNode.Descendants("div");

                foreach (var div in count)
                {
                    if (div.Attributes["class"] != null && div.Attributes["class"].Value == "page_name fl_l ta_l")
                    {
                        tmpC = div.InnerText;
                    }
                }
            }
            catch { }
            return tmpC;
        }

        public static string getCity(string html)
        {

            string tmpC = "";
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);

                var count = doc.DocumentNode.Descendants("div");

                foreach (var div in count)
                {
                    if (div.Attributes["class"] != null && div.Attributes["class"].Value == "labeled fl_l")
                    {
                        HtmlAgilityPack.HtmlDocument d = new HtmlDocument();
                        d.LoadHtml(div.InnerHtml);

                        var test = d.DocumentNode.Descendants("a");
                        foreach (var temp in test)
                        {
                            if (temp.Attributes["href"] != null && temp.Attributes["href"].Value.Contains("c[city]="))
                            {
                                return temp.InnerText;
                            }
                        }

                    }
                }
            }
            catch { }
            return tmpC;
        }

        public static SortedSet<string> findMail(string html)
        {
           SortedSet<string> list = new SortedSet<string>();
           try
           {
               string pattern = "([a-z])+[a-z0-9_-]+@[a-z0-9_-]+(\\.[a-z0-9_-]+)*\\.[a-z]{2,6}";
               Regex regex = new Regex(pattern, RegexOptions.None, TimeSpan.FromMilliseconds(10000));

               MatchCollection match = regex.Matches(html);

               foreach (Match m in match)
               {
                   list.Add(m.Groups[0].Value);
               }
           }
           catch { }
            return list;
        }

        public static SortedSet<string> findSkype(string html)
        {
            SortedSet<string> list = new SortedSet<string>();
            try
            {
                string pattern = "skype:(.*?)\\?";
                Regex regex = new Regex(pattern);
                Match match = regex.Match(html);

                while (match.Success)
                {
                    list.Add(match.Groups[0].Value.Remove(0, 6).Replace("?", ""));
                    match = match.NextMatch();
                }
            }
            catch { }
            return list;
        }

        public static SortedSet<string> findICQ(string html)
        {
            SortedSet<string> list = new SortedSet<string>();
            try
            {
                string pattern = "([1-9])+(?:-?\\d){4,}";
                Regex regex = new Regex(pattern);
                Match match = regex.Match(html);

                while (match.Success)
                {


                    list.Add(match.Groups[0].Value);


                    match = match.NextMatch();
                }
            }
            catch { }
            return list;
        }

    }
}
