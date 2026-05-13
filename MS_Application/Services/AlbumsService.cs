using MS_Application.Constants;
using MS_Application.DataTransferObjects.Albums;
using MS_Application.DataTransferObjects.Base;
using MS_Application.Helpers;
using MS_Application.Repositories.Interfaces;
using MS_Application.Services.Interfaces;
using MS_Domain.Entities.DISTS;
using MS_Domain.Enums;

namespace MS_Application.Services
{
    public class AlbumsService : IAlbumsService
    {
        private readonly IDistUnitOfWork _distUnitOfWork;

        public AlbumsService(IDistUnitOfWork distUnitOfWork)
        {
            _distUnitOfWork = distUnitOfWork;
        }

        public async Task<BaseTableResponse<AlbumResponseDto>> GetAlbums(BaseSearchDto<AlbumRequestDto> dto, long userId)
        {
            var result = new BaseTableResponse<AlbumResponseDto>();

            var repoAlbum = _distUnitOfWork.GetRepositoryReadOnlyAsync<DistAlbums>().QueryAll();

            var query = repoAlbum.Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(dto.SearchParams.Keyword))
            {
                query = query.Where(x =>
                    x.Title.Contains(dto.SearchParams.Keyword));
            }

            if (dto.SearchParams.AlbumType.HasValue)
            {
                query = query.Where(x => x.AlbumType == dto.SearchParams.AlbumType);
            }

            if (dto.SearchParams.AlbumType == 1)
            {
                query = query.Where(x =>x.CreatedBy == userId);
            }

            var totalRecords = query.Count();

            var data = query
                .OrderByDescending(x => x.CreatedAt)
                .Skip(dto.Start)
                .Take(dto.PageSize)
                .Select(x => new AlbumResponseDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    ReleaseDate = x.ReleaseDate,
                    ArtistId = x.ArtistId,
                    AlbumTypeName = EnumHelper.GetDisplayName((AlbumType)x.AlbumType),
                    IsActived = x.IsActived,
                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy
                })
                .ToList();

            result.TotalRecords = totalRecords;
            result.Data = data;
            result.Code = ResponseStatusCode.Status200;

            return result.Success(string.Format(Messages.Action.GetSuccess, "albums"));
        }
    }
}