using System;
using System.Collections.Generic;
using System.Text;

namespace StartingMultiTenantLib
{
    internal class AppResponseDto
    {
        public string ClientName { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMsg { get; set; }
        public AppRequestDto Request { get; set; }

        public AppResponseDto() {
        }

        public AppResponseDto(bool result) {
            ErrorCode = result ? 0 : -1;
        }
    }

    internal class AppResponseDto<T> : AppResponseDto
    {
        public AppResponseDto() : base() {
        }
        public AppResponseDto(bool result) : base(result) {
        }
        public List<T>? ResultList { get; set; }
        public T Result { get; set; }
    }
}
