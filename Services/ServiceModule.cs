using API.Interfaces;
using Autofac;

namespace API.Services
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(this.ThisAssembly)
                .Where(t => t.IsAssignableTo<API.Interfaces.IService>())
                .AsImplementedInterfaces()
                .InstancePerDependency();
            builder.RegisterGeneric(typeof(CrudService<,,>)).As(typeof(ICrudService<,,>)).InstancePerDependency();
        }
    }
}
