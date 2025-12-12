using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Corno.Web.Models.Packing;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Base;
using Corno.Web.Services.Packing.Interfaces;

namespace Corno.Web.Services.Packing;

public class BaseCartonService : PrintService<Carton>, IBaseCartonService
{
    #region -- Constructors --

    public BaseCartonService(IGenericRepository<Carton> genericRepository)
        : base(genericRepository)
    {
        /*//_cartonDetailService = Bootstrapper.Get<ICartonDetailService>();
        _labelService = Bootstrapper.Get<IBaseLabelService>();
        _productService = Bootstrapper.Get<IProductService>();*/

        SetIncludes($"{nameof(Carton.CartonDetails)}," +
                    $"{nameof(Carton.CartonRackingDetails)}");
    }
    #endregion

    /*#region -- Data Members --

    private readonly IBaseLabelService _labelService;
    private readonly IProductService _productService;

    #endregion

    #region -- Private Methods --
    private async Task<Models.Packing.Label> AddCartonDetailAsync(Carton carton, string barcode,
        ICollection<string> oldStatus)
    {
        var barcodeLabel = await _labelService.GetByBarcodeAsync(barcode, oldStatus);

        carton.CartonDetails.Add(new CartonDetail
        {
            ProductId = barcodeLabel.ProductId,
            ItemId = barcodeLabel.ItemId,
            Barcode = barcodeLabel.Barcode,
            SystemWeight = barcodeLabel.SystemWeight,
            NetWeight = barcodeLabel.NetWeight,
            BomQuantity = (int)(barcodeLabel.Quantity ?? 0),
            Quantity = barcodeLabel.Quantity,
            PackingTypeId = barcodeLabel.PackingTypeId
        });

        // Update barcode log
        barcodeLabel.Status = StatusConstants.Packed;
        await _labelService.UpdateAsync(barcodeLabel);

        return barcodeLabel;
    }
    #endregion

    #region -- Protected Methods --
    *//*protected override LinqKit.ExpressionStarter<Carton> GetExtraPredicate()
    {
        var predicate = base.GetExtraPredicate();
        var fromDate = DateTime.Now.AddMonths(-3);
        predicate.And(p => p.PackingDate >= fromDate);

        return predicate;
    }*//*
    #endregion

    #region -- Public Methods (Get) --

    public async Task<Carton> GetByBarcodeAsync(string barcode)
    {
        var carton = await FirstOrDefaultAsync(c => c.CartonBarcode == barcode, c => c).ConfigureAwait(false);
        if (null == carton)
            throw new Exception("Carton with this barcode does not exist.");
        return carton;
    }

    public async Task<Carton> GetByBarcodeAsync(string barcode, IEnumerable<string> oldStatus,
        bool checkInDetails = false)
    {
        if (string.IsNullOrEmpty(barcode))
            throw new Exception("Invalid Barcode");
        // Check for valid Customer / Order No.
        Carton carton = null;
        if (checkInDetails)
            carton = await FirstOrDefaultAsync(b => b.CartonDetails.Any(d => d.Barcode == barcode), b => b).ConfigureAwait(false);

        carton ??= await FirstOrDefaultAsync(b => b.CartonBarcode == barcode, b => b).ConfigureAwait(false);
        if (null == carton)
            throw new Exception("Carton with barcode value ('" + barcode + ") does not available in system.");

        if (!oldStatus.Contains(carton.Status))
            throw new Exception($"Barcode {barcode} should have status : " +
                                $"{string.Join(",", oldStatus)}. " +
                                $"It has current status : {carton.Status}");

        return carton;
    }

    public virtual string GetBarcode(string scannedBarcode)
    {
        return scannedBarcode;
    }

    public Carton GetByCartonNo(int cartonNo)
    {
        return FirstOrDefault(c => c.CartonNo == cartonNo , c => c);
    }

    public int GetNextCartonNoForSoNo(string soNo)
    {
        return Max(c => c.SoNo == soNo, c => c.CartonNo) + 1;
    }

    public int GetNextCartonNoForWarehouseOrderNo(string warehouseOrderNo)
    {
        return Max(c => c.WarehouseOrderNo == warehouseOrderNo ,
            c => c.CartonNo) + 1;
    }

    protected int GetNextCartonNoForProductionOrderNo(string productionOrderNo)
    {
        return Max(c => c.ProductionOrderNo == productionOrderNo ,
            c => c.CartonNo) + 1;
    }

    public virtual CartonDetail GetBomCartonDetail(Carton carton, Models.Packing.Label label,
        ICollection<string> labelOldStatus, string middleStatus)
    {
        return null;
    }

    public virtual BaseReport GetCartonLabelRpt(Carton carton)
    {
        // Do Nothing
        return null;
    }

    public double GetPackQuantity(List<Carton> cartons, int packingTypeId, int itemId)
    {
        var quantities = cartons.SelectMany(d => d.CartonDetails.Where(x => x.PackingTypeId == packingTypeId && x.ItemId == itemId), 
            (_, d) => d.Quantity);
        var packQuantity = quantities.Sum(d => d);
        return packQuantity ?? 0;
    }

    *//*public double GetPackQuantity(string productionOrderNo, int packingTypeId, int itemId)
    {
        var quantities = from carton in Get(c =>
                c.ProductionOrderNo == productionOrderNo, c =>
                new { c.Id })
            join cartonDetail in _cartonDetailService.Get(d =>
                        d.PackingTypeId == packingTypeId && d.ItemId == itemId, 
                    d => new { d.CartonId, d.Quantity })
                on carton.Id equals cartonDetail?.CartonId
            select cartonDetail?.Quantity;
        var packQuantity = quantities.Sum(d => d);
        return packQuantity ?? 0;
    }*//*

    #endregion

    #region -- Protected Methods (Create)

    protected virtual int GenerateCartonNo()
    {
        return GetNextSequence(FieldConstants.CartonSerialNo);
    }

    #endregion

    #region -- Public Methods (Create) --

    public virtual async Task<Carton> CreateCartonAsync(int productId, int packingTypeId)
    {
        var product = await _productService.GetByIdAsync(productId).ConfigureAwait(false);
        if (null == product)
            throw new Exception("Invalid product.");
        if (!product.ProductItemDetails.Any())
            throw new Exception("BOM is not created for selected product");

        if (packingTypeId <= 0)
            throw new Exception("Invalid packing type");

        var productItems = product.ProductItemDetails
            .Where(productItem => productItem.PackingTypeId == packingTypeId);

        var cartonNo = GetNextSequence(FieldConstants.CartonSerialNo);
        var carton = new Carton
        {
            SerialNo = cartonNo,
            PackingDate = DateTime.Now,
            PackingTypeId = packingTypeId,
            ProductId = product.Id,
            CartonNo = cartonNo,

            CartonDetails = productItems
                .SelectMany(productItem => Enumerable.Range(0, (int)(productItem.Quantity ?? 0))
                    .Select(_ => new CartonDetail
                    {
                        ItemId = productItem.ItemId,
                        Barcode = string.Empty,
                        SystemWeight = (productItem.SystemWeight ?? 0) / (productItem.Quantity ?? 1),
                        Tolerance = productItem.Tolerance ?? 0,
                        BomQuantity = (int)(productItem.Quantity ?? 0),
                        Quantity = 0,
                        PackingTypeId = productItem.PackingTypeId,
                        Layer = productItem.Layer,
                        PackingSequence = productItem.PackingSequence
                    })).ToList()
        };

        return carton;
    }

    
    public virtual Carton CreateBomCarton(int productId, int packingTypeId)
    {
        return CreateCarton(productId, packingTypeId);
    }

    public virtual Carton CreateNewBomCarton(string barcode, ICollection<string> oldStatus)
    {
        return null;
    }

    
    public virtual async Task<Carton> CreateLotCartonAsync(Product product, int packingTypeId,
        List<string> barcodes, Models.Packing.Label label = null)
    {
        var labelsList = await _labelService.GetAsync(b =>
            barcodes.Contains(b.Barcode), b => new
        {
            b.Code,
            b.BatchNo,
            b.ItemId,
            b.Barcode,
            b.SystemWeight,
            b.GrossWeight,
            b.NetWeight,
        }).ConfigureAwait(false);

        var cartonNo = GenerateCartonNo();
        var carton = new Carton
        {
            SerialNo = cartonNo,
            PackingDate = DateTime.Now,
            PackingTypeId = packingTypeId,
            ProductId = product?.Id,
            CartonNo = cartonNo,
            CartonBarcode = cartonNo.ToString(),
            //TareWeight = tareWeight,

            CartonDetails = labelsList.Select(label =>
            {
                return new CartonDetail
                {
                    PackingTypeId = packingTypeId,

                    Code = label?.Code,
                    BatchNo = label?.BatchNo,
                    ProductId = product?.Id,
                    ItemId = label?.ItemId,
                    Barcode = label?.Barcode,

                    SystemWeight = label?.SystemWeight,
                    GrossWeight = label?.GrossWeight,
                    NetWeight = label?.NetWeight,
                    Quantity = 1,
                };
            }).ToList()
        };

        return carton;
    }

    #endregion

    #region -- Public Methods (Update) --
    public virtual async Task<bool> UpdateStatusAsync(string barcode)
    {
        var carton = await GetByBarcodeAsync(barcode).ConfigureAwait(false);
        if (null == carton) return false;

        carton.Status = StatusConstants.Scanned;
        carton.CartonDetails.ForEach(d => d.Status = StatusConstants.Scanned);
        await UpdateAsync(carton).ConfigureAwait(false);
        //Save();

        return true;
    }
    #endregion
    
    #region -- Public Moethods (Pallet In)
    public IEnumerable<Carton> GetPalletCartons(string palletNo, IEnumerable<string> oldStatus,
        string newStatus)
    {
        //var palletNo = GetPalletNoFromBarcode(barcode);
        var cartons = Get<Carton>(c => oldStatus.Contains(c.Status) &&
                                       c.CartonRackingDetails.Any(d => d.PalletNo == palletNo &&
                                                                       oldStatus.Contains(d.Status)),
                null)
            ;

        // Check whether it is already has new status
        if (cartons.Any()) return cartons;

        cartons = Get<Carton>(c => c.Status == newStatus &&
                                   c.CartonRackingDetails.Any(d => d.PalletNo == palletNo &&
                                                                   d.Status == newStatus)
                , null)
            ;
        if (cartons.Any())
            throw new Exception($"This pallet already has status : {newStatus}");

        return cartons;
    }

    //public List<Carton> PerformPalletIn(AndroidRequest androidRequest, string palletNo,
    //    ICollection<string> oldStatus, bool searchInDetails)
    //{
    //    var cartons = new List<Carton>();
    //    foreach (var barcode in androidRequest.Barcodes)
    //    {
    //        var carton = GetByBarcode(barcode, oldStatus, searchInDetails);

    //        // Update barcode log
    //        carton.CartonRackingDetails.Add(new CartonRackingDetail
    //        {
    //            ScanDate = DateTime.Now,
    //            PalletNo = palletNo,
    //            Status = StatusConstants.PalletIn
    //        });

    //        carton.Status = StatusConstants.PalletIn;

    //        Update(carton);
    //        Save();

    //        cartons.Add(carton);
    //    }

    //    return cartons;
    //}
    #endregion

    #region -- Public Moethods (Disptach) --
    public void PerformDispatch(string referenceNo, string barcode, string oldStatus)
    {
        var carton = GetByBarcode(barcode);
        if (null == carton)
            throw new Exception("No carton found for this barcode.");
        if (carton.Status == StatusConstants.Dispatch)
            throw new Exception("This barcode is already dispatched.");
        if (carton.Status != oldStatus)
            throw new Exception("Expected status for this barcode is " + oldStatus +
                                ". Current status is " + carton.Status);

        carton.ReferenceNo = referenceNo;
        carton.Status = StatusConstants.Dispatch;

        Update(carton);
        Save();
    }

    #endregion*/
    
}