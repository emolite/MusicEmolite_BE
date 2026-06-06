using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Base
{
    public class BaseTableResponse<T>
    {
        public string? Code { get; set; }
        public string? Type { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages { get; set; }
        public List<T>? Data { get; set; }

        public string? Message { get; set; }
    }
}
