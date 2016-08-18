using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ViKing.Engine;

namespace Parser
{
    public class Account
    {
        private Random rand = new Random();
        public List<Mails> mails = new List<Mails>();
        public List<Mails> ok = new List<Mails>();
        public List<string> proxy = new List<string>();
        public List<Mails> vk = new List<Mails>();
        public List<Mails> WatsApp = new List<Mails>();
        int returned = 0;
        int returnedProxy = 0;

        public Mails getMail()
        {
            return mails[rand.Next(0, mails.Count - 1)];
        }

        public Mails getOKAccount()
        {

            if (ok.Count < 2) { return null; }
            if (returned < ok.Count)
            {
                return ok[returned++];
            }
            else
            {
                returned = 1;
                return ok[returned++];
            }
        }

        public Proxy getProxy()
        { 
            if (returnedProxy < ok.Count)
            {
                return new Proxy(proxy[returnedProxy++],ProxyTypes.Socks5);
            }
            else
            {
                returnedProxy = 1;
                return new Proxy(proxy[returnedProxy++],ProxyTypes.Socks5);
            }
        }

        
        public static Account Load()
        {
            XmlSerializer serializer =
         new XmlSerializer(typeof(Account));
            if (!File.Exists("Sys.CFG"))
            {
                using (StreamWriter swt = new StreamWriter("Sys.CFG"))
                {
                    serializer.Serialize(swt, new Account());
                }
            }
            Account acc;
            using (StreamReader read = new StreamReader("Sys.CFG"))
            {
               acc  = (Account)serializer.Deserialize(read);
                read.Close();
            }
            return acc;
        }

        public void Delete(Mails what, ParserType type)
        {
            if (type == ParserType.OK)
            {
                ok.Remove(what);
                Save();
            }
        }

        private void Save()
        {
            XmlSerializer serializer =
         new XmlSerializer(typeof(Account));

            using (StreamWriter swt = new StreamWriter("Sys.CFG"))
            {
                serializer.Serialize(swt, this);
            }
        }
 
    }
}
