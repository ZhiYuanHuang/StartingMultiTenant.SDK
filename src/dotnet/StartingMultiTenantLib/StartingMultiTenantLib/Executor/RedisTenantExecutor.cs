using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StartingMultiTenantLib
{
    internal class RedisTenantExecutor : RequestApiExecutor, IReadTenantExecutor
    {
        private readonly string _connStr;
        private readonly bool _useRequestApi;

        private ConnectionMultiplexer _pubConnection = null;
        private IDatabase _pubDb;

        public bool IsConnected { get => _pubConnection?.IsConnected ?? false; }

        private const string _redisHashKeyTemplate = "{0}:{1}:DbConns";
         
        private readonly ILogger<RedisTenantExecutor> _logger;
        private readonly object _lockObj;

        public RedisTenantExecutor(string connStr,ILogger<RedisTenantExecutor> logger,bool useRequestApi,
            string baseUrl=null, string clientId = null, string clientSecret = null) 
            : base(logger, baseUrl, clientId, clientSecret) {
            _logger = logger;
            _connStr = connStr;
            _useRequestApi = useRequestApi;
            _lockObj = new object();
        }

        public override async Task<TenantDbConnsDto> GetTenantDbConns(string tenantDomain, string tenantIdentifier, string serviceIdentifier) {
            
            var result = await getFromRedis(tenantDomain, tenantIdentifier, serviceIdentifier);
            if (result == null && _useRequestApi) {
                return await base.GetTenantDbConns(tenantDomain, tenantIdentifier, serviceIdentifier);
            }
            return result;
        }

        private ConnectionMultiplexer createConnection() {
            if (_pubConnection == null) {
                lock (_lockObj) {
                    if (_pubConnection == null) {
                        _pubConnection = ConnectionMultiplexer.Connect(_connStr);
                        _pubDb = _pubConnection.GetDatabase();
                    }
                }
            }

            return _pubConnection;
        }

        private async Task<TenantDbConnsDto> getFromRedis(string tenantDomain, string tenantIdentifier, string serviceIdentifier) {
            string redisHashKey = string.Format(_redisHashKeyTemplate,tenantDomain, tenantIdentifier);
            TenantDbConnsDto tenantDbConnsDto = null;
            try {
                createConnection();

                var hashEntryArr= await _pubDb.HashGetAllAsync(redisHashKey);
                if (hashEntryArr.Length == 0) {
                    ////注释冗余redis不存在，回源查找
                    //tenantDbConnsDto = new TenantDbConnsDto() {
                    //    NoExist = true,
                    //    TenantDomain = tenantDomain,
                    //    TenantIdentifier = tenantIdentifier,
                    //    InnerDbConnList = new List<TenantDbConnDto>(),
                    //    ExternalDbConnList = new List<TenantDbConnDto>()
                    //};


                } else {
                    tenantDbConnsDto = new TenantDbConnsDto() {
                        TenantDomain = tenantDomain,
                        TenantIdentifier = tenantIdentifier,
                        InnerDbConnList = new List<TenantDbConnDto>(),
                        ExternalDbConnList = new List<TenantDbConnDto>()
                    };

                    foreach (var hashEntry in hashEntryArr) {
                        string fieldName = hashEntry.Name;
                        string dbConnValue= hashEntry.Value.ToString();
                        if (string.IsNullOrEmpty(dbConnValue)) {
                            continue;
                        }
                        if (!string.IsNullOrEmpty(fieldName)) {
                            string[] fieldNameArr= fieldName.Split('_');
                            if (fieldNameArr.Length == 3) {
                                if(!string.IsNullOrEmpty(serviceIdentifier) && string.Compare(fieldNameArr[1], serviceIdentifier, true) != 0) {
                                    continue;
                                }
                                
                                if (string.Compare(fieldNameArr[0], "Inner", true)==0) {
                                    tenantDbConnsDto.InnerDbConnList.Add(new TenantDbConnDto() { 
                                        ServiceIdentifier = serviceIdentifier,
                                        DbIdentifier=fieldNameArr[2],
                                        DbConn=dbConnValue
                                    });
                                }else if (string.Compare(fieldNameArr[0], "External", true) == 0) {
                                    tenantDbConnsDto.ExternalDbConnList.Add(new TenantDbConnDto() {
                                        ServiceIdentifier = serviceIdentifier,
                                        DbIdentifier = fieldNameArr[2],
                                        DbConn = dbConnValue
                                    });
                                }
                            }
                        }
                    }
                }
            }catch(Exception ex) {
                _logger?.LogError($"getFromRedis raise error,ex:{ex.Message}");
            }

            return tenantDbConnsDto;
        }
    }
}
