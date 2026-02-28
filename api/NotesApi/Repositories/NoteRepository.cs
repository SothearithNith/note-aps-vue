using Dapper;
using NotesApi.Models;
using System.Data;

namespace NotesApi.Repositories
{
    public class NoteRepository
    {
        private readonly IDbConnection _db;

        public NoteRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> CreateAsync(Note note)
        {
            var sql = @"
                INSERT INTO Notes (UserId, Title, Content, CreatedAt, UpdatedAt)
                VALUES (@UserId, @Title, @Content, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int)";
            return await _db.QuerySingleAsync<int>(sql, note);
        }

        public async Task<IEnumerable<Note>> GetByUserIdAsync(int userId)
        {
            var sql = "SELECT * FROM Notes WHERE UserId = @UserId";
            return await _db.QueryAsync<Note>(sql, new { UserId = userId });
        }

        public async Task<Note?> GetByIdAsync(int id, int userId)
        {
            var sql = "SELECT * FROM Notes WHERE Id = @Id AND UserId = @UserId";
            return await _db.QueryFirstOrDefaultAsync<Note>(sql, new { Id = id, UserId = userId });
        }

        public async Task<bool> UpdateAsync(Note note)
        {
            note.UpdatedAt = DateTime.UtcNow;
            var sql = @"
                UPDATE Notes
                SET Title = @Title,
                    Content = @Content,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id AND UserId = @UserId";
            var affectedRows = await _db.ExecuteAsync(sql, note);
            return affectedRows > 0;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var sql = "DELETE FROM Notes WHERE Id = @Id AND UserId = @UserId";
            var affectedRows = await _db.ExecuteAsync(sql, new { Id = id, UserId = userId });
            return affectedRows > 0;
        }
    }
}