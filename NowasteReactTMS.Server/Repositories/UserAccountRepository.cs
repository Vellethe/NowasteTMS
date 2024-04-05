using Dapper;
using Microsoft.AspNetCore.Identity;
using NowastePalletPortal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using WMan.Data.ConnectionFactory;

namespace NowastePalletPortal.Extensions.Helpers
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly IConnectionFactory connectionFactory;

        public UserAccountRepository(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task UpdateUser(ApplicationUser user)
        {
            using (var connection = connectionFactory.CreateConnection())
            {
                const string sqlUpdate = @"UPDATE [dbo].[AspNetUsers]
                               SET [PhoneNumber] = @PhoneNumber
                                  ,[BusinessUnitId] = @BusinessUnitId
                                  ,[PasswordHash] = @PasswordHash
                                  ,[DivisionId] = @DivisionId
                               WHERE [Id] = @Id;";
                await connection.ExecuteAsync(sqlUpdate, new
                {
                    user.PhoneNumber,
                    user.BusinessUnitId,
                    user.PasswordHash,
                    user.DivisionId,
                    user.Id
                });
            }
        }

        public async Task SetUserPalletAccount(string userId, long palletAccountId)
        {
            using (var connection = connectionFactory.CreateConnection())
            {
                const string sqlInsert = @"UPDATE [dbo].[AspNetUsers]
                               SET [PalletAccountId] = @PalletAccountId
                               WHERE [Id] = @UserId;";
                await connection.ExecuteAsync(sqlInsert, new { UserId = userId, PalletAccountId = palletAccountId });
            }
        }

        public async Task SetUserRole(string userId, List<string> rolesForUser)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var connection = connectionFactory.CreateConnection())
            {
                // Delete old values
                const string sqlDelete = @"DELETE FROM [dbo].[AspNetUserRoles] WHERE [UserId] = @userId";
                await connection.QueryAsync<IdentityUserRole<string>>(sqlDelete, new { UserId = userId });

                foreach (var roleId in rolesForUser)
                {
                    // Insert new values
                    const string sqlInsert = @"INSERT INTO [dbo].[AspNetUserRoles]
                                ([UserId]
                                 ,[RoleId])
                    VALUES
                    (@UserId, @RoleId);";
                    await connection.ExecuteAsync(sqlInsert, new { UserId = userId, RoleId = roleId });
                }

                scope.Complete();
            }
        }

        public async Task<IEnumerable<Division>> GetDivisions()
        {
            using (var connection = connectionFactory.CreateConnection())
            {
                const string sql = @"SELECT [Id]
                                           ,[Name]
                                        FROM [dbo].[Division];";
                return await connection.QueryAsync<Division>(sql);
            }
        }
    }
}
