using Shinetech.Infrastructure.Contract;
using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    /// <summary>
    /// 所有数据库对象的父类
    /// </summary>
    public class BaseEntity : IEntity
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public override int Id { get; set; }

        /// <summary>
        /// 删除标记
        /// </summary>
        public bool IsDelete { get; set; } = false;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人
        /// </summary>
        public int CreatedBy { get; set; }
        public int Status { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新人
        /// </summary>
        public int UpdatedBy { get; set; }

        public virtual string GetEntityName()
        {
            return this.GetType().Name;
        }

    }
}