using System;
using System.Collections.Generic;
using System.Text;

namespace StartingMultiTenantLib.Const
{
    public static class SMTConsts
    {
        internal const string TenantToken = "__tenant__";
        public const string TenantIdentifierHeaderKey = "TenantIdentifier_SMT";
        public const string TenantDomainHeaderKey = "TenantDomain_SMT";
    }
}
