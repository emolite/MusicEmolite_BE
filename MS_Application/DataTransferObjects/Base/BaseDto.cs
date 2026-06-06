using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Base
{
    public class BaseDto
    {
        public long? Id { get; set; }
        public bool? IsActived { get; set; } = true;
        public bool? IsDeleted { get; set; } = false;

        public DateTime? CreatedAt { get;set; }
        public DateTime? UpdatedAt { get; set; }
        public long? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
    }
}
