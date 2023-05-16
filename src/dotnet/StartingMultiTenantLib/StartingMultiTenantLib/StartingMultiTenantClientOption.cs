using System;
using System.Collections.Generic;
using System.Text;

namespace StartingMultiTenantLib
{
    public class StartingMultiTenantClientOption
    {
        private readonly StartingMultiTenantLibOptionBuilder _optionBuilder;
        public StartingMultiTenantClientOption(StartingMultiTenantLibOptionBuilder startingMultiTenantLibOptionBuilder) {
            _optionBuilder=startingMultiTenantLibOptionBuilder;
        }

        internal IRequestTenantExecutor ResolveExecutor() {
            IRequestTenantExecutor executor = null;
            var libOption= _optionBuilder.Option;
            switch (libOption.TargetType) {
                case EnumTargetType.RequestApi:
                default:
                    executor = new RequestApiExecutor(libOption.RequestBaseUrl);
                    break;
            }

            return executor;
        }
    }
}
