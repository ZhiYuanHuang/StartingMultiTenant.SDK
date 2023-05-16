using System;
using System.Collections.Generic;
using System.Text;

namespace StartingMultiTenantLib
{
    public enum EnumTargetType
    {
        RequestApi=0,
        Redis=1,
        K8sSecret=2,
    }
}
