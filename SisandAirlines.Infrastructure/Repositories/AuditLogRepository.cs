using Dapper;
using System.Data;

namespace SisandAirlines.Infrastructure.Repositories
{
    
    public class AuditLogRepository
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction? _transaction;

        public AuditLogRepository(IDbConnection connection, IDbTransaction? transaction = null)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task AddAsync(string tableName, string operation, int recordId, string? userEmail = null)
        {
            const string sql = @"
                INSERT INTO audit_log (table_name, operation, record_id, user_email, log_time)
                VALUES (@TableName, @Operation, @RecordId, @UserEmail, NOW());";
            await _connection.ExecuteAsync(sql, new
            {
                TableName = tableName,
                Operation = operation,
                RecordId = recordId,
                UserEmail = userEmail
            }, transaction: _transaction);
        }
    }
}
