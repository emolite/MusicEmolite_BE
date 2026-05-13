using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Base
{
    public class BaseResponse<T>
    {
        public string? Code { get; set; }

        public string? Type { get; set; }

        public string? Message { get; set; } = string.Empty;

        public int? TotalRecords { get; set; } = 0;

        public int? TotalPages { get; set; } = 0;

        public T? Data { get; set; }
    }
}
