using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViKing.Engine;

namespace Parser
{
    class GProxy
    {
        public Queue<Proxy> list = new Queue<Proxy>();
        private string Key { get { return "mbcTouPXeQIfJaBTs3Yl"; } }
       Account acc;
        public GProxy()
        { 
            acc = Account.Load(); 
            Load();
        }

        public Proxy getProxy()
        {
            return Valid();
        }

        private Proxy Valid()
        {

            if (acc.proxy.Count > 0)
            {
                while (acc.proxy.Count > 0)
                {
                    try
                    {
                        Proxy p = acc.getProxy();
                        if (VkRequest.Request("https://www.google.ru/", proxy: p).ContentUTF8 != String.Empty)
                        {
                            return p;
                        }

                    }
                    catch
                    {

                    }
                }
            }
            else
            { 
                while (list.Count > 0)
                {
                    try
                    {
                        Proxy p = list.Dequeue();
                        if (VkRequest.Request("https://www.google.ru/", proxy: p).ContentUTF8 != String.Empty)
                        {
                            return p;
                        }

                    }
                    catch
                    {

                    }
                }
            }

            Load();

            return Valid();
        }

        public void Load()
        {
            var test = VkRequest.Request("http://api.best-proxies.ru/feeds/proxylist.txt?key=" + Key + "&limit=0&level=1,2&includeType&google=1&response=300").ContentUTF8;

            foreach (string s in test.Split("\n"))
            {
                string[] temp = s.Split("://");

                switch (temp[0])
                {
                    case "http": list.Enqueue(new Proxy(temp[1], ProxyTypes.HTTP));break;
                    case "socks4": list.Enqueue(new Proxy(temp[1], ProxyTypes.Socks4)); break;
                    case "socks5": list.Enqueue(new Proxy(temp[1], ProxyTypes.Socks5)); break;
                } 
            }

        }
    }
}
