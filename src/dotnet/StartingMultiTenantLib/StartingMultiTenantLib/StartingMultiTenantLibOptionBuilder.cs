using System;
using System.Collections.Generic;
using System.Text;

namespace StartingMultiTenantLib
{
    public class StartingMultiTenantLibOptionBuilder
    {
        internal StartingMultiTenantLibOption Option;
        public StartingMultiTenantLibOptionBuilder(StartingMultiTenantLibOption option) { 
            Option = option;
        }

        public StartingMultiTenantLibOptionBuilder UseRequest(string baseUrl,string clientId,string clientSecret) {
            Option.EnableRequest = true;
            Option.ClientId = clientId;
            Option.ClientSecret = clientSecret;
            if (!string.IsNullOrEmpty(baseUrl)) {
                if (baseUrl.EndsWith('/')) {
                    baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
                }

                Option.RequestBaseUrl = baseUrl;
            }
            return this;
        }

        public StartingMultiTenantLibOptionBuilder UseRedis(string connStr) {
            Option.TargetType = EnumTargetType.Redis;
            Option.RedisConnStr= connStr;
            return this;
        }

        public StartingMultiTenantLibOptionBuilder UseK8sSecret(string secretFilePath) {
            Option.TargetType = EnumTargetType.K8sSecret;
            Option.K8sSecretFilePath= secretFilePath;
            return this;
        }
    }
}
