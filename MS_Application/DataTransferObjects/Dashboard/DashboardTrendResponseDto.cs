using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Dashboard
{
    public class DashboardTrendResponseDto
    {
        public DateTime Date { get; set; }

        public long Views { get; set; }

        public long Likes { get; set; }

        public long Users { get; set; }
    }
}
