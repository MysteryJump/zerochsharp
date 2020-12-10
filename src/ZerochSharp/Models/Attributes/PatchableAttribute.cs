using System;

namespace ZerochSharp.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]

    public class PatchableAttribute : Attribute
    {
        public PatchableAttribute()
        {

        }
    }
}
