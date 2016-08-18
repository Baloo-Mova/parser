using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Hello.Models
{
    public class Users
    {
        public int Id {get;set;}
        [Display(Name="Логин: ")]
        [Required(ErrorMessage = "Введите логин")]
        public string login { get; set; }
         [Display(Name = "Пароль: ")]
        [Required(ErrorMessage="Введите пароль")]
        [DataType(DataType.Password)]
        public string parol { get; set; }

       public  Users()
        {

        }

       public Users(string l, string p)
       {
           login = l;
           parol = p;
       }

    }
}