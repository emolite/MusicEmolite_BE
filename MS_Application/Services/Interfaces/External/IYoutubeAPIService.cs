using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Youtube;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.Services.Interfaces.External
{
    public interface IYoutubeAPIService
    {
        Task<BaseTableResponse<YoutubeVideoDto>> SearchAsync(BaseSearchDto<YoutubeSearchRequestDto> request, long userId);
    }
}
