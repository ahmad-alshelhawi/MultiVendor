// Domain/Attributes/NoAuditAttribute.cs
using System;

namespace AttarStore.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class NoAuditAttribute : Attribute { }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class NoAuditForRolesAttribute : Attribute
    {
        public string[] Roles { get; }

        public NoAuditForRolesAttribute(params string[] roles)
        {
            Roles = roles;
        }
    }
}
