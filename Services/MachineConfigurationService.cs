using Corno.Web.Models;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Services;

public class MachineConfigurationService : BaseService<MachineConfiguration>, IMachineConfigurationService
{
    public MachineConfigurationService(IGenericRepository<MachineConfiguration> machineConfigurationRepository)
    : base(machineConfigurationRepository)
       
    {
    }

}