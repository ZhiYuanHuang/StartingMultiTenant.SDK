using System;
using System.Collections.Generic;
using System.Text;

namespace StartingMultiTenantLib
{
    internal class AppRequestDto
    {
        public long RequestId { get; set; }
        public string? RequestObj { get; set; }
    }

    internal class AppRequestDto<T> : AppRequestDto
    {
        public List<T>? DataList { get; set; }
        public T Data { get; set; }
    }
}
