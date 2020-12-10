using System;

namespace ZerochSharp.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class SettingTxtAttribute : Attribute
    {
        private string name { get; set; }
        public SettingTxtAttribute(string name)
        {
            this.name = name;
        }
        public string Name => name;
    }
}
