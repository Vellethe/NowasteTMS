using Dapper;
using NowasteTms.Model;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Transactions;
using WMan.Data.ConnectionFactory;
public class SupplierRepository : ISupplierRepository
{
    private readonly Dictionary<string, string> columnMapping = new Dictionary<string, string>
        {
            {"Id", "[s].[SupplierId]"},
            {"Name", "[cbu].[Name]"},
            {"Country", "[ci].[Country]"},
            {"Currency", "[cu].[ShortName]"}
        };

    private readonly IConnectionFactory connectionFactory;
    private readonly IBusinessUnitRepository businessUnitRepository;

    public SupplierRepository(IConnectionFactory connectionFactory, IBusinessUnitRepository businessUnitRepository)
    {
        this.connectionFactory = connectionFactory;
        this.businessUnitRepository = businessUnitRepository;
    }

    public async Task<List<Supplier>> GetSuppliers(bool includeInactive = false)
    {
        using var connection = connectionFactory.CreateConnection();

        var suppliers = await connection.QueryAsync<string>(
            @"SELECT
                supplierPK = s.SupplierPK,
                supplierID = s.SupplierID,
                businessUnitPK = s.businessUnitPK,
                businessUnit = JSON_QUERY((
                    SELECT
                    businessUnitPK = bu.BusinessUnitPK,
                    name = bu.name,
                    financeInformationPK = bu.financeInformationPK,
                    financeInformation = JSON_QUERY((

                        SELECT
                            financeInformationPK = fi.financeInformationPK,
                            vat = fi.VAT,
                            currencyPK = fi.currencyPK,
                            currency = JSON_QUERY((
                                SELECT
                                    currencyPK = cu.currencyPK,
                                    name = cu.Name,
                                    shortName = cu.ShortName
                                    FROM Currency cu
                                    WHERE
                                        cu.currencyPK = fi.CurrencyPK
                                        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                            ))
                        FROM FinanceInformation fi
                        WHERE
                            fi.FinanceInformationPK = bu.FinanceInformationPK
                            FOR JSON PATH, without_array_wrapper
                    )),
                    contactInformations = JSON_QUERY((
                        SELECT
                        contactInformationPK = ci.contactInformationPK,
                        businessUnitPK = ci.businessUnitPK,
                        isDefault = ci.isDefault,
                        [references] = JSON_QUERY((
                            SELECT
                                referencePK = re.ReferencePK,
                                name = re.Name,
                                phone = re.Phone,
                                email = re.Email,
                                comment = re.Comment,
                                contactInformationPK = re.ContactInformationPK
                                FROM Reference re
                                WHERE
                                    re.ContactInformationPK = ci.ContactInformationPK
                                    FOR JSON PATH
                        )),
                        transportZones = JSON_QUERY((
                            SELECT
                                transportZonePK = tz.TransportZonePK,
                                name = tz.Name,
                                description = tz.Description
                                FROM ContactInformationTransportZone citz
                                INNER JOIN TransportZone tz ON
                                    tz.TransportZonePK = citz.TransportZonePK
                                    WHERE
                                        citz.ContactInformationPK = ci.ContactInformationPK
                                        FOR JSON PATH
                        )),
                        phone = ci.phone,
                        cellularPhone = ci.cellularPhone,
                        email = ci.email,
                        fax = ci.fax,
                        preferredCommunication = ci.preferredCommunication,
                        address = ci.address,
                        zipcode = ci.zipcode,
                        city = ci.city,
                        country = ci.country,
                        externalId = ci.externalId,
                        isActive = ci.isActive,
                        description = ci.description,
                        isEditable = ci.isEditable

                        FROM ContactInformation ci
                        WHERE
                            ci.BusinessUnitPK = bu.BusinessUnitPK
                            FOR JSON PATH
                    )),
                    company = bu.Company,
                    divisionId = bu.DivisionId,
                    isEditable = bu.isEditable
                    FROM BusinessUnit bu
                    WHERE
                        bu.BusinessUnitPK = s.BusinessUnitPK
                        for JSON PATH, WITHOUT_ARRAY_WRAPPER
                )),
                isActive = s.isActive
                from Supplier s
                where
                s.[isActive] = 1 OR @includeInactive = 1
                for json path",
            new
            {
                includeInactive
            });

        var sb = new StringBuilder();
        foreach (var jsonPart in suppliers)
            sb.Append(jsonPart);

        var deserialized = JsonSerializer.Deserialize<List<Supplier>>(sb.ToString());

        if (deserialized == null) return null;

        foreach (var dser in deserialized)
        {
            dser.BusinessUnit ??= new BusinessUnit();
            dser.BusinessUnit.ContactInformations ??= new List<ContactInformation>();
            dser.BusinessUnit.FinanceInformation ??= new FinanceInformation();
        }

        return deserialized;
    }

