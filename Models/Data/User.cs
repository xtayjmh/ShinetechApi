using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.Data
{

    /// <summary>
    /// 用户表
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// 登录账号
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// 是否是管理员
        /// </summary>
        public bool IsAdmin { get; set; }
        /// <summary>
        /// 启用禁用
        /// </summary>
        public bool IsEnable { get; set; }

        [InverseProperty("CreatedByPerson")]
        public virtual ICollection<Parameter> CreatedParameters { get; set; }

        [InverseProperty("UpdateByPerson")]
        public virtual ICollection<Parameter> UpdatedParameters { get; set; }
        public override string GetEntityName()
        {
            return " 用户" + "LoginName:" + LoginName;
        }
    }
}
