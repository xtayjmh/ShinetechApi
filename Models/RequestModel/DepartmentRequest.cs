using System.ComponentModel.DataAnnotations;

namespace API.Models.RequestModel
{
    public class DepartmentRequest : BaseRequestModel
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
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
    }

    public class SetEnableEmployeeRequest
    {
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public bool IsEnable { get; set; }
    }
}
