using Finbuckle.MultiTenant;
using System;
using System.Collections.Generic;
using System.Text;

namespace StartingMultiTenantLib
{
    public class TenantDbConnDto
    {
        public string ServiceIdentifier { get; set; }
        public string DbIdentifier { get; set; }
        public string DbConn { get; set; }
    }
    public class TenantDbConnsDto: ITenantInfo
    {
        public string TenantDomain { get; set; }
        public string TenantIdentifier { get; set; }
        public List<TenantDbConnDto> InnerDbConnList { get; set; }
        public List<TenantDbConnDto> ExternalDbConnList { get; set; }
        public string? Id { get; set; }
        public string? Identifier { 
            get {
                return TenantIdentifier;
            }
            set {
                TenantIdentifier = value;
            }
        }
        public string? Name { get; set; }
        public string? ConnectionString { get; set; }
    }
}
