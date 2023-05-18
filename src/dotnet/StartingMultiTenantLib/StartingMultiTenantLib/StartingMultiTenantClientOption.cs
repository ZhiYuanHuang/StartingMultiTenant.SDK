using Microsoft.Extensions.Logging;
using StartingMultiTenantLib.Executor;
using System;
using System.Collections.Generic;
using System.Text;

namespace StartingMultiTenantLib
{
    public class StartingMultiTenantClientOption
    {
        private readonly StartingMultiTenantLibOptionBuilder _optionBuilder;
        private readonly ILoggerFactory _loggerFactory;
        internal StartingMultiTenantClientOption(ILoggerFactory loggerFactory,  StartingMultiTenantLibOptionBuilder startingMultiTenantLibOptionBuilder) {
            _optionBuilder=startingMultiTenantLibOptionBuilder;
            _loggerFactory=loggerFactory;
        }

        internal StartingMultiTenantClientOption(StartingMultiTenantLibOptionBuilder startingMultiTenantLibOptionBuilder) {
            _optionBuilder = startingMultiTenantLibOptionBuilder;
        }

        internal IReadTenantExecutor ResolveReadExecutor() {
            IReadTenantExecutor executor = null;
            var libOption= _optionBuilder.Option;
            switch (libOption.ReadTargetType) {
                case EnumTargetType.Redis: {
                        var logger = _loggerFactory?.CreateLogger<RedisTenantExecutor>();
                        executor = new RedisTenantExecutor(libOption.RedisConnStr,logger,libOption.EnableRequest, libOption.RequestBaseUrl, libOption.ClientId, libOption.ClientSecret);
                    }
                    break;
                case EnumTargetType.K8sSecret: {
                        var logger = _loggerFactory?.CreateLogger<K8sSecretTenantExecutor>();
                        executor = new K8sSecretTenantExecutor(logger, libOption.RequestBaseUrl, libOption.ClientId, libOption.ClientSecret);
                    }
                    break;
                case EnumTargetType.Custom: {
                        executor = _optionBuilder.ResolveCustomReadExecutorFunc();
                    }
                    break;
                case EnumTargetType.RequestApi:
                default: {
                        var logger = _loggerFactory?.CreateLogger<RequestApiExecutor>();
                        executor = new RequestApiExecutor(logger, libOption.RequestBaseUrl, libOption.ClientId, libOption.ClientSecret);
                    }
                    break;
            }

            return executor;
        }

        internal IWriteTenantExecutor ResolveWriteExecutor() {
            IWriteTenantExecutor executor = null;
            var libOption = _optionBuilder.Option;
            switch (libOption.WriteTargetType) {
                case EnumTargetType.Custom: {
                        executor = _optionBuilder.ResolveCustomWriteExecutorFunc();
                    }
                    break;
                case EnumTargetType.RequestApi:
                default: {
                        if (libOption.EnableRequest) {
                            var logger = _loggerFactory?.CreateLogger<RequestApiExecutor>();
                            executor = new RequestApiExecutor(logger, libOption.RequestBaseUrl, libOption.ClientId, libOption.ClientSecret);

                        }
                    }
                    break;
            }

            return executor;
        }
    }
}
