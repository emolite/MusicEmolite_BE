using MS_Application.DataTransferObjects.Albums;
using MS_Application.DataTransferObjects.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.Services.Interfaces
{
    public interface IAlbumsService
    {
        Task<BaseTableResponse<AlbumResponseDto>> GetAlbums(BaseSearchDto<AlbumRequestDto> dto, long userId);
    }
}
