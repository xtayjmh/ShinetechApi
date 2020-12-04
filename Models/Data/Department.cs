using System.Text.Json;

namespace API.Models.Data
{
    /// <summary>
    /// 部门
    /// </summary>
    public class Department : BaseEntity
    {
        /// <summary>
        /// 上级部门
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// 部门编码
        /// </summary>
        public string DeptCode { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string ResponsibleMan { get; set; }
        /// <summary>
        /// 负责人电话
        /// </summary>
        public string ResponsiblePhone { get; set; }
        public string Remark { get; set; }
        public override string GetEntityName()
        {
            return "部门" + JsonSerializer.Serialize(this);
        }
    }
}
