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

        //空租户数据源identifier
        public const string Empty_Tenant = "empty";
        //系统用domain
        public const string Sys_TenantDomain = "sys.com";

        public const string AuthorPolicy_SuperAdmin = "TenantSuperAdminPolicy";
        //服务超级管理权限
        public const string Service_Super_Admin_Scope = "tenantservice.superadmin";

        //服务单租户管理员认证策略
        public const string AuthorPolicy_TenantAdmin = "TenantAdminPolicy";
        public const string Service_Admin_Role = "tenantserviceadmin";
    }
}
