using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using StartingMultiTenantLib.Executor;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace StartingMultiTenantLib
{
    internal class RequestApiExecutor: IReadTenantExecutor,IWriteTenantExecutor
    {
        private const string RelateGetTenantDbConnUrl = "/api/tenantcenter/GetDbConn";
        private const string RelateGetTokenUrl = "/api/connect/token";
        private const string RelateCreateTenant = "/api/tenantcenter/create";

        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _baseUrl;
        private readonly string _getTenantDbConnUrl = string.Empty;
        private readonly string _getTokenUrl = string.Empty;
        private readonly string _createTenantUrl = string.Empty;
        private readonly string _cacheKey_token=string.Empty;

        private readonly HttpClient _httpClient;
        private readonly ILogger<RequestApiExecutor> _logger;

        public RequestApiExecutor(ILogger<RequestApiExecutor> logger,string baseUrl, string clientId, string clientSecret) { 
            _clientId= clientId;
            _clientSecret= clientSecret;
            _cacheKey_token = string.Format("StartingMultiTenant_Token_{0}",clientId);
            _baseUrl = baseUrl;
            _getTenantDbConnUrl = string.Concat(_baseUrl, RelateGetTenantDbConnUrl);
            _getTokenUrl = string.Concat(_baseUrl,RelateGetTokenUrl);
            _createTenantUrl = string.Concat(_baseUrl,RelateCreateTenant);
            _httpClient = new HttpClient();
            _logger = logger;
        }

        public virtual async Task<TenantDbConnsDto> GetTenantDbConns(string tenantDomain, string tenantIdentifier, string serviceIdentifier) {
            string token =await getToken();

            string queryStr = $"tenantDomain={tenantDomain}&tenantIdentifier={tenantIdentifier}";
            if (!string.IsNullOrEmpty(serviceIdentifier)) {
                queryStr += $"&serviceIdentifier={serviceIdentifier}";
            }

            string url = string.Concat(_getTenantDbConnUrl,"?", queryStr);
            HttpRequestMessage httpRequestMessage= new HttpRequestMessage(HttpMethod.Get, url);
            httpRequestMessage.Headers.Add("Authorization",$"Bearer {token}");
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

        public async Task<CreateTenantResultDto> CreateTenant(string tenantDomain, string tenantIdentifier, List<string> createDbScriptNameList=null, string tenantName = null, string description = null) {
            string token = await getToken();

            AppRequestDto<CreateTenantDto> appRequestDto = new AppRequestDto<CreateTenantDto>() {
                Data = new CreateTenantDto() {
                    TenantDomain = tenantDomain,
                    TenantIdentifier = tenantIdentifier,
                    TenantName = tenantName,
                    Description=description,
                    CreateDbScripts = createDbScriptNameList != null ? createDbScriptNameList:(new List<string>()),
                }
            };
            var jsonContent= JsonContent.Create(appRequestDto);

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,_createTenantUrl);
            httpRequestMessage.Content = jsonContent;
            httpRequestMessage.Headers.Add("Authorization", $"Bearer {token}");
            var resp=await _httpClient.SendAsync(httpRequestMessage);
            resp.EnsureSuccessStatusCode();
            string respContent=await resp.Content.ReadAsStringAsync();
            var appResp = Newtonsoft.Json.JsonConvert.DeserializeObject<AppResponseDto>(respContent);
            if(appResp==null || appResp.ErrorCode!=0) {
                return new CreateTenantResultDto() { Success=false,ErrMsg=appResp?.ErrorMsg};
            }

            return new CreateTenantResultDto() { Success=true};
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
