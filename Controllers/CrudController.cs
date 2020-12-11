using API.Auth;
using API.Interfaces;
using API.Models;
using API.Models.RequestModel;
using API.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shinetech.Common;
using Shinetech.Infrastructure.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    /// <summary>
    ///  
    /// </summary>
    /// <typeparam name="TableModel"></typeparam>
    [APIAuthAttribute]
    [Produces("application/json")]
    public class CrudController<TableModel, PageView, PageInput> : Controller where TableModel : BaseEntity where PageView : BaseViewModel where PageInput : BaseRequestModel
    {
        protected readonly IConfiguration _configuration;
        protected readonly ICrudService<TableModel, PageView, PageInput> _service;
        // protected readonly IMapper _mapper;
        readonly ICurrentUser _userAccount;
        protected IExcelHandlerService _excelHandlerService;

        public CrudController(IServiceProvider serviceProvider)
        {
            _configuration = serviceProvider.GetService<IConfiguration>();
            _service = serviceProvider.GetService<ICrudService<TableModel, PageView, PageInput>>();
            // _mapper = serviceProvider.GetService<IMapper>();
            _userAccount = serviceProvider.GetService<ICurrentUser>();
            _excelHandlerService = serviceProvider.GetService<IExcelHandlerService>();
        }

        /// <summary>
        /// set the result sort,this method should be called before GetAll function 
        /// </summary>
        /// <param name="func"></param>
        protected virtual void OnSetOrderBy(Func<TableModel, Object> func)
        {
            _service.SetOrderBy(func);
        }

        protected virtual void SetOrderBySequence(bool ascent)
        {
            _service.SetOrderBySequence(ascent);
        }

        protected virtual void OnSetIncludeProperties(string includeProperties)
        {
            _service.SetIncludeProperties(includeProperties);
        }


        /// <summary>
        /// 搜索功能
        /// </summary>
        /// <param name="paginatedSearchRequest">
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// Operation取值：
        /// 0:等于,1:不等于,2:大于,3:小于,4:大于等于,5:小于等于,6:包含,7:日期当天
        /// 
        /// 
        /// Relationship取值：
        /// 0:And,1:Or
        /// 
        /// 以人员查询为例，查询isAdmin为true并且(name包含“a”或者loginName包含“a”)：
        /// 
        /// 
        ///          {
        ///	            "pageIndex": 0,
        ///	            "pageSize": 10,
        ///	            "search": [{
        ///	            	"key": "isAdmin",
        ///	            	"value": "true",
        ///	            	"operation": 0,
        ///	            	"relationship": 0,
        ///	            	"sequence": 1,
        ///	            	"Priorities": [{
        ///	            		"key": "name",
        ///	            		"value": "a",
        ///	            		"operation": 6,
        ///	            		"relationship": 0,
        ///	            		"sequence": 0
        ///
        ///                    }, {
        ///	            		"key": "loginName",
        ///	            		"value": "a",
        ///	            		"operation": 6,
        ///	            		"relationship": 1,
        ///	            		"sequence": 1
        ///	            	}]
        ///
        ///	            }]
        ///          }
        ///          
        /// 
        /// 
        ///          
        /// 查询支持逐级下探查询，例如查询职位所在部门包含监管部 (job.department.deptName包含"监管部")：
        /// 
        /// 
        ///      {
        ///     	    "pageIndex": 0,
        ///     	    "pageSize": 10,
        ///     
        ///     	    "search": [{
        ///     	    	"key": "department.deptName",
        ///     	    	"value": "监管部",
        ///     	    	"Operation": 6
        ///     
        ///             }]
        ///     }
        ///
        /// 查询签订日期为2020-12-08的数据，operation用7，value为字符串的日期格式：
        /// 
        ///
        ///     {
        ///         "pageIndex": 1,
        ///         "pageSize": 10,
        ///         "search": [{
        ///             "key": "signingDate",
        ///             "value": "2020-12-08",
        ///             "operation": 7
        ///         }]
        ///     }
        /// 
        /// </remarks>
        [HttpPost("Search")]
        [APIAuthAttribute]
        public virtual async Task<PaginatedList<PageView>> Search([FromBody] PaginatedSearchRequest paginatedSearchRequest)
        {
            if (string.IsNullOrEmpty(paginatedSearchRequest.OrderBy))
            {
                paginatedSearchRequest.OrderBy = "CreatedTime";
            }

            return await Task.FromResult(_service.GetAll(paginatedSearchRequest.PageIndex, paginatedSearchRequest.PageSize, paginatedSearchRequest.Search, paginatedSearchRequest.OrderBy, paginatedSearchRequest.OrderByAscent));
        }

        [HttpPost("ExcelFile")]
        [APIAuthAttribute]
        public IActionResult GetExcelFile([FromBody] PaginatedSearchRequest paginatedSearchRequest)
        {
            var dataList = _service.GetAll(paginatedSearchRequest.PageIndex, paginatedSearchRequest.PageSize, paginatedSearchRequest.Search, paginatedSearchRequest.OrderBy, paginatedSearchRequest.OrderByAscent);
            if (dataList != null)
            {
                var stream = _excelHandlerService.ExportToStream(dataList.Items);
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "导出结果.xlsx");
            }

            return null;
        }

        /// <summary>
        /// 根据Id删除
        /// </summary>
        /// <param name="id">要删除的Id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [APIAuthAttribute]
        public virtual CommonResponse Delete(string id)
        {
            try
            {
                var result = false;
                if (id.Contains(","))
                {
                    result = _service.BatchDelete(id);
                }
                else
                {
                    result = _service.Delete(int.Parse(id));
                }

                return new CommonResponse()
                {
                    code = result ? (int) ResponseCode.OK : (int) ResponseCode.BadRequest,
                    data = new
                    {
                        success = result
                    }
                };
            }
            catch (Exception e)
            {
                if (e.InnerException != null && e.InnerException.Message.Contains("conflict"))
                {
                    return new CommonResponse()
                    {
                        code = (int) ResponseCode.Conflict,
                        data = new
                        {
                            success = false,
                            message = e.Message
                        }
                    };
                }
                else
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// 更新实体类
        /// </summary>
        /// <param name="updateModel">要更新的实体类</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [APIAuthAttribute]
        public virtual CommonResponse Update([FromBody] PageInput updateModel, int id)
        {
            updateModel.Id = id;
            var result = _service.Update(updateModel);
            return new CommonResponse()
            {
                code = result ? (int) ResponseCode.OK : (int) ResponseCode.BadRequest,
                data = new
                {
                    Success = result
                }
            };
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="updateModels">更新集合</param>
        /// <returns></returns>
        [HttpPut]
        public virtual CommonResponse BatchUpdate([FromBody] List<PageInput> updateModels)
        {
            var result = _service.BatchUpdate(updateModels);
            return new CommonResponse()
            {
                code = result ? (int) ResponseCode.OK : (int) ResponseCode.BadRequest,
                data = new
                {
                    Success = result
                }
            };
        }

        /// <summary>
        /// 根据Id获取单条记录
        /// </summary>
        /// <param name="id">指定的记录Id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [APIAuthAttribute]
        public virtual PageView Get(int id)
        {
            return _service.GetOne(id);
        }

        /// <summary>
        /// 获得默认项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("firstOne")]
        [APIAuthAttribute]
        public virtual PageView FirstOne()
        {
            return _service.GetFirstDefault();
        }

        /// <summary>
        /// 获得最大Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetMaxId")]
        [APIAuthAttribute]
        public virtual int GetMaxId()
        {
            return _service.GetMaxId();
        }

        /// <summary>
        ///新增
        /// </summary>
        /// <param name="addModel">新增实体类</param>
        /// <returns></returns>
        [HttpPost]
        [APIAuthAttribute]
        public virtual CommonResponse Add([FromBody] PageInput addModel)
        {
            var result = _service.Add(addModel);

            return new CommonResponse()
            {
                code = result != 0 ? (int) ResponseCode.Created : (int) ResponseCode.BadRequest,
                data = new
                {
                    Success = result != 0 ? true : false,
                    AddedID = result
                }
            };
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="addModels">批量新增</param>
        /// <returns></returns>
        [HttpPost("Batch")]
        [APIAuthAttribute]
        public virtual CommonResponse BatchAdd([FromBody] List<PageInput> addModels)
        {
            var result = _service.BatchAdd(addModels);
            return new CommonResponse()
            {
                code = result != 0 ? (int) ResponseCode.Created : (int) ResponseCode.BadRequest,
                data = new
                {
                    Success = result != 0 ? true : false,
                    AddedID = result
                }
            };
        }

        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="checkKeyValues"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// Operation取值：
        /// 0:等于,1:不等于,2:大于,3:小于,4:大于等于,5:小于等于,6:包含
        /// 
        /// 
        /// Relationship取值：
        /// 0:And,1:Or
        /// 
        ///  /// 查询name等于张三:
        /// 
        ///        [
        ///         {
        ///           "key": "name",
        ///           "value": "张三",
        ///           "operation":0
        ///          }
        ///        ]
        ///        
        ///  查询name包含张三:
        /// 
        ///        [
        ///         {
        ///           "key": "name",
        ///           "value": "张三",
        ///           "operation":6
        ///          }
        ///        ]
        ///        
        ///        
        ///  查询name等于张三且Id等于2:
        /// 
        ///        [
        ///         {
        ///           "key": "name",
        ///           "value": "张三",
        ///           "operation":0,
        ///           "sequence": 0,
        ///          },
        ///          {
        ///            "key": "id",
        ///            "value": "2"
        ///            "operation":0,
        ///            "relationship":0
        ///            "sequence": 1,
        ///           }
        ///        ]
        ///        
        ///  查询name等于张三或者name等于李四:
        /// 
        ///        [
        ///         {
        ///           "key": "name",
        ///           "value": "张三",
        ///           "operation":0,
        ///           "sequence": 0,
        ///          },
        ///          {
        ///            "key": "name",
        ///            "value": "李四"
        ///            "operation":0,
        ///            "relationship":1
        ///            "sequence": 1,
        ///           }
        ///        ]
        /// </remarks>
        [HttpPost("Exists")]
        [APIAuthAttribute]
        public CommonResponse Exists([FromBody] List<SearchCondition> checkKeyValues)
        {
            var existsId = _service.Exists(checkKeyValues);
            return new CommonResponse()
            {
                code = (int) ResponseCode.OK,
                data = new
                {
                    Exists = existsId > 0,
                    Id = existsId,
                }
            };
        }
    }
}