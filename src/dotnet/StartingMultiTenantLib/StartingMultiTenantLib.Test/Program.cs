using Microsoft.Extensions.DependencyInjection;

namespace StartingMultiTenantLib.Test
{
    public class Program
    {
        public static void Main(string[] args) {
            main(args);
            Console.ReadLine();
        }
        public static async void main(string[] args) {

            string mountSecretFilePath = "/etc/startingmultitenant/tenant-secret";
            string thisServiceIdentifier = "test.svc";

            #region use sdk by Injection

            IServiceCollection services = new ServiceCollection();
            services.AddLogging();
            //Injection invoke client
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

            //Injection tenantStore，if need to implement IMultiTenantStore of Finbuckle.MultiTenant 
            //注入tenantStore,当集成 Finbuckle.MultiTenant 的 IMultiTenantStore时
            services.AddServiceMutiTenantStore(option => {
                //will cache tenant in memory when value >0
                option.CacheMilliSec = 1000 * 60 * 3;
                //bind serviceIdentifier
                option.ServiceIdentifier = thisServiceIdentifier;
            });
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            //now test client
            var startingMultiTenantClient= serviceProvider.GetService<StartingMutilTenantClient>();
            var tenantServiceDbConns=await  startingMultiTenantClient.GetTenantDbConns("abc.com", "testTenant1",thisServiceIdentifier);

            //now test stort
            var contextTenantDomain= serviceProvider.GetService<ContextTenantDomain>();
            //when use Finbuckle.MultiTenant,the tenantDomain will be set by Middleware
            contextTenantDomain.TenantDomain = "abc.com";
            var multiStore= serviceProvider.GetService<ServiceMutiTenantStore>();
            var tenantServiceDbConns2= await  multiStore.TryGetByIdentifierAsync("testTenant1");

            //test cache
            var tenantServiceDbConns3 = await multiStore.TryGetByIdentifierAsync("testTenant1");

            #endregion

            #region use sdk by instance 

            StartingMutilTenantClient smtClient = new StartingMutilTenantClient(StartingMultiTenantLibExtensions.UseRedis("127.0.0.1:6379,password=123456"));
            var tenantServiceDbConns4= smtClient.GetTenantDbConns("abc.com", "testTenant1", thisServiceIdentifier);
            int index = 100;
            smtClient = new StartingMutilTenantClient(StartingMultiTenantLibExtensions.UseRequest("http://localhost:5251", "testClient", "123456"));
            var createResult=await smtClient.CreateTenant("abc.com",$"testTenant{index++}",new List<string>() { "CreateTestDb" },"测试tenant");
            ServiceMutiTenantStore tenantStore1 = new ServiceMutiTenantStore(new ServiceMutiTenantStoreOption(), contextTenantDomain, smtClient, null);
            var tenantServiceDbConns5 = await tenantStore1.TryGetByIdentifierAsync("testTenant1");

            #endregion

        }
    }
}