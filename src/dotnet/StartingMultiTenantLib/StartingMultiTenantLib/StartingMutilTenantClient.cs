using Microsoft.Extensions.Logging;
using StartingMultiTenantLib.Executor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StartingMultiTenantLib
{
    public class StartingMutilTenantClient
    {
        private readonly StartingMultiTenantClientOption _option;
        private readonly ILogger<StartingMutilTenantClient> _logger;
        private IReadTenantExecutor _readTenantExecutor = null;
        private IWriteTenantExecutor _writeTenantExecutor = null;
        private readonly object _lockObj = new object();
        public StartingMutilTenantClient(StartingMultiTenantClientOption clientOption) {
            _option = clientOption;
        }

        public async Task<TenantDbConnsDto> GetTenantDbConns(string tenantDomain, string tenantIdentifier, string serviceIdentifier) {
            var requestExecutor = getReadTenantExecutor();
            return await requestExecutor.GetTenantDbConns(tenantDomain,tenantIdentifier,serviceIdentifier);
        }

        public async Task<CreateTenantResultDto> CreateTenant(string tenantDomain, string tenantIdentifier, List<string> createDbScriptNameList = null, string tenantName = null, string description = null) {
            var requestExecutor = getWriteTenantExecutor();

            try {
                var result= await requestExecutor.CreateTenant(tenantDomain, tenantIdentifier, createDbScriptNameList, tenantName, description);
                return result;
            } catch(Exception ex) {
                _logger?.LogError(ex, "CreateTenant raise error");
                return new CreateTenantResultDto() { Success=false,ErrMsg="invoke error"};
            }
            
        }

        private IReadTenantExecutor getReadTenantExecutor() {
            if (_readTenantExecutor == null) {
                lock (_lockObj) {
                    if (_readTenantExecutor == null) {
                        _readTenantExecutor = _option.ResolveReadExecutor();
                    }
                }
            }

            return _readTenantExecutor;
        }

        private IWriteTenantExecutor getWriteTenantExecutor() {
            if (_writeTenantExecutor == null) {
                lock (_lockObj) {
                    if (_writeTenantExecutor == null) {
                        _writeTenantExecutor = _option.ResolveWriteExecutor();
                    }
                }
            }

            return _writeTenantExecutor;
        }
    }
}
