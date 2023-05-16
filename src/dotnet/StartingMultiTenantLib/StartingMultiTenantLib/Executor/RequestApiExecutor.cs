using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace StartingMultiTenantLib
{
    public class RequestApiExecutor: IRequestTenantExecutor
    {
        private const string RelateGetTenantDbConnUrl = "/api/tenantcenter/GetDbConn";
        private const string RelateGetTokenUrl = "/api/connect/token";

        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _baseUrl;
        private readonly string _getTenantDbConnUrl = string.Empty;
        private readonly string _getTokenUrl = string.Empty;
        private readonly string _cacheKey_token=string.Empty;

        private readonly HttpClient _httpClient;
        protected readonly ILogger<IRequestTenantExecutor> _logger;

        public RequestApiExecutor(ILogger<IRequestTenantExecutor> logger,string baseUrl, string clientId, string clientSecret) { 
            _clientId= clientId;
            _clientSecret= clientSecret;
            _cacheKey_token = string.Format("StartingMultiTenant_Token_{0}",clientId);
            _baseUrl = baseUrl;
            _getTenantDbConnUrl = string.Concat(_baseUrl, RelateGetTenantDbConnUrl);
            _getTokenUrl = string.Concat(_baseUrl,RelateGetTokenUrl);
            _httpClient = new HttpClient();
            _logger = logger;
        }

        public virtual async Task<TenantDbConnsDto> GetTenantDbConns(string tenantDomain, string tenantIdentifier, string serviceIdentifier) {
            string tokne =await getToken();

            string queryStr = $"tenantDomain={tenantDomain}&tenantIdentifier={tenantIdentifier}";
            if (!string.IsNullOrEmpty(serviceIdentifier)) {
                queryStr += $"serviceIdentifier={serviceIdentifier}";
            }

            string url = string.Concat(_getTenantDbConnUrl,"?", queryStr);
            HttpRequestMessage httpRequestMessage= new HttpRequestMessage(HttpMethod.Get, url);
            var resp= await _httpClient.SendAsync(httpRequestMessage);
            resp.EnsureSuccessStatusCode();

            string respContent= await resp.Content.ReadAsStringAsync();
            var appResp = Newtonsoft.Json.JsonConvert.DeserializeObject<AppResponseDto<TenantDbConnsDto>>(respContent);
            if (appResp == null || appResp.ErrorCode != 0 || appResp.Result==null) {
                _logger?.LogError($"GetTenantDbConns raise error,resp content:{respContent}");
                return null;
            }

            return appResp.Result;
        }

        private async Task<string> getToken() {
            if (MemoryCacheHelper.Contains(_cacheKey_token, out string token)) {
                return token;
            }

            AppRequestDto<ApiClientDto> appRequestDto = new AppRequestDto<ApiClientDto>() { 
                Data=new ApiClientDto() { 
                    ClientId= _clientId,
                    ClientSecret= _clientSecret,
                },
            };

            var jsonContent = JsonContent.Create(appRequestDto);
            var resp= await _httpClient.PostAsync(_getTokenUrl,jsonContent);
            resp.EnsureSuccessStatusCode();
            string respContent= await resp.Content.ReadAsStringAsync();
            var appResp= Newtonsoft.Json.JsonConvert.DeserializeObject<AppResponseDto<string>>(respContent);
            if(appResp==null || appResp.ErrorCode!=0 || string.IsNullOrEmpty(appResp.Result)) {
                _logger?.LogError($"get token from remote raise error,resp content:{respContent}");
                return null;
            }

            MemoryCacheHelper.Set(_cacheKey_token,1000*60*60*12,appResp.Result);

            return appResp.Result;
        }
    }
}
