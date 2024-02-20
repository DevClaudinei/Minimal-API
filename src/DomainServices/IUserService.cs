using Domain.Models;

namespace DomainServices
{
    public interface IUserService
    {
        long CreateUser(User user);
        IEnumerable<User> GetAll();
        User? GetById(long id);
        void UpdateUser(long id, User user);
        void DeleteUser(long id);
    }
}
