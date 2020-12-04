

using API.Models.Data;
using API.Models.RequestModel;
using API.Models.ViewModel;

namespace API.Interfaces
{
    public interface IUserService : ICrudService<User, UserViewModel, UserRequest>
    {

    }
}
