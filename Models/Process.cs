using System;
using System.Collections.Generic;
using Corno.Web.Globals;

namespace Corno.Web.Models;

public class Process : MasterModel
{
    #region -- Properties --
    public string ShortName { get; set; }
    public double? CycleTime { get; set; }
    public double? ProcessTime { get; set; }
    #endregion

    #region -- Public Methods --

    public string GetProcessStatus()
    {
        return GetProcessStatus(Code);
    }

    public static string GetProcessStatus(string code)
    {
        switch (code)
        {
            // Production
            case OperationConstants.Cutting:
                return StatusConstants.Cut ;
            case OperationConstants.EdgeBanding:
                return StatusConstants.EdgeBanded;
            case OperationConstants.Q1EdgeBanding:
                return StatusConstants.Q1 ;
            case OperationConstants.Drilling:
                return StatusConstants.Drilled;
            case OperationConstants.Boring1B:
                return StatusConstants.Bored1B;
            case OperationConstants.Boring2B:
                return StatusConstants.Bored2B;
            case OperationConstants.Q2SubAssembly:
                return StatusConstants.Q2;
            case OperationConstants.CurveFlooring:
                return StatusConstants.CurveFloored;
            case OperationConstants.Routing1R:
                return StatusConstants.Routing1R;
            case OperationConstants.Routing2R:
                return StatusConstants.Routing2R;
            case OperationConstants.ManualEdgeBanding:
                return StatusConstants.ManualEdgeBanded;
            case OperationConstants.Cleaning:
                return StatusConstants.Cleaned;
            case OperationConstants.SubAssembly:
                return StatusConstants.SubAssembled;
            case OperationConstants.Packing:
                return StatusConstants.Packed;
            case OperationConstants.Q3Packing:
                return StatusConstants.Q3;
            default:
                throw new Exception($"Invalid process code {code}.");
        }
    }

    /*public static string GetNameBySymbol(string symbol)
    {
        switch (symbol)
        {
            // Production
            case SymbolConstants.Cutting:
                return OperationConstants.Cutting;
            case SymbolConstants.EdgeBanding:
                return OperationConstants.EdgeBanding;
            case SymbolConstants.Q1EdgeBanding:
                return OperationConstants.Q1EdgeBanding;
            case SymbolConstants.Boring1:
                return OperationConstants.Boring1B;
            case SymbolConstants.Boring2:
                return OperationConstants.Boring2B;
            case SymbolConstants.ManualEdgeBanding:
                return OperationConstants.ManualEdgeBanding;
            case SymbolConstants.Q2SubAssembly:
                return OperationConstants.Q2SubAssembly;
            case SymbolConstants.Cleaning:
                return OperationConstants.Cleaning;
            case SymbolConstants.Routing1:
                return OperationConstants.Routing1R;
            case SymbolConstants.Routing2:
                return OperationConstants.Routing2R;
            case SymbolConstants.SubAssembly:
                return OperationConstants.SubAssembly;
            case SymbolConstants.Packing:
                return OperationConstants.Packing;
            case SymbolConstants.Q3Packing:
                return OperationConstants.Q3Packing;
            default:
                throw new Exception($"Invalid Symbol {symbol}.");
        }
    }*/

    public static List<string> GetAllStatus()
    {
        return new List<string>
        {
            StatusConstants.Cut,
            StatusConstants.EdgeBanded,
            StatusConstants.Q1,
            StatusConstants.Drilled,
            StatusConstants.Bored1B,
            StatusConstants.Bored2B,
            StatusConstants.Q2,
            StatusConstants.CurveFloored,
            StatusConstants.Routing1R,
            StatusConstants.Routing2R,
            StatusConstants.ManualEdgeBanded,
            StatusConstants.Cleaned,
            StatusConstants.SubAssembling,
            StatusConstants.SubAssembled,
        };
    }

    #endregion
}