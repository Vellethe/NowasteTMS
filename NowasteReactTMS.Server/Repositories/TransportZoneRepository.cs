using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class TransportZoneRepository : ITransportZoneRepository
{
    private readonly IConnectionFactory connectionFactory;

    public TransportZoneRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public async Task<List<TransportZone>> GetAll()
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var transportZone = await connection.QueryAsync<TransportZone>(@"
                SELECT [TransportZonePK]
                      ,[Name]
                      ,[Description]
                  FROM [dbo].[TransportZone]");

            return transportZone.ToList();
        }
    }

    public async Task<TransportZone> GetForContactInformation(Guid contactInformationPK)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var transportZones = await connection.QueryAsync<TransportZone>(@"
                    SELECT tz.[TransportZonePK]
                          ,tz.[Name]
                          ,tz.[Description]
                          ,citz.[ContactInformationPK]
                    FROM [dbo].[TransportZone] tz
                    JOIN [dbo].[ContactInformationTransportZone] citz ON citz.TransportZonePK = tz.TransportZonePK
                    WHERE citz.ContactInformationPK = @contactInformationPK",
                new
                {
                    contactInformationPK
                });

            return transportZones.FirstOrDefault();
        }
    }

    public async Task<IEnumerable<TransportZone>> GetAllForContactInformation(Guid pk)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var transportZones = await connection.QueryAsync<TransportZone>(@"
                    SELECT tz.[TransportZonePK]
                          ,tz.[Name]
                          ,tz.[Description]
                          ,citz.[ContactInformationPK]
                    FROM [dbo].[TransportZone] tz
                    JOIN [dbo].[ContactInformationTransportZone] citz ON citz.TransportZonePK = tz.TransportZonePK
                    WHERE citz.ContactInformationPK = @pk
                       ",
                new
                {
                    pk
                });

            return transportZones;
        }
    }

    public async Task<IEnumerable<TransportZone>> GetAllForContactInformation(List<Guid> pk)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var transportZones = await connection.QueryAsync<TransportZone>(@"
                        SELECT tz.[TransportZonePK]
                              ,tz.[Name]
                              ,tz.[Description]
                              ,citz.[ContactInformationPK]
                        FROM [dbo].[TransportZone] tz
                        JOIN [dbo].[ContactInformationTransportZone] citz ON citz.TransportZonePK = tz.TransportZonePK
                        WHERE citz.ContactInformationPK IN @pk
                         ",
                new
                {
                    pk
                });

            return transportZones;
        }
    }

    public async Task<TransportZone> Create(TransportZone transportZone)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                INSERT INTO [dbo].[TransportZone]
                           ([TransportZonePK]
                           ,[Name]
                           ,[Description])
                     VALUES
                           (@TransportZonePK
                           ,@Name
                           ,@Description)",
                transportZone);
        }
        return transportZone;
    }

    public async Task<int> Connect(Guid transportZonePK, Guid contactInformationPK)
    {
        if (await Get(transportZonePK) == null)
            return 0;

        using (var connection = connectionFactory.CreateConnection())
        {
            return await connection.ExecuteAsync(@"
                INSERT INTO [dbo].[ContactInformationTransportZone]
                           ([ContactInformationPK]
                           ,[TransportZonePK])
                     VALUES
                           (@contactInformationPK,
                            @transportZonePK)",
                 new
                 {
                     transportZonePK,
                     contactInformationPK
                 });
        }
    }

    public async Task<int> Disconnect(Guid transportZonePK, Guid contactInformationPK)
    {
        if (await Get(transportZonePK) == null)
            return 0;

        using (var connection = connectionFactory.CreateConnection())
        {
            return await connection.ExecuteAsync(@"
                DELETE FROM [dbo].[ContactInformationTransportZone]
                      WHERE [ContactInformationPK] = @contactInformationPK
                        AND [TransportZonePK] = @transportZonePK",
                new
                {
                    transportZonePK,
                    contactInformationPK
                });
        }
    }

    public async Task<TransportZone> Get(Guid pk)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var transportZone = await connection.QueryFirstOrDefaultAsync<TransportZone>(@"
                SELECT [TransportZonePK]
                      ,[Name]
                      ,[Description]
                  FROM [dbo].[TransportZone]
                WHERE [TransportZonePK] = @pk",
                new
                {
                    pk
                });

            return transportZone;
        }
    }
}