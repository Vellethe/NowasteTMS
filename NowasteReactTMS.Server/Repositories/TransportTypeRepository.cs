using Dapper;
using NowasteReactTMS.Server.Repositories.Interface;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;

namespace NowasteReactTMS.Server.Repositories
{
    public class TransportTypeRepository : ITransportTypeRepository
    {
        private readonly IConnectionFactory _connectionFactory;
        public TransportTypeRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<TransportType>> GetAll()
        {
            using (var conn = _connectionFactory.CreateConnection())
            {
                var result = await conn.QueryAsync<TransportType, PalletType, TransportType>(@"
                SELECT tt.[TransportTypePK]
                      ,tt.[Description]
                      ,tt.[PalletTypeId]
                      
                      ,pt.[Id]
                      ,pt.[Description]
                      ,pt.[Stamp]
                      ,pt.[Footprint]
                  FROM [dbo].[TransportType] tt
                JOIN [PalletType] pt ON tt.[PalletTypeId] = pt.[Id]",
                    (tt, pt) =>
                    {
                        tt.PalletType = pt;

                        return tt;
                    });

                return result.ToList();
            }
        }

        public async Task<int> Add(TransportType transportType)
        {
            using (var conn = _connectionFactory.CreateConnection())
            {
                return await conn.ExecuteAsync(@"
                INSERT INTO [dbo].[TransportType]
                           ([TransportTypePK]
                           ,[Description]
                           ,[PalletTypeId])
                     VALUES
                           (@TransportTypePK
                           ,@Description
                           ,@PalletTypeId)",
                transportType);
            }
        }

        public async Task<TransportType> Get(Guid pk)
        {
            using (var conn = _connectionFactory.CreateConnection())
            {
                var transportType = await conn.QueryAsync<TransportType, PalletType, TransportType>(@"
                SELECT tt.[TransportTypePK]
                      ,tt.[Description]
                      ,tt.[PalletTypeId]
                      
                      ,pt.[Id]
                      ,pt.[Description]
                      ,pt.[Stamp]
                      ,pt.[Footprint]
                  FROM [dbo].[TransportType] tt
                JOIN [PalletType] pt ON tt.[PalletTypeId] = pt.[Id]
                WHERE [TransportTypePK] = @pk",
                (tt, pt) =>
                {
                    tt.PalletType = pt;

                    return tt;
                },
                new
                {
                    pk
                });

                return transportType.FirstOrDefault();
            }
        }

        public async Task<int> Update(TransportType transportType)
        {
            using (var conn = _connectionFactory.CreateConnection())
            {
                return await conn.ExecuteAsync(@"
                UPDATE [dbo].[TransportType]
                   SET [TransportTypePK] = @TransportTypePK
                      ,[Description] = @Description
                      ,[PalletTypeId] = @PalletTypeId
                 WHERE [TransportTypePK] = @TransportTypeId", transportType);
            }
        }
    }
}
