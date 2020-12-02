using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Shinetech.Infrastructure.Contract;

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
        private object GetSqlFormatPropertyValue(PropertyInfo pType)
        {
            object result = "";
            var pValue = pType.GetValue(this);
            Type realType = pType.PropertyType;
            if (pType.PropertyType.Name == "Nullable`1")
            {
                if (pValue == null)
                {
                    return "null";
                }
                realType = pType.PropertyType.GetGenericArguments().Single();
            }

            if (realType == typeof(int) || realType == typeof(decimal) || realType == typeof(double) || realType == typeof(float))
            {
                result = pValue;
            }
            else if (realType == typeof(bool))
            {
                result = pValue;
            }
            else if (realType.BaseType == typeof(System.Enum))
            {
                result = (int)pValue;
            }
            else
            {
                result = "'" + pValue + "'";
            }
            return result;
        }
    }
}