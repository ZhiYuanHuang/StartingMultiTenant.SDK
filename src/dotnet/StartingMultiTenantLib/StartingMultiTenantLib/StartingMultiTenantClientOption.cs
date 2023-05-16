using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace StartingMultiTenantLib
{
    public class StartingMultiTenantClientOption
    {
        private readonly StartingMultiTenantLibOptionBuilder _optionBuilder;
        private readonly ILoggerFactory _loggerFactory;
        public StartingMultiTenantClientOption(ILoggerFactory loggerFactory,  StartingMultiTenantLibOptionBuilder startingMultiTenantLibOptionBuilder) {
            _optionBuilder=startingMultiTenantLibOptionBuilder;
            _loggerFactory=loggerFactory;
        }

        internal IRequestTenantExecutor ResolveExecutor() {
            IRequestTenantExecutor executor = null;
            var libOption= _optionBuilder.Option;
            switch (libOption.TargetType) {
                case EnumTargetType.Redis: {
                        var logger = _loggerFactory.CreateLogger<IRequestTenantExecutor>();
                        executor = new RedisTenantExecutor(libOption.RedisConnStr,logger,libOption.EnableRequest, libOption.RequestBaseUrl, libOption.ClientId, libOption.ClientSecret);
                    }
                    break;
                case EnumTargetType.K8sSecret: {
                        var logger = _loggerFactory.CreateLogger<IRequestTenantExecutor>();
                        executor = new RequestApiExecutor(logger, libOption.RequestBaseUrl, libOption.ClientId, libOption.ClientSecret);
                    }
                    break;
                case EnumTargetType.RequestApi:
                default: {
                        var logger = _loggerFactory.CreateLogger<IRequestTenantExecutor>();
                        executor = new RequestApiExecutor(logger, libOption.RequestBaseUrl, libOption.ClientId, libOption.ClientSecret);
                    }
                    break;
            }

            return executor;
        }
    }
}
