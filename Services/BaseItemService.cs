using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models;
using Corno.Web.Models.Masters;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.File.Interfaces;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Services.Progress.Interfaces;
using Corno.Web.Windsor;
using Mapster;

namespace Corno.Web.Services;

public class BaseItemService : MasterService<Item>, IBaseItemService
{
    #region -- Constructors --
    public BaseItemService(IGenericRepository<Item> genericRepository, IBaseProcessService processService, 
        IMiscMasterService miscMasterService) : base(genericRepository)
    {
        _processService = processService;
        _miscMasterService = miscMasterService;

        // Do not enable includes globally; use explicit methods when details are needed.
        SetIncludes($"{nameof(Item.ItemProcessDetails)}," +
                    $"{nameof(Item.ItemMachineDetails)}," +
                    $"{nameof(Item.ItemPacketDetails)}");
    }
    #endregion

    #region -- Data Members --
    
    private readonly IBaseProcessService _processService;
    private readonly IMiscMasterService _miscMasterService;

    #endregion

    #region -- Private Fields --

    /*private void UpdatePhoto(ICornoModel entity)
    {
        var photoPathField = entity.GetType().GetProperty(FieldConstants.PhotoPath);
        if (photoPathField == null) return;

        var imagePath = photoPathField.GetValue(entity)?.ToString();
        if (string.IsNullOrEmpty(imagePath) || !System.IO.File.Exists(imagePath)) return;
        var photoField = entity.GetType().GetProperty(FieldConstants.Photo);
        if (null == photoField) return;
        var data = ImageHelper.GetBytes(imagePath);
        photoField.SetValue(entity, data);
    }*/

    protected virtual async Task UpdateItemTypeAsync(Item entity, Item newEntity)
    {
        if (string.IsNullOrEmpty(entity.ItemType)) return;

        var itemTypes = await _miscMasterService.GetAsync(m => m.MiscType == MiscMasterConstants.ItemType &&
            (m.Code == entity.ItemType || m.Name == entity.ItemType), m => m.Id).ConfigureAwait(false);
        var itemTypeId = itemTypes.FirstOrDefault();
        if (itemTypeId <= 0)
            newEntity.ItemTypeId = itemTypeId;
        else
        {
            var miscMaster = await _miscMasterService.GetOrCreateAsync(entity.ItemType,
                entity.ItemType, MiscMasterConstants.ItemType).ConfigureAwait(false);
            newEntity.ItemTypeId = miscMaster?.Id;
        }
    }

    protected virtual async Task UpdateItemCategoryAsync(Item entity, Item newEntity)
    {
        if (string.IsNullOrEmpty(entity.ItemCategory)) return;

        var itemCategories = await _miscMasterService.GetAsync(m => m.MiscType == MiscMasterConstants.ItemCategory &&
            (m.Code == entity.ItemCategory || m.Name == entity.ItemCategory), m => m.Id).ConfigureAwait(false);
        var itemCategoryId = itemCategories.FirstOrDefault();
        if (itemCategoryId <= 0)
            newEntity.ItemCategoryId = itemCategoryId;
        else {
            var miscMaster = await _miscMasterService.GetOrCreateAsync(entity.ItemCategory,
                entity.ItemCategory, MiscMasterConstants.ItemCategory).ConfigureAwait(false);
            newEntity.ItemCategoryId = miscMaster?.Id;
        }
    }

    protected virtual async Task UpdateUnitAsync(Item entity, Item newEntity)
    {
        if (string.IsNullOrEmpty(entity.Unit)) return;

        var units = await _miscMasterService.GetAsync(m => m.MiscType == MiscMasterConstants.Unit &&
            (m.Code == entity.Unit || m.Name == entity.Unit), m => m.Id).ConfigureAwait(false);
        var unitId = units.FirstOrDefault();
        if (unitId <= 0)
            newEntity.UnitId = unitId;
        else {
            var miscMaster = await _miscMasterService.GetOrCreateAsync(entity.Unit,
                entity.Unit, MiscMasterConstants.Unit).ConfigureAwait(false);
            newEntity.UnitId = miscMaster?.Id;
        }
    }

