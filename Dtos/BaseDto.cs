using Newtonsoft.Json;
using Volo.Abp.Data;

namespace Corno.Web.Dtos;

public class BaseDto : CornoDto, IHasExtraProperties
{
    public int? SerialNo { get; set; }
    public string Code { get; set; }
    public int Id { get; set; }
    public string Status { get; set; }

    public ExtraPropertyDictionary ExtraProperties { get; set; } = new();
    public string SerializedExtraProperties
    {
        get => JsonConvert.SerializeObject(ExtraProperties);
        set => ExtraProperties = null != value ? JsonConvert.DeserializeObject<ExtraPropertyDictionary>(value) : new();
    }
}