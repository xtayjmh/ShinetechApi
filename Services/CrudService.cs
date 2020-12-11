using API.Interfaces;
using API.Models;
using API.Models.Data;
using API.Models.RequestModel;
using API.Models.ViewModel;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shinetech.Common;
using Shinetech.Infrastructure.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace API.Services
{
    public class CrudService<TableModel, PageView, PageInput> : ICrudService<TableModel, PageView, PageInput>
               where TableModel : BaseEntity
               where PageView : BaseViewModel
               where PageInput : BaseRequestModel
    {
        protected readonly ICrudRepository<TableModel> _repository;
        protected readonly IMapper _mapper;
        protected IUnitOfWork _unitOfWork;
        protected string _includeProperties = "";
        protected ICurrentUser _currentUser;
        protected readonly ICrudRepository<ActionLog> logRepository;

        public CrudService(IServiceProvider serviceProvider)
        {
            _repository = serviceProvider.GetService<ICrudRepository<TableModel>>(); ;
            _mapper = serviceProvider.GetService<IMapper>(); ;
            _unitOfWork = serviceProvider.GetService<IUnitOfWork>(); ;
            _currentUser = serviceProvider.GetService<ICurrentUser>();
            logRepository = serviceProvider.GetService<ICrudRepository<ActionLog>>();
            List<string> includes = AutoSetIncludeProperties("", typeof(TableModel));
            SetIncludeProperties(string.Join(',', includes));
        }

        protected virtual List<string> AutoSetIncludeProperties(string parentName, Type entityType)
        {
            List<string> includes = new List<string>();

            PropertyInfo[] props = entityType.GetProperties();
            foreach (PropertyInfo property in props)
            {
                if (property.PropertyType.Name== "List`1")
                {

                }
                if (property.PropertyType.Namespace.Equals(entityType.Namespace)||(property.PropertyType.Name == "List`1"&& property.PropertyType.GetGenericArguments()[0].Namespace.Equals(entityType.Namespace)))
                {
                    if (!string.IsNullOrEmpty(parentName) && !parentName.EndsWith("."))
                    {
                        parentName = parentName + ".";
                    }
                    includes.Add(parentName + property.Name);
                    includes.AddRange(AutoSetIncludeProperties(parentName + property.Name, property.PropertyType));
                }
            }
            return includes;
        }

        public virtual int Add(PageInput addModel, bool autoSave = true)
        {

            TableModel addDbModel = _mapper.Map<TableModel>(addModel);
            addDbModel.IsDelete = false;
            addDbModel.CreatedBy = _currentUser.Id;
            addDbModel.CreatedTime = DateTime.Now;
            addDbModel.UpdatedBy = _currentUser.Id;
            addDbModel.UpdatedTime = DateTime.Now;
            _repository.Insert(addDbModel);
            if (autoSave)
            {
                if (_repository.Save() > 0)
                {
                    logRepository.DbSet.Add(new ActionLog()
                    {
                        Who = _currentUser.Name,
                        Content = "新建" + addDbModel.GetEntityName()
                    });
                    logRepository.Save();
                    return addDbModel.Id;
                }
                else
                {
                    return 0;
                }
            }
            return addDbModel.Id;
        }
        public virtual int BatchAdd(List<PageInput> addModel, bool autoSave = true)
        {
            List<TableModel> addDbModels = _mapper.Map<List<TableModel>>(addModel);
            foreach (TableModel item in addDbModels)
            {
                item.IsDelete = false;
                item.CreatedBy = _currentUser.Id;
                item.CreatedTime = DateTime.Now;
                item.UpdatedBy = _currentUser.Id;
                item.UpdatedTime = DateTime.Now;
            }
            _repository.DbSet.AddRange(addDbModels);
            if (autoSave)
            {
                if (_repository.Save() > 0)
                {
                    if (addDbModels.Count > 0)
                    {
                        logRepository.DbSet.Add(new ActionLog()
                        {
                            Who = _currentUser.Name,
                            Content = "批量新建" + addDbModels[0].GetEntityName()
                        });
                        logRepository.Save();
                    }
                    return addDbModels.Count;
                }
                else
                {
                    return 0;
                }
            }


            return addDbModels.Count;
        }
        public virtual bool BatchDelete(string IDs, bool autoSave = true)
        {
            List<int> ids = IDs.Split(",").Select(r => int.Parse(r)).ToList(); ;
            List<TableModel> models = _repository.DbSet.Where(r => ids.Contains(r.Id)).ToList();
            foreach (TableModel item in models)
            {
                item.IsDelete = true;
                item.UpdatedBy = _currentUser.Id;
                item.UpdatedTime = DateTime.Now;
            }
            if (autoSave)
            {
                int result = _repository.Save();
                if (result > 0)
                {
                    if (models.Count > 0)
                    {
                        logRepository.DbSet.Add(new ActionLog()
                        {
                            Who = _currentUser.Name,
                            Content = "批量删除" + models[0].GetEntityName()
                        });
                        logRepository.Save();
                    }

                }
                return result > 0;
            }

            return false;
        }

        public virtual bool Delete(int Id, bool autoSave = true)
        {
            TableModel model = _repository.DbSet.Where(r => r.Id == Id).FirstOrDefault(); ;
            if (model != null)
            {
                model.IsDelete = true;
                model.UpdatedBy = _currentUser.Id;
                model.UpdatedTime = DateTime.Now;
            }
            if (autoSave)
            {
                int result = _repository.Save();
                if (result > 0)
                {
                    logRepository.DbSet.Add(new ActionLog()
                    {
                        Who = _currentUser.Name,
                        Content = "删除" + model.GetEntityName()
                    });
                    logRepository.Save();
                }
                return result > 0;
            }
            return false;
        }

 
        public virtual List<TableModel> Exists(Expression<Func<TableModel, bool>> predicate)
        {
            List<TableModel> models = _repository.Get(predicate.CombineWithAndAlso(r => r.IsDelete == false), includeProperties: _includeProperties).ToList();
            return models;
        }
        private object GetPropertyValue(object obj, string property)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(property.FirstCharToUpper());
            if (propertyInfo == null)
            {
                return null;
            }
            return propertyInfo.GetValue(obj, null);
        }

        public static Expression<Func<TPoco, bool>> GetEqualsPredicate<TPoco>(string propertyName, object value, Type fieldType)
        {

            ParameterExpression parameterExp = Expression.Parameter(typeof(TPoco), @"t");   //(tpoco t)
            MemberExpression propertyExp = Expression.Property(parameterExp, propertyName);// (tpoco t) => t.Propertyname

            ConstantExpression someValue = fieldType.IsEnum // get and eXpressionConstant.  Careful Enums must be reduced
                ? Expression.Constant(Enum.ToObject(fieldType, value)) // Marc Gravell fix
                : Expression.Constant(value, fieldType);

            BinaryExpression equalsExp = Expression.Equal(propertyExp, someValue); // yes this could 1 unreadble state if embedding someValue determination

            return Expression.Lambda<Func<TPoco, bool>>(equalsExp, parameterExp);
        }
        protected virtual Expression<Func<TableModel, bool>> KeyValueCheck(List<SearchCondition> checkKeyValues)
        {

            try
            {
                if (checkKeyValues == null || checkKeyValues.Count == 0)
                {
                    return r => true;
                }
                ParameterExpression pe = Expression.Parameter(typeof(TableModel));
                Expression combined = null;
                Expression<Func<TableModel, bool>> expression = null;
                int index = 0;

                foreach (SearchCondition keyword in checkKeyValues.OrderBy(r => r.Sequence))
                {
                    if (keyword.Priorities != null)
                    {
                        Expression<Func<TableModel, bool>> privoitiesExpression = KeyValueCheck(keyword.Priorities);
                        if (expression == null)
                        {
                            expression = privoitiesExpression;
                        }
                        else
                        {
                            switch (keyword.Relationship)
                            {
                                case ConditionRelationship.And:
                                    expression = expression.CombineWithAnd(privoitiesExpression);
                                    break;
                                case ConditionRelationship.Or:
                                    expression = expression.CombineWithOrElse(privoitiesExpression);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    string[] splitKeys = keyword.Key.Split(".");
                    Expression parameterExpresseion = pe;
                    Expression columnNameProperty = null;
                    foreach (string key in splitKeys)
                    {
                        columnNameProperty = Expression.Property(parameterExpresseion, key.FirstCharToUpper());
                        if (columnNameProperty.Type.BaseType == typeof(BaseEntity))
                        {
                            parameterExpresseion = columnNameProperty;
                        }
                    }

                    Expression columnValue = null;
                    Expression columnValue2 = null;
                    object objSearchValue = keyword.Value;
                    Type columnType = columnNameProperty.Type;
                    bool isNullable = false;
                    if (columnType.IsGenericType && columnType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        isNullable = true;
                        columnType = columnType.GetGenericArguments()[0];
                    }
                    if (objSearchValue == null)
                    {
                        columnValue = Expression.Constant(objSearchValue);
                    }
                    else
                    {
                        string searchValue =objSearchValue.ToString();
                        if (columnType == typeof(int))
                        {
                            columnValue = Expression.Constant(int.Parse(searchValue));
                            if (isNullable == true)
                            {
                                columnValue = Expression.Convert(columnValue, typeof(int?));
                            }
                        }
                        else if (columnType == typeof(bool))
                        {
                            columnValue = Expression.Constant(bool.Parse(searchValue));
                            if (isNullable == true)
                            {
                                columnValue = Expression.Convert(columnValue, typeof(bool?));
                            }
                        }
                        else if (columnType == typeof(string))
                        {
                            columnValue = Expression.Constant(searchValue);
                            if (isNullable == true)
                            {
                                columnValue = Expression.Convert(columnValue, typeof(DateTime?));
                            }
                        }
                        else if (columnType == typeof(Guid))
                        {
                            columnValue = Expression.Constant(Guid.Parse(searchValue));
                            if (isNullable == true)
                            {
                                columnValue = Expression.Convert(columnValue, typeof(Guid?));
                            }
                        }
                        else if (columnType == typeof(DateTime))
                        {
                            columnValue = Expression.Constant(DateTime.Parse(searchValue));
                            if (isNullable == true)
                            {
                                columnValue = Expression.Convert(columnValue, typeof(DateTime?));
                            }
                        }
                        else if (columnType.BaseType == typeof(Enum))
                        {
                            columnValue = Expression.Constant(Enum.Parse(columnType, objSearchValue.ToString()));
                        }
                        else
                        {
                            columnValue = Expression.Constant(objSearchValue);
                        }
                    }

                    Expression e1 = null;
                    Expression e2 = null;

                    switch (keyword.Operation)
                    {
                        case ConditionOperation.Equal:
                            e1 = Expression.Equal(columnNameProperty, columnValue);
                            break;
                        case ConditionOperation.NotEqual:
                            e1 = Expression.NotEqual(columnNameProperty, columnValue);
                            break;
                        case ConditionOperation.GreaterThan:
                            e1 = Expression.GreaterThan(columnNameProperty, columnValue);
                            break;
                        case ConditionOperation.LessThan:
                            e1 = Expression.LessThan(columnNameProperty, columnValue);
                            break;
                        case ConditionOperation.GreaterThanOrEqual:
                            e1 = Expression.GreaterThanOrEqual(columnNameProperty, columnValue);
                            break;
                        case ConditionOperation.LessThanOrEqual:
                            e1 = Expression.LessThanOrEqual(columnNameProperty, columnValue);
                            break;
                        case ConditionOperation.Contains:
                            if (columnNameProperty.Type == typeof(int))
                            {
                                e1 = Expression.Equal(columnNameProperty, columnValue);
                            }
                            else
                            {
                                MethodInfo method = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
                                e1 = Expression.Call(columnNameProperty, method, columnValue);
                            }
                            break;
                        case ConditionOperation.BetweenAnd:
                            e1 = Expression.GreaterThanOrEqual(columnNameProperty, columnValue);
                            e2 = Expression.LessThanOrEqual(columnNameProperty, columnValue2);
                            break;
                        default:
                            break;
                    }

                    if (combined == null)
                    {
                        combined = e1;
                        if (e1 != null && e2 != null && keyword.Operation == ConditionOperation.BetweenAnd)
                        {
                            combined = Expression.AndAlso(e1, e2);
                        }
                    }
                    else
                    {
                        switch (keyword.Relationship)
                        {
                            case ConditionRelationship.And:
                                combined = Expression.AndAlso(combined, e1);//AndAlso，用And不行，用and的时候，生成的sql语句中是&
                                break;
                            case ConditionRelationship.Or:
                                combined = Expression.OrElse(combined, e1); //要用OrElse，用Or不行，用or的时候，生成的sql语句中是单竖线|
                                break;
                            default:
                                break;
                        }

                    }
                    index++;


                }

                Expression<Func<TableModel, bool>> result = Expression.Lambda<Func<TableModel, bool>>(combined, new ParameterExpression[] { pe });
                if (expression == null)
                {
                    return result;
                }
                else
                {
                    return expression.CombineWithAnd(result);
                }
            }
            catch (Exception e)
            {
                throw new BusinessException((int)ResponseCode.BadRequest, e.Message);
            }


        }
        public virtual PaginatedList<PageView> GetAll(int pageIndex, int pageSize, List<SearchCondition> checkKeyValues, string orderBy = "", bool orderByAscent = true)
        {

            if (!string.IsNullOrEmpty(orderBy))
            {
                SetOrderBy(r => GetPropertyValue(r, orderBy));
            }
            SetOrderBySequence(orderByAscent);
            Expression<Func<TableModel, bool>> searchExpression = KeyValueCheck(checkKeyValues);

            PaginatedList<TableModel> list = _repository.GetPageList(searchExpression.CombineWithAndAlso(r => r.IsDelete == false), pageIndex, pageSize, _includeProperties);
            PaginatedList<PageView> vList = _mapper.Map<PaginatedList<PageView>>(list);
            return vList;
        }

        public virtual PageView GetFirstDefault()
        {
            TableModel model = null;
            if (!string.IsNullOrEmpty(_includeProperties))
            {
                model = _repository.DbSet.AsNoTracking().Include(_includeProperties).FirstOrDefault(r => r.IsDelete == false);
            }
            else
            {
                model = _repository.DbSet.AsNoTracking().FirstOrDefault(r => r.IsDelete == false);
            }

            if (model != null && model.IsDelete == true)
            {
                return null;
            }
            PageView vModel = _mapper.Map<PageView>(model);
            return vModel;
        }
        public virtual PageView GetOne(int Id)
        {
            TableModel model = _repository.GetById(Id, _includeProperties);
            if (model != null && model.IsDelete == true)
            {
                return null;
            }
            PageView vModel = _mapper.Map<PageView>(model);
            return vModel;
        }
        public virtual int GetMaxId()
        {
            if (_repository.DbSet.Any())
            {
                return _repository.DbSet.AsNoTracking().Max(r => r.Id);
            }
            else
            {
                return 0;
            }

        }
        public virtual void SetIncludeProperties(string includeProperties)
        {
            //if (string.IsNullOrEmpty(includeProperties))
            //{
            //    includeProperties = "CreatedByPerson,UpdateByPerson";
            //}
            //else if (!includeProperties.Contains("CreatedByPerson"))
            //{
            //    includeProperties += ",CreatedByPerson,UpdateByPerson";
            //}
            _includeProperties = includeProperties;
        }

        public virtual void SetOrderBy(Func<TableModel, object> func)
        {
            _repository.SetOrderByField(func);
        }

        public virtual void SetOrderBySequence(bool ascent)
        {
            _repository.SetOrderBySequence(ascent);
        }

        protected virtual void OnUpdateMapping(TableModel existsData, TableModel toUpdateData)
        {
            toUpdateData.CreatedBy = existsData.CreatedBy;
            toUpdateData.CreatedTime = existsData.CreatedTime;
        }
        public virtual bool Update(PageInput updateModel, bool autoSave = true)
        {
            TableModel updateDbModel = _mapper.Map<TableModel>(updateModel);
            TableModel dbExists = _repository.GetById(updateModel.Id, _includeProperties);
            if (dbExists != null)
            {
                OnUpdateMapping(dbExists, updateDbModel);
            }
            else
            {
                throw new BusinessException(400, "ToUpdatedNotExists");
            }

            updateDbModel.UpdatedBy = _currentUser.Id;
            updateDbModel.UpdatedTime = DateTime.Now;

            _repository.Update(updateDbModel);

            if (autoSave)
            {
                return _repository.Save() > 0;
            }
            return false;
        }
        public virtual bool BatchUpdate(List<PageInput> updateModel, bool autoSave = true)
        {
            List<TableModel> updateDbModels = _mapper.Map<List<TableModel>>(updateModel);
            foreach (TableModel updateDbModel in updateDbModels)
            {
                TableModel dbExists = _repository.GetById(updateDbModel.Id, _includeProperties);
                if (dbExists != null)
                {
                    OnUpdateMapping(dbExists, updateDbModel);
                }
                updateDbModel.UpdatedBy = _currentUser.Id;
                updateDbModel.UpdatedTime = DateTime.Now;
            }
            _repository.DbSet.UpdateRange(updateDbModels);

            if (autoSave)
            {
                return _repository.Save() > 0;
            }
            return false;
        }
        public virtual int Exists(List<SearchCondition> checkKeyValues)
        {
            int result = 0;
            if (checkKeyValues != null)
            {
                Expression<Func<TableModel, bool>> existsExpression = KeyValueCheck(checkKeyValues);
                var existsModels = Exists(existsExpression);
                if (existsModels != null)
                {
                    result = existsModels.Count;
                }
            }
            return result;
        }
    }
}
