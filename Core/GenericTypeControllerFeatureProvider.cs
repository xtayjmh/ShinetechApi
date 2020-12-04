using API.Controllers;
using API.Models;
using API.Models.RequestModel;
using API.Models.ViewModel;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Shinetech.Common;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace API.Core
{
    public class GenericTypeControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {

            var entityAssembly = typeof(BaseEntity).Assembly;
            var modelNamespace = entityAssembly.GetTypes().FirstOrDefault(a => a.BaseType == typeof(BaseEntity))?.Namespace;

            var viewModelAssembly = typeof(BaseViewModel).Assembly;


            var requestModelAssembly = typeof(BaseRequestModel).Assembly;
            var requestNamespace = requestModelAssembly.GetTypes()
                .FirstOrDefault(a => a.BaseType == typeof(BaseRequestModel))?.Namespace;


            var viewModelControllers = viewModelAssembly.GetExportedTypes()
                .Where(x => x.GetCustomAttributes<GeneratedControllerAttribute>().Any());

            foreach (var viewModel in viewModelControllers)
            {
                var name = viewModel.Name.Replace("ViewModel", "");

                var dataModel = entityAssembly.GetType($"{modelNamespace}.{name}");
                var requestModel = requestModelAssembly.GetType($"{requestNamespace}.{name}{"Request"}");
                if (dataModel != null && requestModel != null)
                {
                    feature.Controllers.Add(
                        typeof(CrudController<,,>).MakeGenericType(dataModel, viewModel, requestModel).GetTypeInfo()
                    );
                }
            }
        }
    }
}
