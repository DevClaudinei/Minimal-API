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
        VerifyIfEmailExists(user.Email);
        VerifyIfNicknameExists(user.Nickname);

        var parameters = new
        {
            user.FirstName,
            user.LastName,
            user.Age,
            user.Email,
            user.Nickname,
            user.CreatedAt
        };

        using (var con = new MySqlConnection(_connectionString))
        {
            const string sql = "INSERT INTO Users (FirstName, LastName, Age, Email, Nickname, CreatedAt) " +
                "VALUES (@FirstName, @LastName, @Age, @Email, @Nickname, @CreatedAt); SELECT LAST_INSERT_ID();";
            var id = con.ExecuteScalar<long>(sql, parameters);

            return id;
        }
    }

    private void VerifyIfEmailExists(string? email)
    {
        var parameters = new
        {
            email
        };

        var emailFound = ConsultaEmailNoBanco(parameters);

        if (emailFound != null)
            throw new BadRequestException($"User already exists for email: {email}");
    }

    private User? ConsultaEmailNoBanco(object parameters)
    {
        using (var con = new MySqlConnection(_connectionString))
        {
            con.Open();

            const string sql = "SELECT * FROM Users WHERE Email = @email";
            var user = con.QuerySingleOrDefault<User>(sql, parameters);
            
            return user;
        }
    }

    private void VerifyIfNicknameExists(string? nickname)
    {
        var parameters = new
        {
            nickname
        };

        var emailFound = ConsultaNicknameNoBanco(parameters);

        if (emailFound != null)
            throw new BadRequestException($"User already exists for email: {nickname}");
    }

    private User? ConsultaNicknameNoBanco(object parameters)
    {
        using (var con = new MySqlConnection(_connectionString))
        {
            con.Open();

            const string sql = "SELECT * FROM Users WHERE Nickname = @nickname";
            var user = con.QuerySingleOrDefault<User>(sql, parameters);

            return user;
        }
    }

    public IEnumerable<User> GetAll()
    {
        string sql = @"SELECT * FROM Users";

        using (var con = new MySqlConnection(_connectionString))
        {
            return con.Query<User>(sql);
        }
    }

    public User? GetById(long id)
    {
        var parameters = new
        {
            id 
        };

        using (var con = new MySqlConnection(_connectionString))
        {
            con.Open();

            const string sql = "SELECT * FROM Users WHERE Id = @id";
            var user = con.QuerySingleOrDefault<User>(sql, parameters);

            return user;
        }
    }

    public void UpdateUser(long id, User user)
    {
        GetById(id);

        user.UpdatedAt = DateTime.Now;
        var parameters = new
        {
            id,
            user.FirstName,
            user.LastName,
            user.Age,
            user.Email,
            user.Nickname,
            user.UpdatedAt
        };

        using (var con = new MySqlConnection(_connectionString))
        {
            const string sql = "UPDATE Users SET FirstName = @FirstName, LastName = @LastName, Age = @Age, Email = @Email," +
                "Nickname = @Nickname, UpdatedAt = @UpdatedAt WHERE Id = @Id";

            con.Execute(sql, parameters);
        }
    }

    public void DeleteUser(long id)
    {
        throw new NotImplementedException();
    }
}
