using System;
using System.Collections.Generic;
using System.Text;

namespace StartingMultiTenantLib
{
    public class StartingMultiTenantLibOption
    {
        public bool EnableRequest { get; set; }
        
        public EnumTargetType TargetType { get; set; } = EnumTargetType.RequestApi;
        internal string RequestBaseUrl { get; set; }
        internal string RedisConnStr { get; set; }
        internal string K8sSecretFilePath { get;  set; }


        //public void UseRequest(string baseUrl) {
        //    _enableRequest = true;
        //    if (!string.IsNullOrEmpty(baseUrl)) {
        //        if (baseUrl.EndsWith('/')) {
        //            baseUrl= baseUrl.Substring(0, baseUrl.Length - 1);
        //        }
                
        //        _requestBaseUrl= baseUrl;
        //    }
        //}

        //public void UseRedis(string connStr) {
        //    _enableTargetType= EnumTargetType.Redis;
        //    _requestBaseUrl= connStr;
        //}

        //public void UseK8sSecret(string secretFilePath) {
        //    _enableTargetType = EnumTargetType.K8sSecret;
        //    _k8sSecretFilePath = secretFilePath;
        //}
    }
}
