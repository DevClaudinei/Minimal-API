using Dapper;
using Domain.Models;
using DomainServices.Exceptions;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace DomainServices;


public class UserService : IUserService
{
    private readonly IConfiguration _configuration;
    private readonly string? _connectionString;

    public UserService(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("mysql");
    }

    public IDbConnection Connection
    {
        get
        {
            return new MySqlConnection(_connectionString);
        }
    }

    public long CreateUser(User user)
    {
        using (var con = Connection)
        {
            con.Open();

            using (var transaction = con.BeginTransaction())
            {
                VerifyIfEmailExists(user.Email);
                VerifyIfNickNameExists(user.NickName);

                var parameters = new
                {
                    user.FirstName,
                    user.LastName,
                    user.Age,
                    user.Email,
                    user.NickName,
                    user.CreatedAt
                };

                const string sql = "INSERT INTO Users (FirstName, LastName, Age, Email, NickName, CreatedAt) " +
                    "VALUES (@FirstName, @LastName, @Age, @Email, @NickName, @CreatedAt); SELECT LAST_INSERT_ID();";
                var id = con.ExecuteScalar<long>(sql, parameters);

                transaction.Commit();
                return id;
            }
        }
    }

    private void VerifyIfEmailExists(string? email)
    {
        var parameters = new
        {
            email
        };

        var emailFound = VerifyIfEmailAlreadyExistsOnDatabase(parameters);

        if (emailFound)
            throw new BadRequestException($"User already exists for email: {email}");
    }

    private bool VerifyIfEmailAlreadyExistsOnDatabase(object parameters)
    {
        const string sql = "SELECT * FROM Users WHERE Email = @email";

        return ExecuteQuery(sql, parameters).Any();
    }

    private void VerifyIfNickNameExists(string? nickname)
    {
        var parameters = new
        {
            nickname
        };

        var emailFound = VerifyIfNickNameAlreadyExistsOnDatabase(parameters);

        if (emailFound)
            throw new BadRequestException($"User already exists for nickname: {nickname}");
    }

    private bool VerifyIfNickNameAlreadyExistsOnDatabase(object parameters)
    {
        const string sql = "SELECT * FROM Users WHERE Nickname = @nickname";
        
        return ExecuteQuery(sql, parameters).Any();
    }

    private IEnumerable<User> ExecuteQuery(string sql, object parameters)
    {
        using (var con = new MySqlConnection(_connectionString))
        {
            con.Open();
            return con.Query<User>(sql, parameters);
        }
    }

    public IEnumerable<User> GetAll()
    {
        string sql = @"SELECT * FROM Users";

        using (var con = Connection)
        {
            return con.Query<User>(sql);
        }
    }

    public User? GetById(long id)
    {
        var parameters = new { id };

        using (var con = Connection)
        {
            con.Open();
            const string sql = "SELECT * FROM Users WHERE Id = @id";
            return con.QuerySingleOrDefault<User>(sql, parameters);
        }
    }

    private bool VerifyIfUserExists(IDbConnection con, long id)
    {
        var parameters = new { id };

        const string sql = "SELECT * FROM Users WHERE Id = @id";
        var user = con.QuerySingleOrDefault<User>(sql, parameters);

        if (user is null)
            throw new NotFoundException($"User for Id: {id} could not be found.");

        return true;
    }

    public void UpdateUser(long id, User user)
    {
        using (var con = Connection)
        {
            con.Open();

            using (var transaction = con.BeginTransaction())
            {
                VerifyIfUserExists(con, id);

                user.UpdatedAt = DateTime.Now;
                var parameters = new
                {
                    id,
                    user.FirstName,
                    user.LastName,
                    user.Age,
                    user.Email,
                    user.NickName,
                    user.UpdatedAt
                };

                const string sql = "UPDATE Users SET FirstName = @FirstName, LastName = @LastName, Age = @Age, " +
                    "Email = @Email, Nickname = @Nickname, UpdatedAt = @UpdatedAt WHERE Id = @Id";

                con.Execute(sql, parameters, transaction);
                transaction.Commit();
            }
        }
    }

    public void DeleteUser(long id)
    {
        using (var con = Connection)
        {
            con.Open();

            using (var transaction = con.BeginTransaction())
            {
                VerifyIfUserExists(con, id);

                var parameters = new { id };

                const string sql = "DELETE FROM Users WHERE Id = @Id";
                con.Execute(sql, parameters, transaction);

                transaction.Commit();
            }
        }
    }
}
