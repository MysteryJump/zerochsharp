using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZerochSharp.Models
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]

    public class PatchableAttribute : Attribute
    {
        public PatchableAttribute()
        {

        }
    }
}
