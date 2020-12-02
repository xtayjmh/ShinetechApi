using System;

namespace API.Models
{
    public class BaseRequestModel
    {
        /// <summary>
        /// Id标识
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 是否删除，默认否
        /// </summary>
        public bool IsDelete { get; set; } = false;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 创建者
        /// </summary>
        public int? CreatedBy { get; set; }
        /// <summary>
        /// 最后一次更新者
        /// </summary>
        public int? UpdatedBy { get; set; }
    }
}