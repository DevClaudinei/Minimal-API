using Domain.Models;

namespace AppServices
{
    public interface IUserAppService
    {
        long CreateUser(CreateUserRequest userRequest);
        IEnumerable<UserResult> GetAll();
        UserResult GetById(long id);
        void UpdateUser(long id, UpdateUserRequest userRequest);
        void DeleteUser(long id);
    }
}
