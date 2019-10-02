using System;
using System.Collections.Generic;
using System.Text;

namespace ZerochPlus.Models
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class SettingTxtAttribute : Attribute
    {
        private string name { get; set; }
        public SettingTxtAttribute(string name)
        {
            this.name = name;
        }
        public string Name { get { return name; } }
    }
}
