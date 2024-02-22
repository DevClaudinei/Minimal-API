using API.Configurations;
using AppServices;
using Domain.Models;
using DomainServices;
using DomainServices.Exceptions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IUserAppService, UserAppService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddAutoMapperConfiguration();
builder.Services.AddMvcConfiguration();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapPost("/users", (IUserAppService userAppService, CreateUserRequest createUserRequest) =>
{
    try
    {
        var userId = userAppService.CreateUser(createUserRequest);
        return Results.Created("Id: ", userId);
    }
    catch (BadRequestException ex)
    {
        return Results.BadRequest($"{ex.Message}");
    }
}).Produces<long>();

app.MapGet("/users", (IUserAppService userAppService) =>
{
    var users = userAppService.GetAll();
    return Results.Ok(users);
});

app.MapGet("/users/{id}", (IUserAppService userAppService, long id) =>
{
    try
    {
        var user = userAppService.GetById(id);
        return Results.Ok(user);
    }
    catch (NotFoundException ex)
    {
        return Results.NotFound($"{ex.Message}");
    }
}).Produces<User>();

app.MapPut("/users/{id}", (IUserAppService userAppService, long id, UpdateUserRequest updateUserRequest) =>
{
    try
    {
        userAppService.UpdateUser(id, updateUserRequest);
        return Results.NoContent();
    } catch (BadRequestException ex)
    {
        return Results.BadRequest($"{ex.Message}");
    } catch (NotFoundException ex)
    {
        return Results.NotFound($"{ex.Message}");
    }
});

app.MapDelete("/users/{id}", (IUserAppService userAppService, long id) =>
{
    try
    {
        userAppService.DeleteUser(id);
        return Results.NoContent();
    }
    catch (NotFoundException ex)
    {
        return Results.NotFound($"{ex.Message}");
    }
});

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