    protected async Task UpdateFieldsAsync(Item entity, Item newEntity)
    {
        base.UpdateFields(entity, newEntity);

        //UpdatePhoto(entity);

        /*newEntity.Rate = entity.Rate;
        newEntity.Weight = entity.Weight;
        newEntity.WeightTolerance = entity.WeightTolerance;
        newEntity.BoxQuantity = entity.BoxQuantity;*/

        entity.Adapt(newEntity);

        await UpdateItemTypeAsync(entity, newEntity).ConfigureAwait(false);
        await UpdateItemCategoryAsync(entity, newEntity).ConfigureAwait(false);
        await UpdateUnitAsync(entity, newEntity).ConfigureAwait(false);

        /*if (!string.IsNullOrEmpty(entity.ItemType))
        {
            var itemTypeId = _miscMasterService.Get(m => m.MiscType == MiscMasterConstants.ItemType &&
                m.Code == entity.ItemType || m.Name == entity.ItemType, m => m.Id).FirstOrDefault();
            if (itemTypeId <= 0)
                newEntity.ItemTypeId = itemTypeId;
            else
            {
                newEntity.ItemTypeId = _miscMasterService.GetOrCreate(entity.ItemType,
                    entity.ItemType, MiscMasterConstants.ItemType)?.Id;
            }
        }*/

        /*if (!string.IsNullOrEmpty(entity.ItemCategory))
        {
            var itemCategoryId = _miscMasterService.Get(m => m.MiscType == MiscMasterConstants.ItemCategory &&
                m.Code == entity.ItemCategory || m.Name == entity.ItemCategory, m => m.Id).FirstOrDefault();
            if (itemCategoryId <= 0)
                newEntity.ItemCategoryId = itemCategoryId;
            else
            {
                newEntity.ItemTypeId = _miscMasterService.GetOrCreate(entity.ItemCategory,
                    entity.ItemCategory, MiscMasterConstants.ItemCategory)?.Id;
            }
        }*/
        /*if (!string.IsNullOrEmpty(entity.Unit))
        {
            var unitId = _miscMasterService.Get(m => m.MiscType == MiscMasterConstants.Unit &&
                m.Code == entity.Unit || m.Name == entity.Unit, m => m.Id).FirstOrDefault();
            if (unitId <= 0)
                newEntity.ItemCategoryId = unitId;
            else
            {
                newEntity.UnitId = _miscMasterService.GetOrCreate(entity.Unit,
                    entity.Unit, MiscMasterConstants.Unit)?.Id;
            }
        }*/
    }
    #endregion

    #region -- Public Methods --

    public async Task<Item> GetItemAsync(int itemId)
    {
        // Fast path: no includes
        var items = await GetAsync<Item>(i => i.Id == itemId, i => i, null, ignoreInclude: true)
            .ConfigureAwait(false);
        var item = items.FirstOrDefault();
        if (null == item)
            throw new Exception($"Invalid Item ({itemId})!");
        return item;
    }

    /// <summary>
    /// Get item with process/machine/packet details loaded
    /// </summary>
    public async Task<Item> GetItemWithDetailsAsync(int itemId)
    {
        /*SetIncludes($"{nameof(Item.ItemProcessDetails)}," +
                    $"{nameof(Item.ItemMachineDetails)}," +
                    $"{nameof(Item.ItemPacketDetails)}");*/
        var items = await GetAsync<Item>(i => i.Id == itemId, i => i, null, ignoreInclude: false)
            .ConfigureAwait(false);
        var item = items.FirstOrDefault();
        if (null == item)
            throw new Exception($"Invalid Item ({itemId})!");
        return item;
    }

    public async Task ReceiveStockQuantityAsync(int itemId, int quantity)
    {
        var item = await GetItemAsync(itemId).ConfigureAwait(false);
        item.StockQuantity ??= 0;
        item.StockQuantity += quantity;

        await UpdateAndSaveAsync(item).ConfigureAwait(false);
    }

    public async Task IssueStockQuantityAsync(int itemId, double quantity)
    {
        var item = await GetItemAsync(itemId).ConfigureAwait(false);
        item.StockQuantity ??= 0;
        item.StockQuantity -= quantity;

        await UpdateAndSaveAsync(item).ConfigureAwait(false);
    }

