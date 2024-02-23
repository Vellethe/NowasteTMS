using Dapper;
using NowasteTms.Model;
using System.Transactions;
using WMan.Data.ConnectionFactory;

public class BusinessUnitRepository : IBusinessUnitRepository
{
    private readonly IConnectionFactory connectionFactory;
    private readonly IContactInformationRepository contactInformationRepository;

    public BusinessUnitRepository(IConnectionFactory connectionFactory, IContactInformationRepository contactInformationRepository)
    {
        this.connectionFactory = connectionFactory;
        this.contactInformationRepository = contactInformationRepository;
    }

    public async Task<List<BusinessUnit>> GetAll()
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var businessUnits = await connection.QueryAsync<BusinessUnit>("SELECT * FROM BusinessUnit ORDER BY [Name]");

            return businessUnits.ToList();
        }
    }

    public async Task<BusinessUnit> GetByContactInformationPk(Guid toContactInformationPk)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var businessUnits = await connection.QueryAsync<BusinessUnit, FinanceInformation, Currency, BusinessUnit>(@"
                SELECT 
                       bu.[BusinessUnitPK] AS Id
                      ,bu.*

                      ,fi.[FinanceInformationPK] AS Id
                      ,fi.*

                      ,[cu].[CurrencyPK] AS Id
                      ,[cu].[CurrencyPK]
                      ,[cu].[Name]
                      ,[cu].[ShortName]

                  FROM [dbo].[BusinessUnit] bu
                LEFT JOIN [dbo].[FinanceInformation] fi ON bu.FinanceInformationPK = fi.FinanceInformationPK
                LEFT JOIN [dbo].[ContactInformation] ci ON bu.BusinessUnitPK = ci.BusinessUnitPK
                JOIN [Currency] cu ON [fi].[CurrencyPK] = cu.[CurrencyPK]
                WHERE ci.[ContactInformationPK] = @toContactInformationPk"
                , (bu, fi, cu) =>
                {
                    bu.FinanceInformation = fi;
                    bu.FinanceInformation.Currency = cu;

                    return bu;
                },
                new
                {
                    toContactInformationPk
                });

            var contactInformations = await contactInformationRepository.GetForBusinessUnits(new List<Guid> { toContactInformationPk });
            foreach (var bu in businessUnits)
            {
                bu.ContactInformations = contactInformations.Where(ci => ci.BusinessUnitPK == bu.BusinessUnitPK);
            }

            return businessUnits.FirstOrDefault();
        }
    }

    public async Task<BusinessUnit> Get(Guid pk)
    {
        var businessUnits = await Get(new Guid[] { pk });
        return businessUnits.FirstOrDefault();
    }

    public async Task<IEnumerable<BusinessUnit>> Get(IEnumerable<Guid> businessUnitPKs)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var businessUnits = await connection.QueryAsync<BusinessUnit, FinanceInformation, Currency, BusinessUnit>(@"
                SELECT 
                       bu.[BusinessUnitPK] AS Id
                      ,bu.*

                      ,fi.[FinanceInformationPK] AS Id
                      ,fi.*

                      ,[cu].[CurrencyPK] AS Id
                      ,[cu].[CurrencyPK]
                      ,[cu].[Name]
                      ,[cu].[ShortName]

                  FROM [dbo].[BusinessUnit] bu
                LEFT JOIN [dbo].[FinanceInformation] fi ON bu.FinanceInformationPK = fi.FinanceInformationPK
                JOIN [Currency] cu ON [fi].[CurrencyPK] = cu.[CurrencyPK]
                WHERE bu.[BusinessUnitPK] IN @businessUnitPKs",
                (bu, fi, cu) =>
                {
                    bu.FinanceInformation = fi;
                    bu.FinanceInformation.Currency = cu;

                    return bu;
                },
                new
                {
                    businessUnitPKs
                });

            //
            // Add related Contactinformation
            //
            var contactInformations = await contactInformationRepository.GetForBusinessUnits(businessUnitPKs);
            foreach (var bu in businessUnits)
                bu.ContactInformations = contactInformations.Where(ci => ci.BusinessUnitPK == bu.BusinessUnitPK);


            return businessUnits;
        }
    }

    public async Task<BusinessUnit> Update(BusinessUnit bu)
    {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                UPDATE [dbo].[BusinessUnit]
                SET         [Name] = @Name
                           ,[Company] = @Company
                           ,[DivisionId] = @DivisionId
                           ,[IsEditable] = @IsEditable
                           ,[TimeStamp] = GETDATE()
                WHERE [BusinessUnitPK] = @BusinessUnitPK", bu);

            var existingContactInformations = await contactInformationRepository.GetForBusinessUnits(new[] { bu.BusinessUnitPK });

            //var contactInformationsToDelete = bu.ContactInformations != null ?
            //    existingContactInformations.Where(r => bu.ContactInformations.All(r2 => r2.ContactInformationPK != r.ContactInformationPK)) : existingContactInformations;

            var contactInformationsToDelete = bu.ContactInformations.Where(ci => ci.IsActive == false);

            foreach (var c in contactInformationsToDelete)
                await contactInformationRepository.Delete(c);

            var contactInformationsToAdd =
                bu.ContactInformations.Where(r => !existingContactInformations.Any(r2 => r2.ContactInformationPK == r.ContactInformationPK));

            foreach (var c in bu.ContactInformations)
            {
                if (contactInformationsToAdd.Contains(c))
                {
                    c.BusinessUnitPK = bu.BusinessUnitPK;
                    await contactInformationRepository.Add(c);
                }
                else
                    await contactInformationRepository.Update(c);
            }

            await connection.ExecuteAsync(@"
                UPDATE [dbo].[FinanceInformation]
                SET         [VAT] = @VAT
                           ,[CurrencyPK] = @CurrencyPK
                WHERE [FinanceInformationPK] = @FinanceInformationPK", bu.FinanceInformation);

            scope.Complete();
        }
        return bu;
    }

    public async Task<BusinessUnit> Create(BusinessUnit bu)
    {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                INSERT INTO [dbo].[FinanceInformation]
                            ([FinanceInformationPK]
                            ,[VAT]
                            ,[CurrencyPK])

                VALUES      (@FinanceInformationPK
                            ,@VAT
                            ,@CurrencyPK)", bu.FinanceInformation);

            await connection.ExecuteAsync(@"
                INSERT INTO [dbo].[BusinessUnit]
                            ([BusinessUnitPK]
                            ,[Name]
                            ,[Company]
                            ,[DivisionId]
                            ,[IsEditable]
                            ,[FinanceInformationPK]
                            ,[TimeStamp])

                    VALUES  (@BusinessUnitPK
                            ,@Name
                            ,@Company
                            ,@DivisionId
                            ,@IsEditable
                            ,@FinanceInformationPK
                            ,GETDATE())", bu);

            foreach (var c in bu.ContactInformations)
                await contactInformationRepository.Add(c);

            scope.Complete();
        }
        return bu;
    }

    public async Task Delete(BusinessUnit bu)
    {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        using (var connection = connectionFactory.CreateConnection())
        {
            foreach (var c in bu.ContactInformations)
                await contactInformationRepository.Delete(c);

            await connection.ExecuteAsync(@"
                DELETE FROM [dbo].[FinanceInformation]
                WHERE [FinanceInformationPK] = @FinanceInformationPK", bu);

            await connection.ExecuteAsync(@"
                DELETE FROM [dbo].[BusinessUnit]
                WHERE [BusinessUnitPK] = @BusinessUnitPK", bu);

            scope.Complete();
        }
    }

}