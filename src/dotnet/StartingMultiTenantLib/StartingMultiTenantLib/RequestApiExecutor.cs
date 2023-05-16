using System;
using System.Collections.Generic;
using System.Text;

namespace StartingMultiTenantLib
{
    public class RequestApiExecutor: IRequestTenantExecutor
    {
        private const string RelateGetTenantDbConnUrl = "/api/tenantcenter/GetDbConn";

        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _baseUrl;
        private readonly string _getTenantDbConnUrl = string.Empty;
        public RequestApiExecutor(string clientId,string clientSecret,string baseUrl) { 
            _clientId= clientId;
            _clientSecret= clientSecret;
            _baseUrl = baseUrl;
            _getTenantDbConnUrl = string.Concat(_baseUrl, RelateGetTenantDbConnUrl);
        }

         
    }
}
