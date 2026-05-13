using MS_Application.Constants;
using MS_Application.DataTransferObjects.Artist;
using MS_Application.DataTransferObjects.Base;
using MS_Application.Helpers;
using MS_Application.Repositories.Interfaces;
using MS_Application.Services.Interfaces;
using MS_Domain.Entities.DISTS;

namespace MS_Application.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IDistUnitOfWork _distUnitOfWork;

        public ArtistService(IDistUnitOfWork distUnitOfWork)
        {
            _distUnitOfWork = distUnitOfWork;
        }

        public async Task<BaseTableResponse<ArtistResponseDto>> GetArtists(BaseSearchDto<ArtistRequestDto> dto)
        {
            var result = new BaseTableResponse<ArtistResponseDto>();

            var repo = _distUnitOfWork
                .GetRepositoryReadOnlyAsync<DistArtists>()
                .QueryAll();

            var query = repo.Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(dto.SearchParams.Keyword))
            {
                query = query.Where(x =>
                    x.Name.Contains(dto.SearchParams.Keyword) ||
                    x.StageName.Contains(dto.SearchParams.Keyword));
            }

            var totalRecords = query.Count();

            var data = query
                .OrderByDescending(x => x.CreatedAt)
                .Skip(dto.Start)
                .Take(dto.PageSize)
                .Select(x => new ArtistResponseDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    StageName = x.StageName,
                    Country = x.Country,
                    IsActived = x.IsActived,
                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt
                })
                .ToList();

            result.TotalRecords = totalRecords;
            result.Data = data;
            result.Code = ResponseStatusCode.Status200;

            return result.Success(string.Format(Messages.Action.GetSuccess, "artists"));
        }

        public async Task<BaseResponse<ArtistResponseDto>> CreateArtist(ArtistCreateDto dto, long userId)
        {
            var result = new BaseResponse<ArtistResponseDto>();

            var repo = _distUnitOfWork
                .GetRepositoryAsync<DistArtists>();

            var entity = new DistArtists
            {
                Name = dto.Name,
                StageName = dto.StageName,
                Country = dto.Country,
                CreatedBy = userId
            };

            await repo.AddAsync(entity);
            await _distUnitOfWork.SaveChangesAsync();

            result.Data = new ArtistResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                StageName = entity.StageName,
                Country = entity.Country,
                CreatedAt = entity.CreatedAt
            };

            return result.Success(
                string.Format(Messages.Action.CreateSuccess, "artist"));
        }

        public async Task<BaseResponse<ArtistResponseDto>> UpdateArtist(long id, ArtistUpdateDto dto, long userId)
        {
            var result = new BaseResponse<ArtistResponseDto>();

            var repo = _distUnitOfWork
                .GetRepositoryAsync<DistArtists>();

            var artist = await repo.FindByIdAsync(id);

            if (artist == null || artist.IsDeleted)
            {
                return result.Fail(string.Format(Messages.Validation.NotFound, "artist"));
            }

            artist.Name = dto.Name;
            artist.StageName = dto.StageName;
            artist.Country = dto.Country;
            artist.UpdatedBy = userId;
            artist.UpdatedAt = DateTime.UtcNow;

            await repo.UpdateAsync(artist);
            await _distUnitOfWork.SaveChangesAsync();

            result.Data = new ArtistResponseDto
            {
                Id = artist.Id,
                Name = artist.Name,
                StageName = artist.StageName,
                Country = artist.Country,
                CreatedAt = artist.CreatedAt
            };

            return result.Success(
                string.Format(Messages.Action.UpdateSuccess, "artist"));
        }
    }
}