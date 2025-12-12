using Corno.Models.ViewModels;

namespace Corno.Concept.Portal.Areas.BoardStore.Models;

public class MasterViewModel : BaseViewModel
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string NameWithCode { get; set; }

    public string NameWithId { get; set; }

}