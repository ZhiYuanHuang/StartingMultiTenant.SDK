using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StartingMultiTenantLib
{
    internal class K8sSecretTenantExecutor : RequestApiExecutor, IReadTenantExecutor
    {
        private readonly ILogger<K8sSecretTenantExecutor> _logger;
        public K8sSecretTenantExecutor(ILogger<K8sSecretTenantExecutor> logger, string baseUrl, string clientId, string clientSecret) 
            : base(logger, baseUrl, clientId, clientSecret) {
            _logger = logger;
        }

        public override Task<TenantDbConnsDto> GetTenantDbConns(string tenantDomain, string tenantIdentifier, string serviceIdentifier) {
            return base.GetTenantDbConns(tenantDomain, tenantIdentifier, serviceIdentifier);
        }
    }
}
