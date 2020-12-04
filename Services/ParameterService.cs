using API.Interfaces;
using API.Models.Data;
using API.Models.RequestModel;
using API.Models.ViewModel;
using Shinetech.Infrastructure.Contract;
using System;
using System.Linq;
namespace API.Services
{
    public class ParameterService : CrudService<Parameter, ParameterViewModel, ParameterRequest>, IParameterService
    {
        protected readonly ICrudRepository<ActionLog> logRepository;
        public ParameterService(IServiceProvider serviceProvider, ICrudRepository<ActionLog> _logRepository) : base(serviceProvider)
        {
            logRepository = _logRepository;
        }

        public ParameterViewModel GetParameter(string parameterName)
        {
            var existsModel = _repository.Get(r => r.Name == parameterName).FirstOrDefault();
            if (existsModel != null)
            {
                return base.GetOne(existsModel.Id);
            }
            return null;

        }

        protected override void OnUpdateMapping(Parameter existsData, Parameter toUpdateData)
        {
            toUpdateData.CreatedBy = existsData.CreatedBy;
        }
        public bool UpdateParameter(ParameterRequest update)
        {
            logRepository.DbSet.Add(new ActionLog()
            {
                Who = _currentUser.Name,
                Content = "更新参数：" + update.Name + "-" + update.Value
            });
            logRepository.Save();


            var existsModel = _repository.Get(r => r.Name == update.Name).FirstOrDefault();
            if (existsModel != null)
            {
                update.Id = existsModel.Id;
                return base.Update(update);
            }
            else
            {
                return Add(update) > 0;
            }

        }

      
    }
}
