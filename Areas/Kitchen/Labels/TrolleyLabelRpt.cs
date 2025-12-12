using System.Collections.Generic;
using System.Linq;
using Corno.Web.Models.Packing;
using Corno.Web.Reports;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Windsor;

namespace Corno.Web.Areas.Kitchen.Labels;

public partial class TrolleyLabelRpt : BaseReport
{
    #region -- Constructors --
    public TrolleyLabelRpt(IEnumerable<Label> labels)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        if (!labels.Any())
            return;

        var itemService = Bootstrapper.Get<IBaseItemService>();
        var itemIds = labels.Select(l => l.ItemId).Distinct();
        var items = RunAsync(() => itemService.GetAsync(i => itemIds.Contains(i.Id), i => new { i.Id, i.Name, i.Reserved2 }));

        var dataSource = from label in labels
            join item in items on label.ItemId equals item.Id
            select new
            {
                label.SerialNo,
                label.WarehouseOrderNo,
                label.SoNo,
                label.CarcassCode,
                label.Barcode,
                BaanItemCode = item.Reserved2
            };

        DataSource = dataSource;
    }

    #endregion
}