using Corno.Web.Models.Base;

namespace Corno.Web.Models;

public class MachineConfiguration : BaseModel
{
    public int? MachineId { get; set; }

    public int? Parameter1Id { get; set; }
    public int? Parameter2Id { get; set; }
    public int? Parameter3Id { get; set; }

    public int? ShiftTarget { get; set; }
    public int? MonthlyTarget { get; set; }

    public bool AutoQualityEstimate { get; set; }
    public bool QualityModule { get; set; }
    public bool ManualMode { get; set; }
    public bool UserDetailsModule { get; set; }

    // Unplanned breakdown
    public bool MachineBreakdown { get; set; }
    public bool ComponentFailure { get; set; }
    public bool OtherUnplanned { get; set; }

    // Planned breakdown
    public bool Maintenance { get; set; }
    public bool Lunch_Tea_Dinner { get; set; }
    public bool Holiday { get; set; }
    public bool OtherPlanned { get; set; }

    // Ideal Downtime
    public bool NoOperator { get; set; }
    public bool NoMaterial { get; set; }

    // Setup Downtime
    public bool Calibration { get; set; }
    public bool OtherSetup { get; set; }

    public int? IdealCycleTime { get; set; }
     
}