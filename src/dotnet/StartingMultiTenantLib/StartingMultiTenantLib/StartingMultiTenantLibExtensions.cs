﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace StartingMultiTenantLib
{
    public static class StartingMultiTenantLibExtensions
    {
        public static IServiceCollection AddStartingMultiTenantClient(this IServiceCollection services,Action<IServiceProvider,StartingMultiTenantLibOptionBuilder> optionBuilderAction) {
            services.AddSingleton<StartingMultiTenantLibOption>();
            services.AddSingleton<StartingMultiTenantLibOptionBuilder>();
            services.AddSingleton<StartingMultiTenantClientOption>((provider) => {
                var optionBuilder = provider.GetRequiredService<StartingMultiTenantLibOptionBuilder>();
                
                optionBuilderAction(provider,optionBuilder);
                var clientOptions = new StartingMultiTenantClientOption(optionBuilder);
                return clientOptions;
            });
            return services.AddSingleton<StartingMutilTenantClient>();
        }
    }
}
