using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class ReferenceRepository : IReferenceRepository
{
    private readonly IConnectionFactory connectionFactory;

    public ReferenceRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }


    public async Task<IEnumerable<Reference>> GetAllForContactInformation(Guid? contactInformationPK)
    {

        using (var connection = connectionFactory.CreateConnection())
        {
            var references = await connection.QueryAsync<Reference>(@"
                SELECT * 
                    FROM Reference
                    WHERE ContactInformationPK = @contactInformationPK
                ",
                new
                {
                    contactInformationPK
                });


            return references;
        }

    }

    public async Task<IEnumerable<Reference>> GetAllFromListOfContactInformation(List<Guid> contactInformationPK)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var references = await connection.QueryAsync<Reference>(@"
                SELECT * 
                    FROM Reference
                    WHERE ContactInformationPK IN @contactInformationPK
                ",
                new
                {
                    contactInformationPK
                });

            return references;
        }
    }
    public async Task<Reference> Add(Reference reference)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                INSERT INTO [dbo].[Reference]
                            ([ReferencePK]
                            ,[Name]
                            ,[Phone]
                            ,[Email]
                            ,[Comment]
                            ,[ContactInformationPK]
                )
                VALUES
                            (@ReferencePK
                            ,@Name
                            ,@Phone
                            ,@Email
                            ,@Comment
                            ,@ContactInformationPK
                )"
            , reference);

            return reference;
        }
    }

    public async Task<Reference> Update(Reference reference)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                UPDATE [dbo].[Reference]
                SET         [Name] = @Name
                           ,[Phone] = @Phone
                           ,[Email] = @Email
                           ,[Comment] = @Comment
                WHERE [ReferencePK] = @ReferencePK"
                , reference);

            return reference;
        }
    }


    public async Task Delete(Reference reference)
    {

        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                DELETE FROM [dbo].[Reference]
                WHERE [ReferencePK] = @ReferencePK"
                , reference);
        }
    }

}