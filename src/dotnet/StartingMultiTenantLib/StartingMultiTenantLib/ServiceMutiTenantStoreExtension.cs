using Finbuckle.MultiTenant;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace StartingMultiTenantLib
{
    public static class ServiceMutiTenantStoreExtension
    {
        public static IServiceCollection AddServiceMutiTenantStore(this IServiceCollection services, Action<ServiceMutiTenantStoreOption> optionAction,ServiceLifetime serviceLifetime=ServiceLifetime.Scoped) {
            services.AddSingleton<ServiceMutiTenantStoreOption>((provider) => {
                ServiceMutiTenantStoreOption option = new ServiceMutiTenantStoreOption();
                optionAction(option);
                return option;
            });
            services.AddScoped<ContextTenantDomain>();
            services.Add(ServiceDescriptor.Describe(typeof(IMultiTenantStore<TenantDbConnsDto>),typeof(ServiceMutiTenantStore),serviceLifetime));
            return services;
        }

        public static FinbuckleMultiTenantBuilder<TenantDbConnsDto> WithMultiTenant(this IServiceCollection services, Action<ServiceMutiTenantStoreOption> optionAction, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            services.AddSingleton<ServiceMutiTenantStoreOption>((provider) => {
                ServiceMutiTenantStoreOption option = new ServiceMutiTenantStoreOption();
                optionAction(option);
                return option;
            });
            services.AddScoped<ContextTenantDomain>();
            return services.AddMultiTenant<TenantDbConnsDto>()
                .WithStore<ServiceMutiTenantStore>(serviceLifetime)
                .WithStrategy<TenantResolveStrategy>(ServiceLifetime.Singleton, "__tenant__.*");
        }
    }
}