    public async Task<double?> GetStockQuantityAsync(int itemId)
    {
        var item = await GetItemAsync(itemId).ConfigureAwait(false);
        return item.StockQuantity;
    }

    public async Task<string> GetFlavorNameAsync(int itemId)
    {
        var item = await GetItemAsync(itemId).ConfigureAwait(false);
        var miscMasterService = Bootstrapper.Get<IMiscMasterService>();
        return await miscMasterService.GetNameAsync(item.FlavorId ?? 0).ConfigureAwait(false);
    }

    /*public IEnumerable<MasterDto> GetPackets(int itemId) {
        var packetIds = _itemPacketDetailService.Get(i =>
                i.ItemId == itemId, i => i.PackingTypeId)
            .Distinct().ToList();
        if (!packetIds.Any()) 
            return null;
        return _miscMasterService.GetViewModelList(m =>
            packetIds.Contains(m.Id)) as IList<MasterDto>;
    }*/

    public async Task<bool> IsQcApplicableAsync(int itemId)
    {
        var qcCheck =  await FirstOrDefaultAsync(i => i.Id == itemId, i => i.QcCheck).ConfigureAwait(false);
        return qcCheck.ToBoolean();
    }

    public override async Task ImportAsync(string filePath, IBaseProgressService progressService,
        string miscMaster = null)
    {
        var excelFileService = Bootstrapper.Get<IExcelFileService<Item>>();
        var entities = excelFileService.Read(filePath, 0).ToList();
        progressService.Initialize(filePath, 0, entities.Count(), 1);
        try
        {
            foreach (var entity in entities)
            {
                try
                {
                    var exists = await GetByCodeAsync(entity.Code).ConfigureAwait(false);
                    if (null != exists)
                    {
                        await UpdateFieldsAsync(entity, exists).ConfigureAwait(false);
                        await UpdateAndSaveAsync(exists).ConfigureAwait(false);

                        progressService.Report(0, 1, 0);
                        continue;
                    }

                    entity.SerialNo = await GetNextSerialNoAsync().ConfigureAwait(false);
                    await UpdateFieldsAsync(entity, entity).ConfigureAwait(false);

                    await AddAndSaveAsync(entity).ConfigureAwait(false);

                    progressService.Report(1, 0, 0);
                }
                catch (Exception exception)
                {
                    LogHandler.LogError(exception);
                }
            }
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
        }
    }

    /*public override void Export(string filePath, IBaseProgressService progressService)
    {
        var excelFileService = Bootstrapper.Get<IExcelFileService<ItemExportModel>>();

        var records = from item in Get<Item>()
                join itemType in _miscMasterService.GetViewModelList()
                    on item.ItemTypeId equals itemType.Id into defaultItemType
                from itemType in defaultItemType.DefaultIfEmpty()
                join itemCategory in _miscMasterService.GetViewModelList()
                    on item.ItemCategoryId equals itemCategory.Id into defaultItemCategory
                from itemCategory in defaultItemCategory.DefaultIfEmpty()
                select ItemExportModel.GetModel(item, itemType, itemCategory)
            ;

        excelFileService.Save(filePath, 0, records);
    }*/

    public async Task UpdateWeightAsync(Item item, double weight, double tolerance)
    {
        if (null == item) return;

        if ((item.Weight ?? 0) > 0 && (item.WeightTolerance ?? 0) > 0)
            return;

        item.Weight = weight;
        item.WeightTolerance = tolerance;

        await UpdateAndSaveAsync(item).ConfigureAwait(false);
    }
    #endregion

