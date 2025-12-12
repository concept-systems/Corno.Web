using Corno.Web.Models.Packing;
using Corno.Web.Services.Base.Interfaces;

namespace Corno.Web.Services.Packing.Interfaces;

public interface IBaseCartonService : IPrintService<Carton>
{
   /* #region -- Public Methods (Get) --
    Carton GetByBarcode(string barcode);
    Carton GetByBarcode(string barcode, IEnumerable<string> oldStatus, bool checkInDetails = false);
    string GetBarcode(string scannedBarcode);
    Carton GetByCartonNo(int cartonNo);
    int GetNextCartonNoForSoNo(string soNo);
    int GetNextCartonNoForWarehouseOrderNo(string warehouseOrderNo);
    CartonDetail GetBomCartonDetail(Carton carton, Models.Packing.Label label, ICollection<string> labelOldStatus,
        string middleStatus);

    BaseReport GetCartonLabelRpt(Carton carton);

    double GetPackQuantity(List<Carton> cartons, int packingTypeId, int itemId);
    //double GetPackQuantity(string productionOrderNo, int packingTypeId, int itemId);
    #endregion

    #region -- Public Methods (Create) --
    Carton CreateBomCarton(int productId, int packingTypeId);
    Carton CreateCarton(int productId, int packingTypeId);
    Carton CreateNewBomCarton(string barcode, ICollection<string> oldStatus);
    Carton CreateLotCarton(Product product, int packingTypeId, 
        List<string> barcodes, Models.Packing.Label label = null);
    #endregion

    #region -- Public Methods (Update) --
    bool UpdateStatus(string barcode);
    #endregion

    #region -- Public Moethods (Pallet In)
    IEnumerable<Carton> GetPalletCartons(string palletNo, IEnumerable<string> oldStatus,
        string newStatus);
    //List<Carton> PerformPalletIn(AndroidRequest androidRequest, string palletNo,
    //    ICollection<string> oldStatus, bool searchInDetails);
    #endregion

    #region -- Public Moethods (Disptach) --
    void PerformDispatch(string referenceNo, string barcode, string oldStatus);

    #endregion*/
}