using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Ganss.Excel;

namespace Corno.Web.Models.Masters;

public class Item : MasterModel
{
    #region -- Constructors --
    public Item()
    {
        ItemProcessDetails = new List<ItemProcessDetail>();
        ItemMachineDetails = new List<ItemMachineDetail>();
        ItemPacketDetails = new List<ItemPacketDetail>();
        //ItemAssemblyDetails = new List<ItemAssemblyDetail>();
    }
    #endregion

    #region -- Properties --
    [Ignore]
    public byte[] Photo { get; set; }
    [DisplayName("Item Type")]
    public int? ItemTypeId { get; set; }
    public int? ItemCategoryId { get; set; }
    [DisplayName("Unit")]
    public int? UnitId { get; set; }
    public double? Rate { get; set; }
    public double? StockQuantity { get; set; }
    public int? PackingTypeId { get; set; }
    public double? BoxQuantity { get; set; }
    public double? ReorderLevel { get; set; }
    public double? Length { get; set; }
    public double? LengthTolerance { get; set; }
    public double? Width { get; set; }
    public double? WidthTolerance { get; set; }
    public double? Thickness { get; set; }
    public double? ThicknessTolerance { get; set; }
    public double? Weight { get; set; }
    public double? WeightTolerance { get; set; }
    public double? Red { get; set; }
    public double? RedTolerance { get; set; }
    public double? Green { get; set; }
    public double? GreenTolerance { get; set; }
    public double? Blue { get; set; }
    public double? BlueTolerance { get; set; }
    public string Color { get; set; }
    public string DrawingNo { get; set; }
    public string RackNo { get; set; }
    public bool? Weighing { get; set; }
    public bool? QcCheck { get; set; }
    public bool? PartialQc { get; set; }
    public int? FlavorId { get; set; }
    public string Reserved1 { get; set; }
    public string Reserved2 { get; set; }
    public string Reserved3 { get; set; }
    public string Reserved4 { get; set; }

    [NotMapped]
    public string ItemType { get; set; }
    [NotMapped]
    public string ItemCategory { get; set; }
    [NotMapped]
    public string Unit { get; set; }
    [NotMapped]
    public string Location { get; set; }

    public List<ItemProcessDetail> ItemProcessDetails { get; set; }
    public List<ItemMachineDetail> ItemMachineDetails { get; set; }
    public List<ItemPacketDetail> ItemPacketDetails { get; set; }
    #endregion

    #region -- Public Methods --
    public int? GetHighPriorityMachineId(int? processId)
    {
                
        return ItemMachineDetails.OrderBy(d => d.Priority)
            .FirstOrDefault(d => d.ProcessId == processId)
            ?.MachineId;
    }

    public ItemMachineDetail GetItemMachineDetail(int? processId, int? machineId)
    {
        return ItemMachineDetails.FirstOrDefault(i => i.ProcessId == processId &&
                                                      i.MachineId == machineId);
    }

    public ItemMachineDetail GetItemMachineDetail(int? machineId)
    {
        return ItemMachineDetails.FirstOrDefault(i => i.MachineId == machineId);
    }

    public string GetProcessSequence()
    {
        return string.Join(",", ItemProcessDetails.Select(d => d.ProcessSymbol));
    }

    public int GetProcessSequence(int processId)
    {
        return ItemProcessDetails.FirstOrDefault(i => i.ProcessId == processId)?.ProcessSequence ?? 0;
    }

    public ItemProcessDetail GetProcessDetail(string operation)
    {
        return ItemProcessDetails.FirstOrDefault(i => i.Code == operation);
    }

    public ItemProcessDetail GetProcessDetailBySymbol(string symbol)
    {
        return ItemProcessDetails.FirstOrDefault(i => i.ProcessSymbol == symbol);
    }

    public bool IsProcessAvailable(string symbol)
    {
        return ItemProcessDetails.Any(i => i.ProcessSymbol == symbol);
    }

    public ItemProcessDetail GetNextProcessDetail(string processCode)
    {
        switch (processCode)
        {
            case OperationConstants.None:
                return ItemProcessDetails.OrderBy(d => d.ProcessSequence)
                    .FirstOrDefault();
            default:
                return ItemProcessDetails.OrderBy(d => d.ProcessSequence)
                    .SkipWhile(x => x.Code != processCode)
                    .Skip(1)
                    .FirstOrDefault();
        }
    }

    public string GetNextProcessSymbol(string processCode)
    {
        return GetNextProcessDetail(processCode)?.ProcessSymbol;
    }

    public string GetNextOperation(string processCode)
    {
        return GetNextProcessDetail(processCode)?.Code;
    }

    public ItemProcessDetail GetNextProcessDetailByStatus(string status)
    {
        return ItemProcessDetails.OrderBy(d => d.ProcessSequence)
            .SkipWhile(x => x.Status != status)
            .Skip(1)
            .FirstOrDefault();
    }

    public ItemProcessDetail GetPrevProcessDetail(string processCode)
    {
        // Check if process is assigned to item.
        if (ItemProcessDetails.All(d => d.Code != processCode))
            throw new Exception($"Process '{processCode.ToCamelCase()}' is not assigned " +
                                $"to this item '{Code}'");

        var processDetail = ItemProcessDetails.OrderBy(d => d.ProcessSequence)
            .Reverse()
            .SkipWhile(x => x.Code != processCode)
            .Skip(1)
            .FirstOrDefault();
        if (processDetail?.ProcessSequence < 1)
            return null;
        return processDetail;
        //return ItemProcessDetails.OrderBy(d => d.ProcessSequence)
        //    .SkipWhile(x => x.Code != processCode)
        //    .Skip(-1)
        //    .FirstOrDefault();
    }

    //public string GetNextProcessName(string status)
    //{
    //    return ItemProcessDetails.OrderBy(d => d.ProcessSequence)
    //        .SkipWhile(x => x.Status != status)
    //        .Skip(1)
    //        .FirstOrDefault()?.ProcessSymbol;
    //}

    public void ArrangeProcessSequences()
    {
        var sequence = 1;
        foreach (var processDetail in ItemProcessDetails)
            processDetail.ProcessSequence = sequence++;
    }

    public double GetCycleTime(int processId)
    {
        //var cycleTime = ItemProcessDetails.Where(p => p.ProcessId == processId)
        //    .Sum(p => (p.CycleTime1 ?? 0) + (p.CycleTime2 ?? 0));
        var processTime = ItemProcessDetails.Where(p => p.ProcessId == processId)
            .Sum(p => p.ProcessTime ?? 0);
        return processTime / 60; // for minute calculation.;
    }
    #endregion
}