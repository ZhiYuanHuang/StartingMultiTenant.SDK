using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StartingMultiTenantLib.Executor
{
    public interface IWriteTenantExecutor
    {
        Task<CreateTenantResultDto> CreateTenant(string tenantDomain,string tenantIdentifier,List<string> createDbScriptNameList=null, string tenantName=null, string description=null);
    }
}
