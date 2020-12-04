
using API.Models.RequestModel;
using API.Models.ViewModel;

namespace API.Interfaces
{
    public interface IParameterService : IService
    {
        ParameterViewModel GetParameter(string parameterName);
        /// <summary>
        /// 更新操作，存在则更新，不存在则创建
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        bool UpdateParameter(ParameterRequest update);
    }
}
