using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.Data
{
    /// <summary>
    /// 操作日志
    /// </summary>
    public class ActionLog : BaseEntity
    {
        /// <summary>
        /// 操作人
        /// </summary>
        public string Who { get; set; }
        /// <summary>
        /// 操作内容
        /// </summary>
        [Column(TypeName = "longtext")]
        public string Content { get; set; }
    }
}
