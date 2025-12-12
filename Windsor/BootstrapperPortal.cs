using System.Configuration;
using Autofac;
using Corno.Concept.Portal.Repository;
using Corno.Concept.Portal.Repository.Interfaces;
using Corno.Concept.Portal.Windsor.Context;

namespace Corno.Concept.Portal.Windsor
{
    public static class BootstrapperPortal
    {
        private static Bootstrapper _bootstrapper;

        public static void Initialize()
        {
            _bootstrapper = Bootstrapper.Bootstrap();

            

            // Register GenericRepository
            var builder = new ContainerBuilder();
            
            //builder.Update(Bootstrapper.StaticContainer);
        }
    }
}