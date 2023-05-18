using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace StartingMultiTenantLib
{
    public static class ServiceMutiTenantStoreExtension
    {
        public static IServiceCollection AddServiceMutiTenantStore(this IServiceCollection services, ServiceMutiTenantStoreOption option) {
            services.AddSingleton<ServiceMutiTenantStoreOption>((provider) =>option);
            services.AddScoped<ContextTenantDomain>();
            return services.AddTransient<ServiceMutiTenantStore>();
        }
    }
}
