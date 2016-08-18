using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Parser
{
    public enum ParserType{
        VK,Google,OK    
    }
    class ContactData  
    {
        [DisplayName("ID")]
        public int id { get; set; }
        [DisplayName("Ник и ФИО")]
        public string FIO { get; set; }
        [DisplayName("Ссылка")]
        public string link { get; set; }
        [DisplayName("Пол")]
        public string sex { get; set; }
        [DisplayName("Email")]
        public string mails { get; set; }
        [DisplayName("Страна")]
        public string country { get; set; }
        [DisplayName("Город")]
        public string city { get; set; }
        [DisplayName("Телефоны")]
        public string phones { get; set; }
        [DisplayName("Скайп")]
        public string skypes { get; set; }
        [DisplayName("ICQ")]
        public string icq { get; set; }
        [DisplayName("Поисковой запрос")]
        public string query { get; set; }

        public ParserType type;

        public ContactData()
        {
         
        }

        public ContactData(object id, object fio, object link, object mails, object city, object phones, object skypes)
        {
            this.id = Convert.ToInt32(id);
            if(fio!=null)
            this.FIO = fio.ToString();
            if (link != null)
            this.link = link.ToString();
            if (mails != null)
            this.mails = mails.ToString();
            if (city != null)
            this.city = city.ToString();
            if (phones != null)
            this.phones = phones.ToString();
            if (skypes != null)
            this.skypes = skypes.ToString();
        }

        public override string ToString()
        {
             return id+" "+FIO+" "+link+" "+sex+" "+mails+" "+country+" "+city+" "+phones+" "+skypes+" "+icq+" "+query;
        }
    }
}
