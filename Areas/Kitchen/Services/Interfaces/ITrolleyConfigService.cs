using Corno.Web.Areas.Kitchen.Dto.Put_To_Light;
using Corno.Web.Areas.Kitchen.Models.Put_To_Light;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Areas.Kitchen.Services.Interfaces;

public interface ITrolleyConfigService : IBaseService<TrolleyConfig>
{
    void ValidateDto(TrolleyConfigIndexDto dto);
}