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
    public class TenantDbConnsDto
    {
        public string TenantDomain { get; set; }
        public string TenantIdentifier { get; set; }
        public List<TenantDbConnDto> InnerDbConnList { get; set; }
        public List<TenantDbConnDto> ExternalDbConnList { get; set; }
    }
}
