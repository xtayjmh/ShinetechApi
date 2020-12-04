using API.Models;
using API.Models.RequestModel;
using API.Models.ViewModel;
using Shinetech.Infrastructure.Contract;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace API.Interfaces
{
    public interface ICrudService<TableModel, ViewModel, InputModel> : IService where TableModel : BaseEntity where ViewModel : BaseViewModel where InputModel : BaseRequestModel
    {
        int Add(InputModel addModel, bool autoSave = true);
        int BatchAdd(List<InputModel> addModel, bool autoSave = true);
        bool BatchDelete(string Ids, bool autoSave = true);
        bool Delete(int Id, bool autoSave = true);
        List<TableModel> Exists(Expression<Func<TableModel, bool>> predicate);
        int Exists(List<SearchCondition> checkKeyValues);
        PaginatedList<ViewModel> GetAll(int pageIndex, int pageSize, List<SearchCondition> checkKeyValues, string orderBy = "", bool orderByAscent = true);
        ViewModel GetOne(int Id);
        ViewModel GetFirstDefault();
        int GetMaxId();
        bool Update(InputModel updateModel, bool autoSave = true);
        bool BatchUpdate(List<InputModel> updateModels, bool autoSave = true);


        void SetIncludeProperties(string includeProperties);
        void SetOrderBy(Func<TableModel, Object> func);
        void SetOrderBySequence(bool ascent);
    }
}
