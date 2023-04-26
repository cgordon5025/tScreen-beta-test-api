using System;
using System.Runtime.CompilerServices;
using Data;
using HotChocolate.Data;

namespace GraphQl.GraphQl.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class UseApplicationDbContextAttribute : UseDbContextAttribute
    {
        public UseApplicationDbContextAttribute([CallerLineNumber] int order = 0)
            : base(typeof(ApplicationDbContext))
        {
            Order = order;
        }
    }
}