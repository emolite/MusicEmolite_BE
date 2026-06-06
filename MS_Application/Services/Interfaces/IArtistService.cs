using MS_Application.DataTransferObjects.Artist;
using MS_Application.DataTransferObjects.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.Services.Interfaces
{
    public interface IArtistService
    {
        Task<BaseTableResponse<ArtistResponseDto>> GetArtists(BaseSearchDto<ArtistRequestDto> dto);

        Task<BaseResponse<ArtistResponseDto>> CreateArtist(ArtistCreateDto dto, long userId);

        Task<BaseResponse<ArtistResponseDto>> UpdateArtist(long id, ArtistUpdateDto dto, long userId);
    }
}
