using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace StartingMultiTenantLib
{
    public static class ServiceMutiTenantStoreExtension
    {
        public static IServiceCollection AddServiceMutiTenantStore(this IServiceCollection services, Action<ServiceMutiTenantStoreOption> optionAction) {
            services.AddSingleton<ServiceMutiTenantStoreOption>((provider) => {
                ServiceMutiTenantStoreOption option = new ServiceMutiTenantStoreOption();
                optionAction(option);
                return option;
            });
            services.AddScoped<ContextTenantDomain>();
            return services.AddTransient<ServiceMutiTenantStore>();
        }
    }
}