    #region -- Process Methods --
    public virtual async Task<Process> GetNextProcessAsync(int itemId, string processCode)
    {
        var item = await GetItemWithDetailsAsync(itemId).ConfigureAwait(false);
        if (null == item)
            throw new Exception($"Item ({itemId}) not found.");

        var currentProcess = await _processService.GetByCodeAsync(processCode).ConfigureAwait(false);
        if (null == currentProcess)
            throw new Exception($"The process {processCode} not found");

        var currentProcessDetail = item.ItemProcessDetails.FirstOrDefault(i =>
            i.ProcessId == currentProcess.Id);
        if (null == currentProcessDetail)
            throw new Exception($"The process {currentProcess.Code} is not assigned " +
                                $"for item {item.Code}");

        var nextProcessDetail = item.ItemProcessDetails
            .FirstOrDefault(i => i.ProcessSequence == currentProcessDetail.ProcessSequence + 1);
        // First process
        if (null == nextProcessDetail)
            throw new Exception($"No process available after '{currentProcess.Code}'.");
        return await _processService.GetByIdAsync(nextProcessDetail.ProcessId).ConfigureAwait(false);
    }

    public async Task<Process> GetNextProcessWithNullReturnAsync(int itemId, string processCode)
    {
        var item = await GetItemWithDetailsAsync(itemId).ConfigureAwait(false);
        if (null == item)
            return null;

        var currentProcess = await _processService.GetByCodeAsync(processCode).ConfigureAwait(false);
        if (null == currentProcess)
            return null;

        var currentProcessDetail = item.ItemProcessDetails.FirstOrDefault(i =>
            i.ProcessId == currentProcess.Id);
        if (null == currentProcessDetail)
            return null;

        var nextProcessDetail = item.ItemProcessDetails
            .FirstOrDefault(i => i.ProcessSequence == currentProcessDetail.ProcessSequence + 1);
        // First process
        if (null == nextProcessDetail)
            return null;
        return await _processService.GetByIdAsync(nextProcessDetail.ProcessId).ConfigureAwait(false);
    }

    public async Task<string> GetNextProcessStatusAsync(int itemId, string processCode)
    {
        var item = await GetItemWithDetailsAsync(itemId).ConfigureAwait(false);
        if (null == item)
            throw new Exception($"Item ({itemId}) not found.");

        var currentProcess = await _processService.GetByCodeAsync(processCode).ConfigureAwait(false);
        if (null == currentProcess)
            throw new Exception($"The process {processCode} not found");

        var currentProcessDetail = item.ItemProcessDetails.FirstOrDefault(i =>
            i.ProcessId == currentProcess.Id);
        if (null == currentProcessDetail)
            throw new Exception($"The process {currentProcess.Code} is not assigned " +
                                $"for item {item.Code}");

        var nextProcessDetail = item.ItemProcessDetails
            .FirstOrDefault(i => i.ProcessSequence == currentProcessDetail.ProcessSequence + 1);
        // First process
        if (null == nextProcessDetail)
            throw new Exception($"No process available after '{currentProcess.Code}'.");
        var nextProcess = await _processService.GetByIdAsync(nextProcessDetail.ProcessId).ConfigureAwait(false);

        return Process.GetProcessStatus(nextProcess.Code);
    }

    public async Task<List<string>> GetPreviousProcessStatusAsync(int itemId, string processCode)
    {
        var item = await GetItemWithDetailsAsync(itemId).ConfigureAwait(false);
        if (null == item)
            throw new Exception($"Item ({itemId}) not found.");

        var currentProcess = await _processService.GetByCodeAsync(processCode).ConfigureAwait(false);
        if (null == currentProcess)
            throw new Exception($"The process {processCode} not found");

        var currentProcessDetail = item.ItemProcessDetails.FirstOrDefault(i =>
            i.ProcessId == currentProcess.Id);
        if (null == currentProcessDetail)
            throw new Exception($"The process {currentProcess.Code} is not assigned " +
                                $"for item {item.Code}");
        // First process
        if (currentProcessDetail.ProcessSequence == 1)
            return new List<string> { StatusConstants.Active };

        var previousProcessDetail = item.ItemProcessDetails
            .FirstOrDefault(i => i.ProcessSequence == currentProcessDetail.ProcessSequence - 1);
        // First process
        if (null == previousProcessDetail)
            return new List<string> { StatusConstants.Active };
        var previousProcess = await _processService.GetByIdAsync(previousProcessDetail.ProcessId).ConfigureAwait(false);

        return new List<string> { Process.GetProcessStatus(previousProcess.Code) };
    }

    #endregion
}