    public async Task<Supplier> GetSupplier(Guid pk)
    {
        using var connection = connectionFactory.CreateConnection();
        var p = new DynamicParameters();
        p.Add("@pk", pk);
        p.Add("@JsData", dbType: DbType.String, direction: ParameterDirection.Output, size: -1);

        await connection.ExecuteAsync(@"
                SELECT
                    @JsData = CONVERT(nvarchar(max), (
                SELECT
                supplierPK = s.SupplierPK,
                supplierID = s.SupplierID,
                businessUnitPK = s.businessUnitPK,
                businessUnit = JSON_QUERY((
                    SELECT
                    businessUnitPK = bu.BusinessUnitPK,
                    name = bu.name,
                    financeInformationPK = bu.financeInformationPK,
                    financeInformation = JSON_QUERY((

                        SELECT
                            financeInformationPK = fi.financeInformationPK,
                            vat = fi.VAT,
                            currencyPK = fi.currencyPK,
                            currency = JSON_QUERY((
                                SELECT
                                    currencyPK = cu.currencyPK,
                                    name = cu.Name,
                                    shortName = cu.ShortName
                                    FROM Currency cu
                                    WHERE
                                        cu.currencyPK = fi.CurrencyPK
                                        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                            ))
                        FROM FinanceInformation fi
                        WHERE
                            fi.FinanceInformationPK = bu.FinanceInformationPK
                            FOR JSON PATH, without_array_wrapper
                    )),
                    contactInformations = JSON_QUERY((
                        SELECT
                        contactInformationPK = ci.contactInformationPK,
                        businessUnitPK = ci.businessUnitPK,
                        isDefault = ci.isDefault,
                        [references] = JSON_QUERY((
                            SELECT
                                referencePK = re.ReferencePK,
                                name = re.Name,
                                phone = re.Phone,
                                email = re.Email,
                                comment = re.Comment,
                                contactInformationPK = re.ContactInformationPK
                                FROM Reference re
                                WHERE
                                    re.ContactInformationPK = ci.ContactInformationPK
                                    FOR JSON PATH
                        )),
                        transportZones = JSON_QUERY((
                            SELECT
                                transportZonePK = tz.TransportZonePK,
                                name = tz.Name,
                                description = tz.Description
                                FROM ContactInformationTransportZone citz
                                INNER JOIN TransportZone tz ON
                                    tz.TransportZonePK = citz.TransportZonePK
                                    WHERE
                                        citz.ContactInformationPK = ci.ContactInformationPK
                                        FOR JSON PATH
                        )),
                        phone = ci.phone,
                        cellularPhone = ci.cellularPhone,
                        email = ci.email,
                        fax = ci.fax,
                        preferredCommunication = ci.preferredCommunication,
                        address = ci.address,
                        zipcode = ci.zipcode,
                        city = ci.city,
                        country = ci.country,
                        externalId = ci.externalId,
                        isActive = ci.isActive,
                        description = ci.description,
                        isEditable = ci.isEditable

                        FROM ContactInformation ci
                        WHERE
                            ci.BusinessUnitPK = bu.BusinessUnitPK
                            FOR JSON PATH
                    )),
                    company = bu.Company,
                    divisionId = bu.DivisionId,
                    isEditable = bu.isEditable
                    FROM BusinessUnit bu
                    WHERE
                        bu.BusinessUnitPK = s.BusinessUnitPK
                        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                )),
                isActive = s.isActive
                from Supplier s
                where
                s.[SupplierPK] = @pk
                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER))"
            , p);

        var c = p.Get<string>("@JsData");

        return c == null ? null : JsonSerializer.Deserialize<Supplier>(c);
    }

