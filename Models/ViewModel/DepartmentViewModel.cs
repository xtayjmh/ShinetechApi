using Shinetech.Common;

namespace API.Models.ViewModel
{
    [GeneratedController(description: "部门管理")]
    public class DepartmentViewModel : BaseViewModel
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
    }
}
