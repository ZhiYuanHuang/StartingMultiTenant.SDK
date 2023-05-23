using Finbuckle.MultiTenant;
using Microsoft.Extensions.Logging;
using StartingMultiTenantLib.Const;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace StartingMultiTenantLib
{
    public class ServiceMutiTenantStore : IMultiTenantStore<TenantDbConnsDto>
    {
        private readonly ServiceMutiTenantStoreOption _option;
        private readonly ContextTenantDomain _contextTenantDomain;
        private readonly StartingMutilTenantClient _startingMutilTenantClient;
        private readonly ILogger<ServiceMutiTenantStore> _logger;

        private const string DEFAULT_TENANT_DOMAIN = "default";

        private readonly string CacheKeyTemplate_Tenant;

        public ServiceMutiTenantStore(ServiceMutiTenantStoreOption option,
            ContextTenantDomain contextTenantDomain,
            StartingMutilTenantClient startingMutilTenantClient,
            ILogger<ServiceMutiTenantStore> logger) {
            _option= option;
            _contextTenantDomain= contextTenantDomain;
            _startingMutilTenantClient = startingMutilTenantClient;
            _logger = logger;

            if (!string.IsNullOrEmpty(option?.ServiceIdentifier)) {
                CacheKeyTemplate_Tenant = "ServiceMutiTenantStore_" + option.ServiceIdentifier + "_{0}_{1}";
            } else {
                CacheKeyTemplate_Tenant = "ServiceMutiTenantStore_{0}_{1}";
            }
        }
        public Task<IEnumerable<TenantDbConnsDto>> GetAllAsync() {
            throw new NotImplementedException();
        }

        public Task<bool> TryAddAsync(TenantDbConnsDto tenantInfo) {
            throw new NotImplementedException();
        }

        public Task<TenantDbConnsDto?> TryGetAsync(string id) {
            throw new NotImplementedException();
        }

        public async Task<TenantDbConnsDto?> TryGetByIdentifierAsync(string identifier) {
            string tenantDomain = DEFAULT_TENANT_DOMAIN;
            if(!string.IsNullOrEmpty(_contextTenantDomain?.TenantDomain)) {
                tenantDomain=_contextTenantDomain.TenantDomain;
            }

            TenantDbConnsDto tenantDbConnsDto = null;
            string tenantCacheKey = string.Format(CacheKeyTemplate_Tenant, tenantDomain, identifier);
            if(MemoryCacheHelper.Contains(tenantCacheKey,out tenantDbConnsDto)) {
                return tenantDbConnsDto;
            }

            try {
                tenantDbConnsDto = await _startingMutilTenantClient.GetTenantDbConns(tenantDomain,identifier,_option?.ServiceIdentifier);
                if (tenantDbConnsDto.NoExist && _option.UseEmptySourceWhenNoExistTenant) {
                    tenantDbConnsDto = await _startingMutilTenantClient.GetTenantDbConns(SMTConsts.Sys_TenantDomain, SMTConsts.Empty_Tenant, _option?.ServiceIdentifier);
                }

                if (_option?.CacheMilliSec > 0) {
                    if (tenantDbConnsDto != null) {
                        MemoryCacheHelper.Set(tenantCacheKey, _option.CacheMilliSec, tenantDbConnsDto);
                    } else {
                        MemoryCacheHelper.Set(tenantCacheKey, 100, tenantDbConnsDto);
                    }
                }
            } catch(Exception ex) {
                _logger.LogError($"TryGetByIdentifierAsync raise error,ex:{ex.Message}");
            }

            return tenantDbConnsDto;
        }

        public Task<bool> TryRemoveAsync(string tenantInfo) {
            throw new NotImplementedException();
        }

        public Task<bool> TryUpdateAsync(TenantDbConnsDto tenantInfo) {
            throw new NotImplementedException();
        }
    }
}
