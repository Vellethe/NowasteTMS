using Dapper;
using NowasteTms.Model;
using System.Transactions;
using WMan.Data.ConnectionFactory;
public class ContactInformationRepository : IContactInformationRepository
{

    private readonly IConnectionFactory connectionFactory;
    private readonly IReferenceRepository referenceRepository;
    private readonly ITransportZoneRepository transportZoneRepository;

    public ContactInformationRepository(IConnectionFactory connectionFactory,
        IReferenceRepository referenceRepository,
        ITransportZoneRepository transportZoneRepository)
    {
        this.connectionFactory = connectionFactory;
        this.referenceRepository = referenceRepository;
        this.transportZoneRepository = transportZoneRepository;
    }

    public async Task<ContactInformation> Get(Guid pk)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var contactInformation = await connection.QueryFirstOrDefaultAsync<ContactInformation>(@"
                SELECT * 
                    FROM ContactInformation
                    WHERE ContactInformationPK = @pk
                ",
                new
                {
                    pk
                });

            contactInformation.References = await referenceRepository.GetAllForContactInformation(pk);
            contactInformation.TransportZones = await transportZoneRepository.GetAllForContactInformation(pk);
            return contactInformation;
        }
    }

    public async Task<IEnumerable<ContactInformation>> GetAll()
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var contactInformations =
                await connection.QueryAsync<ContactInformation>("SELECT * FROM ContactInformation");

            return contactInformations;
        }
    }

    public async Task<IEnumerable<ContactInformation>> GetForBusinessUnits(IEnumerable<Guid> businessUnitPKs)
    {
        if (!businessUnitPKs.Any())
            return new List<ContactInformation>();

        using (var connection = connectionFactory.CreateConnection())
        {
            var contactInformations = await connection.QueryAsync<ContactInformation>(@"
                SELECT * 
                    FROM  [ContactInformation]
                    WHERE [BusinessUnitPK] IN @businessUnitPKs
                ",
                new
                {
                    businessUnitPKs
                });
            var listGuid = contactInformations.Select(c => c.ContactInformationPK).ToList();

            var allRefRepo =
                await referenceRepository.GetAllFromListOfContactInformation(listGuid);

            var allTransZoneRepo =
                await transportZoneRepository.GetAllForContactInformation(listGuid);
            foreach (var c in contactInformations)
            {
                c.References = allRefRepo.Where(v => v.ContactInformationPK == c.ContactInformationPK);
                c.TransportZones = allTransZoneRepo.Where(b => b.ContactInformationPK == c.ContactInformationPK);
            }

            return contactInformations;
        }
    }

    public async Task<ContactInformation> Add(ContactInformation ci)
    {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                INSERT INTO [dbo].[ContactInformation]
                            ([ContactInformationPK]
                           ,[IsDefault]
                           ,[Phone]
                           ,[CellularPhone]
                           ,[Email]
                           ,[Fax]
                           ,[PreferredCommunication]
                           ,[Address]
                           ,[Zipcode]
                           ,[City]
                           ,[Country]
                           ,[BusinessUnitPK]
                           ,[ExternalId]
                           ,[IsActive]
                           ,[Description]
                           ,[IsEditable])

                VALUES      (@ContactInformationPK
                           ,@IsDefault
                           ,@Phone
                           ,@CellularPhone
                           ,@Email
                           ,@Fax
                           ,1
                           ,@Address
                           ,@Zipcode
                           ,@City
                           ,@Country
                           ,@BusinessUnitPK
                           ,@ExternalId
                           ,@IsActive
                           ,@Description
                           ,@IsEditable)",
                ci);

            if (null != ci.References)
            {
                foreach (var r in ci.References)
                {
                    r.ContactInformationPK = ci.ContactInformationPK;
                    await referenceRepository.Add(r);
                }
            }

            scope.Complete();

            return ci;
        }
    }

    public async Task<ContactInformation> Update(ContactInformation ci)
    {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                UPDATE [dbo].[ContactInformation]
                SET         [Phone] = @Phone
                           ,[IsDefault] = @IsDefault
                           ,[CellularPhone] = @CellularPhone
                           ,[Email] = @Email
                           ,[Fax] = @Fax
                           ,[Address] = @Address
                           ,[Zipcode] = @Zipcode
                           ,[City] = @City
                           ,[Country] = @Country
                           ,[ExternalId] = @ExternalId
                           ,[IsActive] = @IsActive
                           ,[Description] = @Description
                           ,[IsEditable] = @IsEditable
                WHERE [ContactInformationPK] = @ContactInformationPK", ci);

            var existingReferences = await referenceRepository.GetAllForContactInformation(ci.ContactInformationPK);

            var referencesToDelete = ci.References != null ?
                existingReferences.Where(r => !ci.References.Any(r2 => r2.ReferencePK == r.ReferencePK)) : existingReferences;

            foreach (var r in referencesToDelete)
            {
                await referenceRepository.Delete(r);
            }

            if (null != ci.References)
            {
                var referencesToAdd =
                    ci.References.Where(r => !existingReferences.Any(r2 => r2.ReferencePK == r.ReferencePK));

                foreach (var r in ci.References)
                {
                    if (referencesToAdd.Contains(r))
                    {
                        r.ContactInformationPK = ci.ContactInformationPK;
                        await referenceRepository.Add(r);
                    }
                    else
                    {
                        await referenceRepository.Update(r);
                    }
                }
            }

            scope.Complete();

            return ci;
        }
    }

    public async Task Delete(ContactInformation ci)
    {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        using (var connection = connectionFactory.CreateConnection())
        {
            if (ci.References != null)
            {
                foreach (var r in ci.References)
                {
                    await referenceRepository.Delete(r);
                }
            }

            await connection.ExecuteAsync(@"
                DELETE FROM [dbo].[ContactInformation]
                WHERE [ContactInformationPK] = @ContactInformationPK", ci);

            scope.Complete();
        }
    }
}