using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Dashboard
{
    public class DashboardSummaryResponseDto
    {
        public long TotalViews { get; set; }

        public long Last30DaysViews { get; set; }

        public long TotalLikes { get; set; }

        public long Last30DaysLikes { get; set; }

        public long TotalUsers { get; set; }

        public long Last30DaysUsers { get; set; }
    }
}
