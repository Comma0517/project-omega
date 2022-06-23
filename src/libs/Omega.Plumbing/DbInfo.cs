using System;

namespace Omega.Plumbing
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DbInfo : Attribute
    {
        public string DbName { get; init; }
    }
}