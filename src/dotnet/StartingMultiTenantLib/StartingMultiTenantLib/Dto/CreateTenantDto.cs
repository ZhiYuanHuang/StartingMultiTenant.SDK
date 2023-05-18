using System;
using System.Collections.Generic;
using System.Text;

namespace StartingMultiTenantLib
{
    public class CreateTenantDto {
        public string TenantDomain { get; set; }
        public string TenantIdentifier { get; set; }
        public string TenantName { get; set;}
        public List<string> CreateDbScripts { get; set; }
        public string Description { get; set; }
    }
    public class CreateTenantResultDto
    {
        public bool Success { get; set; }
        public string ErrMsg { get; set; }
    }
}