    public async Task<Supplier> GetById(string id)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var p = new DynamicParameters();
            p.Add("@id", id);
            p.Add("@JsData", dbType: DbType.String, direction: ParameterDirection.Output, size: -1);

            await connection.ExecuteAsync(@"
                SELECT
                    @JsData = CONVERT(nvarchar(max), (
                SELECT
                supplierPK = s.SupplierPK,
                supplierID = s.SupplierID,
                businessUnitPK = s.businessUnitPK,
                businessUnit = JSON_QUERY((
                    SELECT
                    businessUnitPK = bu.BusinessUnitPK,
                    name = bu.name,
                    financeInformationPK = bu.financeInformationPK,
                    financeInformation = JSON_QUERY((

                        SELECT
                            financeInformationPK = fi.financeInformationPK,
                            vat = fi.VAT,
                            currencyPK = fi.currencyPK,
                            currency = JSON_QUERY((
                                SELECT
                                    currencyPK = cu.currencyPK,
                                    name = cu.Name,
                                    shortName = cu.ShortName
                                    FROM Currency cu
                                    WHERE
                                        cu.currencyPK = fi.CurrencyPK
                                        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                            ))
                        FROM FinanceInformation fi
                        WHERE
                            fi.FinanceInformationPK = bu.FinanceInformationPK
                            FOR JSON PATH, without_array_wrapper
                    )),
                    contactInformations = JSON_QUERY((
                        SELECT
                        contactInformationPK = ci.contactInformationPK,
                        businessUnitPK = ci.businessUnitPK,
                        isDefault = ci.isDefault,
                        [references] = JSON_QUERY((
                            SELECT
                                referencePK = re.ReferencePK,
                                name = re.Name,
                                phone = re.Phone,
                                email = re.Email,
                                comment = re.Comment,
                                contactInformationPK = re.ContactInformationPK
                                FROM Reference re
                                WHERE
                                    re.ContactInformationPK = ci.ContactInformationPK
                                    FOR JSON PATH
                        )),
                        transportZones = JSON_QUERY((
                            SELECT
                                transportZonePK = tz.TransportZonePK,
                                name = tz.Name,
                                description = tz.Description
                                FROM ContactInformationTransportZone citz
                                INNER JOIN TransportZone tz ON
                                    tz.TransportZonePK = citz.TransportZonePK
                                    WHERE
                                        citz.ContactInformationPK = ci.ContactInformationPK
                                        FOR JSON PATH
                        )),
                        phone = ci.phone,
                        cellularPhone = ci.cellularPhone,
                        email = ci.email,
                        fax = ci.fax,
                        preferredCommunication = ci.preferredCommunication,
                        address = ci.address,
                        zipcode = ci.zipcode,
                        city = ci.city,
                        country = ci.country,
                        externalId = ci.externalId,
                        isActive = ci.isActive,
                        description = ci.description,
                        isEditable = ci.isEditable

                        FROM ContactInformation ci
                        WHERE
                            ci.BusinessUnitPK = bu.BusinessUnitPK
                            FOR JSON PATH
                    )),
                    company = bu.Company,
                    divisionId = bu.DivisionId,
                    isEditable = bu.isEditable
                    FROM BusinessUnit bu
                    WHERE
                        bu.BusinessUnitPK = s.BusinessUnitPK
                        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                )),
                isActive = s.isActive
                from Supplier s
                where
                s.[SupplierID] = @id
                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER))", p);

            var c = p.Get<string>("@JsData");

            return c == null ? null : JsonSerializer.Deserialize<Supplier>(c);
        }
    }

    public async Task<Supplier> UpdateSupplier(Guid id, Supplier supplier)
    {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                UPDATE [dbo].[Supplier]
                SET         [SupplierID] = @SupplierID
                            ,[IsActive] = @IsActive
                WHERE [SupplierPK] = @SupplierPK",
                supplier);

            supplier.BusinessUnit = await businessUnitRepository.Update(supplier.BusinessUnit);
            scope.Complete();
        }

        return supplier;
    }

    public async Task<Supplier> CreateSupplier(Supplier supplier)
    {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        using (var connection = connectionFactory.CreateConnection())
        {
            await businessUnitRepository.Create(supplier.BusinessUnit);

            await connection.ExecuteAsync(@"
                INSERT INTO [dbo].[Supplier]
                            ([SupplierPK]
                            ,[SupplierID]
                            ,[BusinessUnitPK]
                            ,[IsActive])

                VALUES
                            (@SupplierPK
                            ,@SupplierID
                            ,@BusinessUnitPK
                            ,@IsActive)",
                supplier);

            scope.Complete();
        }

        return supplier;
    }

    public async Task<Guid> DeleteSupplier(Guid id)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                UPDATE [dbo].[Supplier]
                SET         [isActive] = 0
                WHERE [SupplierPK] = @id",
                new { id });
        }
        return id;
    }

    public async Task<List<Supplier>> SearchSuppliers(SearchParameters parameters)
    {
        //using (var client = new HttpClient())
        //{
        //    var body = JsonConvert.SerializeObject(parameters, Formatting.None, new JsonSerializerSettings());
        //    var queryString = new StringContent(body, Encoding.UTF8, "application/json");
        //    var response = await client.PostAsync(_config.BaseUrl + UrlSearchSuppliers, queryString);
        //    response.EnsureSuccessStatusCode();
        //    var json = await response.Content.ReadAsStringAsync();

        //    return JsonConvert.DeserializeObject<Supplier[]>(json).ToList();
        //}

        using (var connection = connectionFactory.CreateConnection())
        {
            var builder = new SqlBuilder();
            var template = builder.AddTemplate(@"
                SELECT
                         s.[SupplierPK] AS Id
                        ,s.[SupplierPK]
                        ,s.[SupplierID]
                        ,s.[BusinessUnitPK]

                        ,cbu.[BusinessUnitPK] AS Id
                        ,cbu.[BusinessUnitPK]
                        ,cbu.[Name]

                        ,ci.[ContactInformationPK] AS Id
                        ,ci.[ContactInformationPK]
                        ,ci.[BusinessUnitPK]
                        ,ci.[Country]

                        ,fi.[FinanceInformationPK] AS Id
                        ,fi.[FinanceInformationPK]
                        ,fi.[CurrencyPK]

                        ,cu.[CurrencyPK] AS Id
                        ,cu.[CurrencyPK]
                        ,cu.[ShortName]

                    FROM [dbo].[Supplier] s
                    JOIN [BusinessUnit] cbu ON s.[BusinessUnitPK] = cbu.[BusinessUnitPK]
               LEFT JOIN [ContactInformation] ci ON ci.[ContactInformationPK] =
                  (SELECT TOP 1 c.[ContactInformationPK]
                           FROM [ContactInformation] c
                          WHERE c.[BusinessUnitPK] = cbu.[BusinessUnitPK]
                            AND c.[IsActive] = 1
                            AND c.[IsDefault] = 1
                            AND c.[isEditable] = 0
                            AND c.[Country] IS NOT NULL
                            AND c.[Country] <> '' )
               LEFT JOIN [FinanceInformation] fi ON cbu.[FinanceInformationPK] = fi.[FinanceInformationPK]
               LEFT JOIN [Currency] cu ON fi.[CurrencyPK] = cu.[CurrencyPK]
                   /**where**/
                   /**orderby**/
                  OFFSET @offset ROWS
              FETCH NEXT @limit ROWS ONLY",
                new
                {
                    offset = parameters.Offset,
                    limit = parameters.Limit
                }
            );

            //where clauses for placeholder /**where**/
            builder.Where("s.[SupplierID] IS NOT NULL");
            builder.Where("s.[SupplierID] <> ''");
            builder.Where("s.[isActive] = 1");

            foreach (var filter in parameters.Filters)
            {
                builder.Where($"{columnMapping[filter.Key]} LIKE '%{filter.Value}%'");
            }

            if (parameters.SortOrders != null && parameters.SortOrders.Any())
            {
                foreach (var column in parameters.SortOrders)
                {
                    builder.OrderBy(columnMapping[column.Key] + " " + (column.Value ? "ASC" : "DESC"));
                }
            }
            else // Forced default sort, OFFSET won't work otherwise (for placeholder /**orderby**/)
            {
                builder.OrderBy("[SupplierID] ASC");
            }

            var suppliers =
                await connection
                    .QueryAsync<Supplier, BusinessUnit, ContactInformation, FinanceInformation, Currency, Supplier>(
                        template.RawSql, map: (s, cbu, ci, fi, cu) =>
                        {
                            s.BusinessUnit = cbu;
                            s.BusinessUnit.FinanceInformation = fi;
                            s.BusinessUnit.FinanceInformation.Currency = cu;
                            s.BusinessUnit.ContactInformations = new List<ContactInformation>() { ci };
                            return s;
                        }, template.Parameters);

            return suppliers.ToList();
        }
    }

    public async Task<int> SearchSuppliersCount(SearchParameters parameters)
    {
        //using (var client = new HttpClient())
        //{
        //    var responseTransactions =
        //        await client.PostAsJsonAsync(_config.BaseUrl + UrlSearchSuppliersCount, parameters);
        //    responseTransactions.EnsureSuccessStatusCode();

        //    return int.Parse(await responseTransactions.Content.ReadAsStringAsync());
        //}

        using (var connection = connectionFactory.CreateConnection())
        {
            var builder = new SqlBuilder();
            var template = builder.AddTemplate(@"
              SELECT count(*)
                FROM [dbo].[Supplier] s
                JOIN [BusinessUnit] cbu ON s.[BusinessUnitPK] = cbu.[BusinessUnitPK]
           LEFT JOIN [ContactInformation] ci ON ci.[ContactInformationPK] =
              (SELECT TOP 1 c.[ContactInformationPK]
                       FROM [ContactInformation] c
                      WHERE c.[BusinessUnitPK] = cbu.[BusinessUnitPK]
                        AND c.[IsActive] = 1
                        AND c.[IsDefault] = 1
                        AND c.[isEditable] = 0
                        AND c.[Country] IS NOT NULL
                        AND c.[Country] <> '' )
           LEFT JOIN [FinanceInformation] fi ON cbu.[FinanceInformationPK] = fi.[FinanceInformationPK]
           LEFT JOIN [Currency] cu ON fi.[CurrencyPK] = cu.[CurrencyPK]
               /**where**/
                    ");

            //where clauses for placeholder /**where**/
            builder.Where("s.[SupplierID] IS NOT NULL");
            builder.Where("s.[SupplierID] <> ''");
            builder.Where("s.[isActive] = 1");

            foreach (var filter in parameters.Filters)
            {
                builder.Where($"{columnMapping[filter.Key]} LIKE '%{filter.Value}%'");
            }

            var result = await connection.QueryAsync<int>(template.RawSql, template.Parameters);

            return result.FirstOrDefault();
        }
    }
}