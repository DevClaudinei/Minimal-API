using AutoMapper;
using Domain.Models;
using DomainServices;
using DomainServices.Exceptions;

namespace AppServices;

public class UserAppService : IUserAppService
{
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public UserAppService(IUserService userService, IMapper mapper)
    {
        _mapper = mapper;
        _userService = userService;
    }

    public long CreateUser(CreateUserRequest userRequest)
    {
        var user = _mapper.Map<User>(userRequest);
        var userId = _userService.CreateUser(user);

        return userId;
    }

    public IEnumerable<UserResult> GetAll()
    {
        var usersFound = _userService.GetAll();

        return _mapper.Map<IEnumerable<UserResult>>(usersFound);
    }

    public UserResult GetById(long id)
    {
        var userFound = _userService.GetById(id);

        if (userFound is null)
            throw new NotFoundException($"User for Id: {id} could not be found.");
        
        return _mapper.Map<UserResult>(userFound);
    }

    public void UpdateUser(long id, UpdateUserRequest userRequest)
    {
        var userToUpdate = _mapper.Map<User>(userRequest);

        _userService.UpdateUser(id, userToUpdate);
    }

    public void DeleteUser(long id)
    {
        throw new NotImplementedException();
    }
}
