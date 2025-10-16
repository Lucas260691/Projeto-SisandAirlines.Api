using Dapper;
using System.Data;

namespace SisandAirlines.Infrastructure.Repositories
{
    /// <summary>
    /// Repositório de auditoria específico para operações de pagamento.
    /// Internamente grava registros na tabela audit_log.
    /// </summary>
    public class PaymentAuditRepository
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction? _transaction;

        public PaymentAuditRepository(IDbConnection connection, IDbTransaction? transaction = null)
        {
            _connection = connection;
            _transaction = transaction;
        }

        /// <summary>
        /// Adiciona um registro de auditoria na tabela audit_log.
        /// </summary>
        /// <param name="paymentId">ID do pagamento associado.</param>
        /// <param name="action">Tipo de ação executada (ex: INSERT, UPDATE, DELETE).</param>
        /// <param name="message">Mensagem descritiva da operação.</param>
        /// <param name="userEmail">Usuário responsável, se disponível.</param>
        public async Task AddAsync(int paymentId, string action, string message, string? userEmail = null)
        {
            const string sql = @"
                INSERT INTO audit_log (table_name, operation, record_id, user_email, log_time)
                VALUES ('payment', @Action, @PaymentId, @UserEmail, NOW());
            ";

            await _connection.ExecuteAsync(sql, new
            {
                PaymentId = paymentId,
                Action = action,
                UserEmail = userEmail
            }, transaction: _transaction);
        }
    }
}

