using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StartingMultiTenantLib.Executor;
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
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                optionBuilderAction(provider,optionBuilder);
                var clientOptions = new StartingMultiTenantClientOption(loggerFactory,optionBuilder);
                return clientOptions;
            });
            return services.AddTransient<StartingMutilTenantClient>();
        }

        public static StartingMultiTenantClientOption UseRequest(string baseUrl, string clientId, string clientSecret) {
            StartingMultiTenantLibOption option = new StartingMultiTenantLibOption();
            StartingMultiTenantLibOptionBuilder builder = new StartingMultiTenantLibOptionBuilder(option);
            builder.UseRequest(baseUrl, clientId, clientSecret);
            return new StartingMultiTenantClientOption(builder);
        }

        public static StartingMultiTenantClientOption UseRedis(string connStr) {
            StartingMultiTenantLibOption option = new StartingMultiTenantLibOption();
            StartingMultiTenantLibOptionBuilder builder = new StartingMultiTenantLibOptionBuilder(option);
            builder.UseRedis(connStr);
            return new StartingMultiTenantClientOption(builder);
        }

        public static StartingMultiTenantClientOption UseK8sSecret(string secretFilePath) {
            StartingMultiTenantLibOption option = new StartingMultiTenantLibOption();
            StartingMultiTenantLibOptionBuilder builder = new StartingMultiTenantLibOptionBuilder(option);
            builder.UseK8sSecret(secretFilePath);
            return new StartingMultiTenantClientOption(builder);
        }

        public static StartingMultiTenantClientOption UseCustomRead(IServiceProvider provider, IReadTenantExecutor readTenantExecutor) {
            StartingMultiTenantLibOption option = new StartingMultiTenantLibOption();
            StartingMultiTenantLibOptionBuilder builder = new StartingMultiTenantLibOptionBuilder(option);
            builder.UseCustomRead(provider, readTenantExecutor);
            return new StartingMultiTenantClientOption(builder);
        }

        public static StartingMultiTenantClientOption UseCustomWrite(IServiceProvider provider, IWriteTenantExecutor writeTenantExecutor) {
            StartingMultiTenantLibOption option = new StartingMultiTenantLibOption();
            StartingMultiTenantLibOptionBuilder builder = new StartingMultiTenantLibOptionBuilder(option);
            builder.UseCustomWrite(provider, writeTenantExecutor);
            return new StartingMultiTenantClientOption(builder);
        }
    }
}
