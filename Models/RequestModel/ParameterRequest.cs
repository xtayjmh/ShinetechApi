using System.ComponentModel.DataAnnotations;

namespace API.Models.RequestModel
{
    public class ParameterRequest : BaseRequestModel
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        [Required]
        public string Value { get; set; }
    }
}
