using System;
using System.Collections.Generic;
using System.Text;

namespace StartingMultiTenantLib
{
    public class ServiceMutiTenantStoreOption
    {
        public int CacheMilliSec { get; set; }
        public string ServiceIdentifier { get; set; }
        /// <summary>
        /// 当租户不存在时使用空租户数据源
        /// </summary>
        public bool UseEmptySourceWhenNoExistTenant { get; set; }
    }
}
