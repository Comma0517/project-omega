using System;

namespace Omega.Plumbing.Data
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DbInfo : Attribute
    {
        public string DbName { get; init; }
    }
}