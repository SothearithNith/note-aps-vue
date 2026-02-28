namespace NotesApi.Repositories;

using Dapper;
using Microsoft.Data.SqlClient;
using NotesApi.Models;

public class UserRepository
{
    private readonly IConfiguration _config;
    public UserRepository(IConfiguration config)
    {
        _config = config;
    }

    private SqlConnection GetConnection() => new SqlConnection(_config.GetConnectionString("DefaultConnection"));

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var conn = GetConnection();
        var sql = "SELECT * FROM Users WHERE Username = @Username";
        return await conn.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
    }

    public async Task<int> CreateAsync(User user)
    {
        using var conn = GetConnection();
        var sql = "INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @PasswordHash); SELECT CAST(SCOPE_IDENTITY() as int)";
        return await conn.ExecuteScalarAsync<int>(sql, user);
    }
}