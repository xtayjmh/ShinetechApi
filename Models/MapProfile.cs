using System;
using System.Linq;
// using AutoMapper;
using Shinetech.Infrastructure.Contract;

namespace API.Models
{
    public class MapProfile// : Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public MapProfile()
        {
            var entityAssembly = typeof(BaseEntity).Assembly;

            var viewModelAssembly = typeof(BaseViewModel).Assembly;
            var modelNamespace = viewModelAssembly.GetTypes().FirstOrDefault(a => a.BaseType == typeof(BaseViewModel))?.Namespace;

            var requestModelAssembly = typeof(BaseRequestModel).Assembly;
            var requestNamespace = requestModelAssembly.GetTypes().FirstOrDefault(a => a.BaseType == typeof(BaseRequestModel))?.Namespace;



            foreach (var entity in entityAssembly.GetTypes().Where(a => a.BaseType == typeof(BaseEntity)))
            {
                var viewModel = viewModelAssembly.GetType($"{modelNamespace}.{entity.Name}ViewModel");
                // if (viewModel != null)
                // {
                //     CreateMap(entity, viewModel);
                //     CreateMap(viewModel, entity);
                //
                //     var pageEntity = Activator.CreateInstance((typeof(PaginatedList<>).MakeGenericType(entity)));
                //     var pageViewModel = Activator.CreateInstance((typeof(PaginatedList<>).MakeGenericType(viewModel)));
                //     CreateMap(pageEntity.GetType(), pageViewModel.GetType()).ReverseMap();
                // }
                //
                // var requestModel = requestModelAssembly.GetType($"{requestNamespace}.{entity.Name}Request");
                // if (requestModel != null)
                // {
                //     CreateMap(requestModel, entity);
                // }
            }
        }
    }
}