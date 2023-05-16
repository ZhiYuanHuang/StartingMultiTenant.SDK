using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StartingMultiTenantLib
{
    public class StartingMutilTenantClient
    {
        private readonly StartingMultiTenantClientOption _option;
        private IRequestTenantExecutor _requestTenantExecutor = null;
        private readonly object _lockObj = new object();
        public StartingMutilTenantClient(StartingMultiTenantClientOption clientOption) {
            _option = clientOption;
            
        }

        public async Task<TenantDbConnsDto> GetTenantDbConns(string tenantDomain, string tenantIdentifier, string serviceIdentifier) {
            var requestExecutor = getRequestTenantExecutor();
            return await requestExecutor.GetTenantDbConns(tenantDomain,tenantIdentifier,serviceIdentifier);
        }

        private IRequestTenantExecutor getRequestTenantExecutor() {
            if (_requestTenantExecutor == null) {
                lock (_lockObj) {
                    if (_requestTenantExecutor == null) {
                        _requestTenantExecutor = _option.ResolveExecutor();
                    }
                }
            }

            return _requestTenantExecutor;
        }
    }
}
