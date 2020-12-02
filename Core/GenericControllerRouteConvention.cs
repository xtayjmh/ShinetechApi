using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Shinetech.Common;

namespace API.Core
{
    public class GenericControllerRouteConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.IsGenericType)
            {
                var genericType = controller.ControllerType.GenericTypeArguments[1];
                var customNameAttribute = genericType.GetCustomAttribute<GeneratedControllerAttribute>();

                var entityType = controller.ControllerType.GenericTypeArguments[0];
                controller.ControllerName = entityType.Name;
                if (!string.IsNullOrEmpty(customNameAttribute?.Description))
                {
                    controller.ControllerName = customNameAttribute.Description;
                }
                else
                {
                    controller.ControllerName = entityType.Name;
                }
                if (!string.IsNullOrEmpty(customNameAttribute?.Route))
                {
                    controller.Selectors.Add(new SelectorModel
                    {
                        AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(customNameAttribute.Route)),
                    });
                }
                else
                {
                    controller.Selectors.Add(new SelectorModel
                    {
                        AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(entityType.Name)),
                    });
                }

            }
        }
    }
}
