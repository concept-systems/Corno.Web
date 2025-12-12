using System;
using System.Linq;
using Corno.Web.Areas.Kitchen.Dto.Put_To_Light;
using Corno.Web.Areas.Kitchen.Models.Put_To_Light;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services;

namespace Corno.Web.Areas.Kitchen.Services;

public class TrolleyConfigService : BaseService<TrolleyConfig>, ITrolleyConfigService
{
    #region -- Constructors --
    public TrolleyConfigService(IGenericRepository<TrolleyConfig> genericRepository) :base (genericRepository)
    {
        //using var scope = Bootstrapper.StaticContainer.BeginScope();
        SetIncludes(nameof(TrolleyConfig.TrolleyLightDetails));
    }
    #endregion

    #region -- Public Methods --
    public void ValidateDto(TrolleyConfigIndexDto dto)
    {
        if (dto.TrolleyLightDetailDtos.Count < 0)
            throw new Exception("No location is added.");

        // Check for same location has multiple colors
        var duplicateLocations = dto.TrolleyLightDetailDtos.GroupBy(x => x.LocationId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateLocations.Count > 0)
            throw new Exception($"Locations ({string.Join(",", duplicateLocations)}) have multiple colors.");
    }
    #endregion
}