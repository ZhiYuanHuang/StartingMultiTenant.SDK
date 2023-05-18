# StartingMultiTenant.SDK
[StartingMultiTenant](https://github.com/ZhiYuanHuang/StartingMultiTenant) 的sdk项目，提供各个语言的sdk，方便接入


## dotnet sdk

(1)StartingMutilTenantClient
(2)ServiceMutiTenantStore

### 调用示例

#### 依赖注入方式
```
 //注入StartingMutilTenantClient
 services.AddStartingMultiTenantClient((provider, builder) =>{
                //use http request and redis access tenant
                builder.UseRequest("http://localhost:5251", "testClient", "123456")
                    .UseRedis("127.0.0.1:6379,password=123456");
                ////or http and k8s
                //string mountSecretFilePath = "/etc/startingmultitenant/tenant-secret";
                //builder.UseRequest("http://localhost:5251", "sysClient", "123456")
                //    .UseK8sSecret(mountSecretFilePath);
                ////or only http
                //builder.UseRequest("http://localhost:5251", "sysClient", "123456");
                ////or only redis
                //builder.UseRedis("127.0.0.1:6379,password=123456");
                ////or only k8s
                //builder.UseK8sSecret(mountSecretFilePath);
            });

 //invoke 
 var tenantServiceDbConns=await  startingMultiTenantClient.GetTenantDbConns("abc.com", "testTenant1",thisServiceIdentifier);

 //ServiceMutiTenantStore
 services.AddServiceMutiTenantStore(option => {
                //will cache tenant in memory when value >0
                option.CacheMilliSec = 1000 * 60 * 3;
                //bind serviceIdentifier
                option.ServiceIdentifier = thisServiceIdentifier;
            });
 var tenantServiceDbConns2= await  multiStore.TryGetByIdentifierAsync("testTenant1");
```

#### 实例化方式
```
//实例化 StartingMutilTenantClient
StartingMutilTenantClient smtClient = new StartingMutilTenantClient(StartingMultiTenantLibExtensions.UseRedis("127.0.0.1:6379,password=123456"));
//调用获取租户
var tenantServiceDbConns4= smtClient.GetTenantDbConns("abc.com", "testTenant1", thisServiceIdentifier);
            
smtClient = new StartingMutilTenantClient(StartingMultiTenantLibExtensions.UseRequest("http://localhost:5251", "testClient", "123456"));
//调用创建租户
var createResult=await smtClient.CreateTenant("abc.com",$"testTenant{index++}",new List<string>() { "CreateTestDb" },"测试tenant");

//实例化 ServiceMutiTenantStore
ServiceMutiTenantStore tenantStore1 = new ServiceMutiTenantStore(new ServiceMutiTenantStoreOption(), contextTenantDomain, smtClient, null);
//调用获取租户
var tenantServiceDbConns5 = await tenantStore1.TryGetByIdentifierAsync("testTenant1");
```
