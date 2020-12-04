using API.Interfaces;
using API.Models.Data;
using API.Models.RequestModel;
using API.Models.ViewModel;
using Shinetech.Common;
using System;

namespace API.Services
{
    public class UserService : CrudService<User, UserViewModel, UserRequest>, IUserService
    {
        public UserService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// 重写添加，密码加密
        /// </summary>
        /// <param name="addModel"></param>
        /// <param name="autoSave"></param>
        /// <returns></returns>
        public override int Add(UserRequest addModel, bool autoSave = true)
        {
          

            addModel.Password = CryptoHelper.Crypto.HashPassword(addModel.Password);
            return base.Add(addModel, autoSave);
        }

        /// <summary>
        /// 重写更新， 密码加密
        /// </summary>
        /// <param name="updateModel"></param>
        /// <param name="autoSave"></param>
        /// <returns></returns>
        public override bool Update(UserRequest updateModel, bool autoSave = true)
        {
            var toModity = _repository.GetById(updateModel.Id);
            if (toModity == null)
            {
                throw new BusinessException(400, "UserNotExists");
            }
            if (!string.IsNullOrEmpty(updateModel.Password))
            {
                updateModel.Password = CryptoHelper.Crypto.HashPassword(updateModel.Password);
            }
            else
            {
                updateModel.Password = toModity.Password;
            }
            return base.Update(updateModel, autoSave);
        }


    }
}