using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StartingMultiTenantLib
{
    public interface IRequestTenantExecutor
    {
        Task<TenantDbConnsDto> GetTenantDbConns(string tenantDomain,string tenantIdentifier,string serviceIdentifier);
    }
}
