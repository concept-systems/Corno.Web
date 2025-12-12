using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Globals;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Base;
using Corno.Web.Services.Packing.Interfaces;
using Corno.Web.Services.Plan.Interfaces;
using Corno.Web.Services.Progress.Interfaces;
using Corno.Web.Windsor;

namespace Corno.Web.Services.Plan;

public class BasePlanService : PrintService<Models.Plan.Plan>, IBasePlanService
{
    #region -- Constructors --

    public BasePlanService(IGenericRepository<Models.Plan.Plan> genericRepository) : base(genericRepository)
    {
        SetIncludes($"{nameof(Models.Plan.Plan.PlanItemDetails)}," +
                    $"{nameof(Models.Plan.Plan.PlanPacketDetails)}");
    }
    #endregion

    #region -- Public Get Methods --

    public virtual async Task<Models.Plan.Plan> GetByWarehouseOrderNoAsync(string warehouseOrderNo)
    {
        if (string.IsNullOrEmpty(warehouseOrderNo))
            throw new Exception($"Invalid Warehouse Order No ({warehouseOrderNo}).");

        var plan = await FirstOrDefaultAsync(p => p.WarehouseOrderNo == warehouseOrderNo, p => p).ConfigureAwait(false);
        if (null == plan)
            throw new Exception($"No plan available for entered Warehouse Order No ({warehouseOrderNo}).");

        return !plan.PlanItemDetails.Any() ?
            throw new Exception($"Warehouse Order No ({warehouseOrderNo}) doesn't have items.") :
            plan;
    }

    #endregion

    #region -- Public Update Methods --

    public virtual async Task UpdatePackQuantitiesAsync(Models.Plan.Plan plan)
    {
        if (null == plan)
            return;

        var cartonService = Bootstrapper.Get<IBaseCartonService>();
        var planItemDetails = plan.PlanItemDetails
            .Where(d => (d.OrderQuantity ?? 0) > (d.PackQuantity ?? 0)).ToList();
        if (planItemDetails.Count <= 0 && plan.Status == StatusConstants.Packed) return;

        if (planItemDetails.Any())
        {
            var cartons = await cartonService.GetAsync(c => c.ProductionOrderNo == plan.ProductionOrderNo, c =>
                new { c.Id, c.CartonDetails }).ConfigureAwait(false);

            planItemDetails.ForEach(d => d.PackQuantity = cartons.SelectMany(c =>
                    c.CartonDetails.Where(x => x.PackingTypeId == d.PackingTypeId && x.ItemId == d.ItemId),
                (_, detail) => detail.Quantity).Sum());
        }
        var allPacked = plan.PlanItemDetails.All(d => (d.PackQuantity ?? 0) >= (d.OrderQuantity ?? 0));
        plan.PackQuantity = plan.OrderQuantity;
        plan.Status = allPacked ? StatusConstants.Packed : StatusConstants.InProcess;

        await UpdateAndSaveAsync(plan).ConfigureAwait(false);
    }

    #endregion

    #region -- Delete --

    public new event EventHandler OnDeleteComplete;
    public new event EventHandler<Exception> OnDeleteError;

    protected virtual void DeleteOtherPlanRelatedEntries(Models.Plan.Plan plan, IBaseProgressService progressService = null)
    {
        // Do Nothing
    }

    public async Task DeleteAsync(object id, IBaseProgressService progressService = null)
    {
        try
        {
            var plan = await GetByIdAsync(id).ConfigureAwait(false);
            if (null == plan)
                throw new Exception($"Plan not found with id {id}");

            DeleteOtherPlanRelatedEntries(plan, progressService);

            await DeleteAsync(plan).ConfigureAwait(false);
            await SaveAsync().ConfigureAwait(false);

            progressService?.Report("Deleted plan & plan related labels, pallets, cartons successfully.");

            OnDeleteComplete?.Invoke(null, null);
        }
        catch (Exception exception)
        {
            OnDeleteError?.Invoke(null, exception);
            throw;
        }
    }
    #endregion
}