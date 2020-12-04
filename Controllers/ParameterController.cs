using API.Auth;
using API.Interfaces;
using API.Models.RequestModel;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace API.Controllers
{
    /// <summary>
    /// 参数设置
    /// </summary>
    [Route("[controller]")]
    [APIAuthAttribute]
    public class ParameterController : Controller
    {
        readonly IParameterService _parameterService;
        public ParameterController(IConfiguration config, IParameterService parameterService, IMapper mapper, ICurrentUser userAccount)
        {
            _parameterService = parameterService;
        }
        /// <summary>
        /// 根据名称获得配置参数
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        [HttpGet("parameterName")]
        public ActionResult GetByName(string parameterName)
        {
            var result = _parameterService.GetParameter(parameterName);
            return Ok(result);
        }
        /// <summary>
        /// 更新配置参数
        /// </summary>
        /// <param name="updateModel"></param>
        /// <returns></returns>
        [HttpPut]
        public bool UpdateByName([FromBody] ParameterRequest updateModel)
        {
            var result = _parameterService.UpdateParameter(updateModel);
            return result;
        }

    }
}
