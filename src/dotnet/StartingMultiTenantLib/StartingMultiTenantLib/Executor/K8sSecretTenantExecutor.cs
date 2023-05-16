using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StartingMultiTenantLib
{
    internal class K8sSecretTenantExecutor : RequestApiExecutor, IRequestTenantExecutor
    {
        public K8sSecretTenantExecutor(ILogger<IRequestTenantExecutor> logger, string baseUrl, string clientId, string clientSecret) 
            : base(logger, baseUrl, clientId, clientSecret) {
        }

        public override Task<TenantDbConnsDto> GetTenantDbConns(string tenantDomain, string tenantIdentifier, string serviceIdentifier) {
            return base.GetTenantDbConns(tenantDomain, tenantIdentifier, serviceIdentifier);
        }
    }
}
