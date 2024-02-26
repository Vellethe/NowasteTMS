using Dapper;
using NowasteTms.Model;
using System.Text;
using System.Transactions;
using WMan.Data.ConnectionFactory;
public class ColumnMapping
{
    /// <summary>
    /// Default constructor. Initialize with your own values of row and search.
    /// </summary>
    public ColumnMapping()
    {
    }

    /// <summary>
    /// Set useLike to true if the mapping are fine with the LIKE operator.
    /// Otherwise false, which defaults to performing an exact match.
    /// </summary>
    public ColumnMapping(string row, bool useLike = true, bool isDate = false)
    {
        this.row = row;

        if (useLike)
        {
            this.search = row + " LIKE '%{0}%'";
        }
        else if (isDate)
            this.search = $"CAST({row} as Date) = '{{0}}'";
        else
        {
            this.search = row + " = {0}";
        }
    }

    public string row { get; set; }
    public string search { get; set; }
}

public class TransportOrderRepository : ITransportOrderRepository
{
    private readonly string defaultFields = @"
       [to].[TransportOrderPK]
      ,[to].[TransportOrderID]
      ,[to].[Price]
      ,[to].[CurrencyPK]
      ,[to].[CollectionDate]
      ,[to].[DeliveryDate]
      ,[to].[OriginalDeliveryDate]
      ,[to].[VehicleRegistrationPlate]
      ,[to].[Comment]
      ,[to].[InternalComment]
      ,[to].[CreatedByUserPK]
      ,[to].[CreatedOn]
      ,[to].[FilenameRevision]
      ,[to].[DivisionId]
      ,[to].[Orderstatus]
      ,[to].[Email]
      ,[to].[TransportType]
      ,[to].[OrderIDs]
      ,[to].[ConsolidatedParentPK]
      ,[to].[IsConsolidation]
      ,[to].[Updated]";

    private readonly string searchFields = @"
       [to].[TransportOrderPK]
      ,[to].[TransportOrderID]
      ,[to].[Price]
      ,[to].[CurrencyPK]
      ,[to].[CollectionDate]
      ,[to].[DeliveryDate]
      ,[to].[OriginalDeliveryDate]
      ,[to].[VehicleRegistrationPlate]
      ,[to].[Comment]
      ,[to].[InternalComment]
      ,[to].[CreatedByUserPK]
      ,[to].[CreatedOn]
      ,[to].[FilenameRevision]
      ,[to].[DivisionId]
      ,[to].[Orderstatus]
      ,[to].[Email]
      ,[to].[TransportType]
      ,[to].[OrderIDs]
      ,[to].[ConsolidatedParentPK]
      ,[to].[IsConsolidation]
      ,[to].[Updated]";

    private readonly IConnectionFactory connectionFactory;
    private readonly ITransportOrderLineRepository transportOrderLineRepository;

    // Match sorting and filtering keywords to column names.
    private readonly Dictionary<string, ColumnMapping> columnMapping = new Dictionary<string, ColumnMapping>
        {
            {"TransportOrderID", new ColumnMapping() {row="[to].[TransportOrderID]", search="[to].[TransportOrderID] LIKE '{0}%'"} },
            {"OrderStatus", new ColumnMapping() {row="[to].[Orderstatus]", search="[to].[Orderstatus]={0}"}},
            {"AgentName", new ColumnMapping("[abu].[Name]")},
            {"TransportOrderLineCount", new ColumnMapping("(SELECT COUNT(*) FROM [TransportOrderLine] WHERE [TransportOrderPK] = [to].[TransportOrderPK])")},
            {"Price", new ColumnMapping("[to].[Price]")},
            {"Currency", new ColumnMapping("[cu].[ShortName]")},
            {"FromCountry", new ColumnMapping("[fci].[Country]")},
            {"FromCity", new ColumnMapping("[fci].[City]")},
            {"ToCountry", new ColumnMapping("[tci].[Country]")},
            {"ToCity", new ColumnMapping("[tci].[City]")},
            {"VehicleRegistrationPlate", new ColumnMapping("[to].[VehicleRegistrationPlate]")},
            {"CollectionDate", new ColumnMapping("[to].[CollectionDate]", false, true)},
            {"DeliveryDate", new ColumnMapping("[to].[DeliveryDate]", false, true)},
            {"Email", new ColumnMapping("[to].[Email]")},
            {"ToCustomerName", new ColumnMapping("[tol].[ToCustomerName]")},
            {"FromSupplierName", new ColumnMapping("[tol].[FromSupplierName]")},
            {"OrderIds", new ColumnMapping("[to].[OrderIDs]")},
            {"PalletQty", new ColumnMapping("[pallets].[Sea]")},
            {"EurPalletQty", new ColumnMapping("[pallets].[Eur]")},
            {"InternalComment", new ColumnMapping("[to].[InternalComment]") },
            {"Updated", new ColumnMapping("[to].[Updated]", false, true)},
            {"UpdateWeekDay", new ColumnMapping("DATEPART(WEEKDAY, [to].[Updated])")},
            {"EtaWeekDay", new ColumnMapping("DATEPART(WEEKDAY, [to].[DeliveryDate])")},
            {"CollectionDateWeekDay", new ColumnMapping("DATEPART(WEEKDAY, [to].[CollectionDate])")},
            {"Created", new ColumnMapping("[to].[CreatedOn]", useLike:false, isDate:true)},
            {"TOCurrencyShortName", new ColumnMapping("[cu].[ShortName]", useLike:true, isDate:false)},
        };

    private readonly string strSearchTransportOrders = @"
SELECT /**select**/
  FROM 
	[TransportOrder] [to]
	LEFT OUTER JOIN [OrderTransportOrder] [oto] ON [oto].TransportOrderPK = [to].TransportOrderPK
	LEFT OUTER JOIN [Order] [o] ON [o].OrderPK = [oto].OrderPK
	INNER JOIN [Currency] cu ON [to].[CurrencyPK] = [cu].[CurrencyPK]
	LEFT OUTER JOIN 
	(
		SELECT 
			[TransportOrderPK], [ToContactInformationPK], 
			[FL] = DENSE_RANK() OVER (PARTITION BY [TransportOrderPK] ORDER BY [LineNumber]),
			[AgentPK],
			[FromContactInformationPK],
			[FromSupplierName],
			[ToCustomerName]
		FROM [TransportOrderLine] WITH (READUNCOMMITTED)
	) [tol] ON [tol].[TransportOrderPK] = [to].[TransportOrderPK] AND [tol].[FL] = 1 
	LEFT OUTER JOIN 
	(
		SELECT 
			[ConsolidatedParentPK] AS [TransportOrderPK], 
			MAX([ToContactInformationPK]) AS [ToContactInformationPK],
			MAX([AgentPK]) AS [AgentPK],
			MAX([FromContactInformationPK]) AS [FromContactInformationPK]
		FROM 
			[TransportOrderLine] l WITH (READUNCOMMITTED)
			INNER JOIN [TransportOrder] t WITH (READUNCOMMITTED) ON t.[TransportOrderPK] = l.[TransportOrderPK]
		GROUP BY t.[ConsolidatedParentPK]
	) [ctol] ON [ctol].[TransportOrderPK] = [to].[TransportOrderPK]
	LEFT OUTER JOIN [Agent] a ON ISNULL(ctol.[AgentPK], [tol].[AgentPK]) = a.[AgentPK]
	LEFT OUTER JOIN [BusinessUnit] [abu] ON [a].[BusinessUnitPK] = [abu].[BusinessUnitPK]
	LEFT OUTER JOIN [ContactInformation] [fci] ON ISNULL(ctol.[FromContactInformationPK], [tol].[FromContactInformationPK]) = [fci].[ContactInformationPK]
	LEFT OUTER JOIN [ContactInformation] [tci] ON ISNULL(ctol.[ToContactInformationPK], [tol].[ToContactInformationPK]) = [tci].[ContactInformationPK]
	LEFT OUTER JOIN pallets ON pallets.[TransportOrderPK] = [to].[TransportOrderPK]
	LEFT OUTER JOIN ConsolidatedPallets ON ConsolidatedPallets.[TransportOrderPK] = [to].[TransportOrderPK]
/**where**/
";

    private readonly string strCountSearchTransportOrders = @"
SELECT COUNT(*)
  FROM [dbo].[TransportOrder] [to]
LEFT JOIN [dbo].[OrderTransportOrder] [oto] ON [oto].TransportOrderPK = [to].TransportOrderPK
LEFT JOIN [dbo].[Order] [o] ON [o].OrderPK = [oto].OrderPK
JOIN [Currency] cu ON [to].[CurrencyPK] = [cu].[CurrencyPK]
LEFT JOIN (
     SELECT 
		*,
		[FL] = DENSE_RANK() OVER (PARTITION BY [TransportOrderPK] ORDER BY [LineNumber])
        FROM [TransportOrderLine] WITH (READUNCOMMITTED)
) [tol] ON
	[tol].[TransportOrderPK] = [to].[TransportOrderPK] AND
	[tol].[FL] = 1
LEFT JOIN [Agent] a ON [tol].[AgentPK] = a.[AgentPK]
LEFT JOIN [BusinessUnit] [abu] ON [a].[BusinessUnitPK] = [abu].[BusinessUnitPK]
LEFT JOIN [ContactInformation] [fci] ON [tol].[FromContactInformationPK] = [fci].[ContactInformationPK]
LEFT JOIN [ContactInformation] [tci] ON [tol].[ToContactInformationPK] = [tci].[ContactInformationPK]
LEFT JOIN (
	SELECT OrderPK, [" + ((int)PalletTypes.SeaPallet) + @"] AS Sea, [" + ((int)PalletTypes.EURBPallet) + @"] AS Eur
	FROM
    (SELECT [ol].[OrderPK], ol.PalletTypeId, SUM(ol.palletQty) AS PalletQty
	FROM [OrderLine] ol
	GROUP BY ol.PalletTypeId, [ol].[OrderPK] ) p
	PIVOT (
	SUM(palletQty)
	FOR PalletTypeId IN ([" + ((int)PalletTypes.EURBPallet) + @"],[" + ((int)PalletTypes.SeaPallet) + @"])) AS pvt
) pallets ON
	pallets.[OrderPK] = [o].[OrderPK]
/**where**/
";

    private readonly string strPalletsAndConsolidatedPallets = @"
WITH Pallets AS 
(
	SELECT [TransportOrderPK], [8] AS Sea, [2] AS Eur
	FROM
		(
			SELECT [to].[TransportOrderPK], ol.PalletTypeId, SUM(ol.palletQty) AS PalletQty
			FROM 
				[OrderLine] ol
				LEFT OUTER JOIN [Order] [o] ON [o].OrderPK = ol.[OrderPK]
				LEFT OUTER JOIN [OrderTransportOrder] [oto] ON [o].OrderPK = [oto].OrderPK
				LEFT OUTER JOIN [TransportOrder] [to] ON  [oto].TransportOrderPK = [to].TransportOrderPK
			GROUP BY ol.PalletTypeId, [to].[TransportOrderPK] 
		) p
		PIVOT 
		(
			SUM(palletQty)
			FOR PalletTypeId IN ([2],[8])
		) AS pvt
),
ConsolidatedPallets AS 
(
	SELECT [TransportOrderPK], [8] AS Sea, [2] AS Eur
	FROM
		(
			SELECT [to].[ConsolidatedParentPK] AS [TransportOrderPK], ol.PalletTypeId, SUM(ol.palletQty) AS PalletQty
			FROM 
				[OrderLine] ol
				LEFT OUTER JOIN [Order] [o] ON [o].OrderPK = ol.[OrderPK]
				LEFT OUTER JOIN [OrderTransportOrder] [oto] ON [o].OrderPK = [oto].OrderPK
				LEFT OUTER JOIN [TransportOrder] [to] ON  [oto].TransportOrderPK = [to].TransportOrderPK
			WHERE [to].[ConsolidatedParentPK] IS NOT NULL
			GROUP BY ol.PalletTypeId, [to].[ConsolidatedParentPK]
		) p
		PIVOT 
		(
			SUM(palletQty)
			FOR PalletTypeId IN ([2],[8])
		) AS pvt
)
";

    public TransportOrderRepository(IConnectionFactory connectionFactory, ITransportOrderLineRepository transportOrderLineRepository)
    {
        this.connectionFactory = connectionFactory;
        this.transportOrderLineRepository = transportOrderLineRepository;
    }

    public async Task<TransportOrder> Get(Guid pk)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var transportOrder = connection.Query<TransportOrder, Currency, TransportOrder>(@"SELECT " + defaultFields + @"
                      ,[cu].[CurrencyPK] AS Id
                      ,[cu].[CurrencyPK]
                      ,[cu].[Name]
                      ,[cu].[ShortName]
                FROM [dbo].[TransportOrder] [to]
                JOIN [Currency] cu ON [to].[CurrencyPK] = cu.[CurrencyPK]
                WHERE [to].[TransportOrderPK] = @pk
                            ",
                (to, cu) =>
                {
                    to.Currency = cu;

                    return to;
                },
                new
                {
                    pk
                }).FirstOrDefault();

            var result = await HandleTransportOrder(new List<TransportOrder> { transportOrder });

            return result.FirstOrDefault();
        }
    }

    public async Task<List<TransportOrder>> GetConsolidatedTransportOrders(Guid consolidatedTransportOrderGuid)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var transportOrderPKs = await connection.QueryAsync<TransportOrder>(
                @"SELECT [to].[TransportOrderPK] 
                    FROM [dbo].[TransportOrder] [to]

                    WHERE [to].[ConsolidatedParentPK] = @consolidatedTransportOrderGuid
                                ",

                new
                {
                    consolidatedTransportOrderGuid
                });

            if (transportOrderPKs.ToList().Count <= 0)
                return new List<TransportOrder>();

            var transportOrders = await GetListTransportOrders(transportOrderPKs.Select(x => x.TransportOrderPK).ToList());

            return transportOrders;
        }
    }

    public async Task<Guid> Add(TransportOrder transportOrder)
    {
        if (transportOrder.TransportOrderPK == Guid.Empty)
            transportOrder.TransportOrderPK = Guid.NewGuid();


        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                INSERT INTO [dbo].[TransportOrder]
                           ([TransportOrderPK]
                           ,[TransportOrderID]
                           ,[Price]
                           ,[CurrencyPK]
                           ,[CollectionDate]
                           ,[DeliveryDate]
                           ,[OriginalDeliveryDate]
                           ,[VehicleRegistrationPlate]
                           ,[CreatedOn]
                           ,[Comment]
                           ,[InternalComment]
                           ,[CreatedByUserPK]
                           ,[Updated]
                           ,[DivisionId]
                           ,[Orderstatus]
                           ,[Email]
                           ,[TransportType]
                           ,[OrderIDs]
                           ,[IsConsolidation])
                     VALUES
                           (@TransportOrderPK
                           ,@TransportOrderID
                           ,@Price
                           ,@CurrencyPK
                           ,@CollectionDate
                           ,@DeliveryDate
                           ,@DeliveryDate
                           ,@VehicleRegistrationPlate
                           ,GETDATE()
                           ,@Comment
                           ,@InternalComment
                           ,@CreatedByUserPK
                           ,GETDATE()
                           ,@DivisionId
                           ,@Orderstatus
                           ,@Email
                           ,@TransportType
                           ,@OrderIds
                           ,@IsConsolidation)",
                                transportOrder);

            //
            // Create new links between Orders and TransportOrder.
            //
            if (transportOrder.OrderTransportOrders != null)
            {
                foreach (var link in transportOrder.OrderTransportOrders)
                {
                    var orderLink = new OrderTransportOrder
                    {
                        OrderPK = link.OrderPK,
                        TransportOrderPK = transportOrder.TransportOrderPK,
                    };
                    await connection.ExecuteAsync(@"
                        INSERT INTO [dbo].[OrderTransportOrder]
                                   ([OrderPK]
                                   ,[TransportOrderPK])
                             VALUES
                                   (@OrderPK
                                   ,@TransportOrderPK
                                    )",
                        orderLink);
                }
            }

            //
            // Add order lines
            //
            if (transportOrder.TransportOrderLines != null)
            {
                foreach (var line in transportOrder.TransportOrderLines)
                {
                    await transportOrderLineRepository.Add(connection, line);
                }
            }

            scope.Complete();
        }

        return transportOrder.TransportOrderPK;
    }

    public async Task<int> SetRejected(Guid pk)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            return await connection.ExecuteAsync(@"
                UPDATE [dbo].[TransportOrder]
                   SET [OrderStatus] = @changed
                  WHERE [TransportOrderPK] = @pk",
                new
                {
                    pk,
                    changed = OrderTransportStatus.Changed
                });
        }
    }

    public async Task<int> SetConfirmed(Guid pk, DateTime eta, string regNr)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            return await connection.ExecuteAsync(@"
                UPDATE [dbo].[TransportOrder]
                   SET [Orderstatus] = @confirmed,
                       [DeliveryDate] = @eta,
                       [VehicleRegistrationPlate] = @regNr
                  WHERE [TransportOrderPK] = @pk",
                new
                {
                    pk,
                    eta,
                    regNr,
                    confirmed = OrderTransportStatus.Confirmed
                });
        }
    }

    public async Task<int> SetArrival(Guid pk, DateTime arrival)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            return await connection.ExecuteAsync(@"
                UPDATE [dbo].[TransportOrder]
                   SET [ArrivalDate] = @arrival
                  WHERE [TransportOrderPK] = @pk",
                 new
                 {
                     pk,
                     arrival
                 });
        }
    }

    public async Task Update(TransportOrder transportOrder)
    {
        transportOrder.Updated = DateTime.Now;

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                UPDATE [dbo].[TransportOrder]
                   SET [TransportOrderID] = @TransportOrderID
                      ,[Price] = @Price
                      ,[CurrencyPK] = @CurrencyPK
                      ,[CollectionDate] = @CollectionDate
                      ,[DeliveryDate] = @DeliveryDate
                      ,[VehicleRegistrationPlate] = @VehicleRegistrationPlate
                      ,[Comment] = @Comment
                      ,[InternalComment] = @InternalComment
                      ,[Orderstatus] = @Orderstatus
                      ,[Updated] = @Updated
                      ,[Email] = @Email
                      ,[TransportType] = @TransportType
                 WHERE [TransportOrderPK] = @TransportOrderPK",
                                transportOrder);

            await transportOrderLineRepository.RemoveAll(connection, transportOrder.TransportOrderPK);

            foreach (var line in transportOrder.TransportOrderLines)
                await transportOrderLineRepository.Add(connection, line);

            scope.Complete();
        }
    }

    public async Task<int> UpdateHeader(TransportOrder transportOrder)
    {
        transportOrder.Updated = DateTime.Now;
        using (var connection = connectionFactory.CreateConnection())
        {
            return await connection.ExecuteAsync(@"
                UPDATE [dbo].[TransportOrder]
                   SET [TransportOrderID] = @TransportOrderID
                      ,[Price] = @Price
                      ,[CurrencyPK] = @CurrencyPK
                      ,[CollectionDate] = @CollectionDate
                      ,[DeliveryDate] = @DeliveryDate
                      ,[VehicleRegistrationPlate] = @VehicleRegistrationPlate
                      ,[Comment] = @Comment
                      ,[InternalComment] = @InternalComment
                      ,[Updated] = @Updated
                      ,[Email] = @Email
                      ,[TransportType] = @TransportType
                      ,[Orderstatus] = @Orderstatus
                 WHERE [TransportOrderPK] = @TransportOrderPK ",
                                transportOrder);
        }
    }

    public async Task<int> IncreaseFilenameRevision(Guid id)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            return await connection.ExecuteAsync(@"
                UPDATE [dbo].[TransportOrder]
                   SET [FilenameRevision] = FilenameRevision + 1
                   WHERE [TransportOrderPK] = @id",
                 new
                 {
                     id
                 }
             );
        }
    }

    public async Task<SearchTransportOrderResponse> SearchOrders(SearchParameters parameters, Guid? agentPK, bool historical)
    {
        if (historical)
            parameters.Filters.Add("historical", "true");

        var response = new SearchTransportOrderResponse();

        var builder = new SqlBuilder();
        using (var connection = connectionFactory.CreateConnection())
        {
            var searchParameterHandle = HandleSearchParameters(parameters, agentPK, builder);

            var transportOrders = await connection.QueryAsync<TransportOrder, Currency, Agent, BusinessUnit, ContactInformation, ContactInformation, Order, TransportOrder>(searchParameterHandle.template.RawSql, (to, cu, a, abu, fci, tci, o) =>
            {
                to.Currency = cu;
                a.BusinessUnit = abu;
                to.TransportOrderLines = new List<TransportOrderLine>{
                        new TransportOrderLine
                        {
                            Agent = a,
                            FromContactInformation = fci,
                            ToContactInformation = tci
                        }
                    };

                to.Order = o;

                return to;
            }, searchParameterHandle.template.Parameters);

            IEnumerable<TransportOrderLine> transportOrderLines;

            if (transportOrders.ToList().Count > 2000)
            {
                transportOrderLines = await transportOrderLineRepository.GetAll();
            }
            else
            {
                transportOrderLines = await transportOrderLineRepository.GetLinesForTransportOrders(transportOrders.Select(to => to.TransportOrderPK));
            }
            var consolidated = transportOrders.ToList().Any(x => x.IsConsolidation);
            var orderRelations = await GetOrderRelations(transportOrders.Select(c => c.TransportOrderPK));

            foreach (var transportOrder in transportOrders)
            {
                transportOrder.TransportOrderLines = transportOrderLines.Where(tol => tol.TransportOrderPK == transportOrder.TransportOrderPK).OrderBy(tol => tol.LineNumber);
                transportOrder.OrderTransportOrders = orderRelations.Where(c => c.TransportOrderPK == transportOrder.TransportOrderPK);

                // Populate fields for consolidated, sum up various fields from children transport orders.
                if (transportOrder.IsConsolidation)
                {
                    await PopulateConsolidatedTransportOrder(transportOrder);
                }
            }

            if (consolidated)
            {
                var ongoing = transportOrders.OrderBy(to => to.TransportOrderID);

                foreach (var column in parameters.SortOrders)
                {
                    string key = column.Key;
                    bool value = column.Value;

                    // NOTE: Currently only FromCountry and ToCountry are adapted for re-sorting!
                    if (key == "FromCountry")
                    {
                        if (value)
                        {
                            ongoing = ongoing.ThenBy(to => to.TransportOrderLines.Where(tol => tol.FromContactInformation.Country != null).FirstOrDefault()?.FromContactInformation.Country == null
                                ? ""
                                : to.TransportOrderLines.FirstOrDefault()?.FromContactInformation.Country);
                        }
                        else
                        {
                            ongoing = ongoing.ThenByDescending(to => to.TransportOrderLines.Where(tol => tol.FromContactInformation.Country != null).FirstOrDefault()?.FromContactInformation.Country == null
                                ? ""
                                : to.TransportOrderLines.FirstOrDefault()?.FromContactInformation.Country);
                        }
                    }
                    if (key == "ToCountry")
                    {
                        if (value)
                        {
                            ongoing = ongoing.ThenBy(to => to.TransportOrderLines.Where(tol => tol.ToContactInformation.Country != null).FirstOrDefault()?.ToContactInformation.Country == null
                                ? ""
                                : to.TransportOrderLines.FirstOrDefault()?.ToContactInformation.Country);
                        }
                        else
                        {
                            ongoing = ongoing.ThenByDescending(to => to.TransportOrderLines.Where(tol => tol.ToContactInformation.Country != null).FirstOrDefault()?.ToContactInformation.Country == null
                                ? ""
                                : to.TransportOrderLines.FirstOrDefault()?.ToContactInformation.Country);
                        }
                    }
                    if (key == "FromCity")
                    {
                        if (value)
                        {
                            ongoing = ongoing.ThenBy(to => to.TransportOrderLines.Where(tol => tol.FromContactInformation.City != null).FirstOrDefault()?.FromContactInformation.City == null
                                ? ""
                                : to.TransportOrderLines.FirstOrDefault()?.FromContactInformation.City);
                        }
                        else
                        {
                            ongoing = ongoing.ThenByDescending(to => to.TransportOrderLines.Where(tol => tol.FromContactInformation.City != null).FirstOrDefault()?.FromContactInformation.City == null
                                ? ""
                                : to.TransportOrderLines.FirstOrDefault()?.FromContactInformation.City);
                        }
                    }
                }

                transportOrders = ongoing;
            }

            response.TransportOrders = transportOrders;
            response.SearchCount = await SearchTransportOrdersCount(builder);

            return response;
        }
    }

    public async Task SetParentTransportOrder(Guid consolidatedTransportOrderPk, IEnumerable<Guid> transportOrderPKs)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            foreach (var childPk in transportOrderPKs)
            {
                await connection.ExecuteAsync(@"
                    UPDATE [dbo].[TransportOrder]
                       SET [ConsolidatedParentPK] = @consolidatedTransportOrderPk
                      WHERE [TransportOrderPK] = @childPk",
                    new
                    {
                        consolidatedTransportOrderPk,
                        childPk
                    });
            }
        }
    }

    public async Task Delete(Guid transportOrderPk)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                UPDATE [TransportOrder]
                SET [ConsolidatedParentPK] = NULL
                WHERE [ConsolidatedParentPK] = @transportOrderPk",
                new
                {
                    transportOrderPk
                });

            await connection.ExecuteAsync(@"
                DELETE FROM [TransportOrder]
                WHERE [TransportOrderPK] = @transportOrderPk",
                new
                {
                    transportOrderPk
                });
        }
    }

    public async Task<TransportOrder> GetWholeById(string transportOrderId)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var transportOrderPK = await connection.QueryAsync<Guid>(@"
                    SELECT [to].[TransportOrderPK]
                    FROM [dbo].[TransportOrder] [to]
                    WHERE [to].[TransportOrderID] = @transportOrderId
                    ", new { transportOrderId });

            return await Get(transportOrderPK.First());
        }
    }

    public async Task<List<TransportOrder>> ExcelExportTransportOrder(SearchParameters parameters, Guid? agentPK)
    {
        if (agentPK != null)
        {
            parameters.Filters.Add("businessUnitId", agentPK.ToString());
        }

        try
        {
            SqlBuilder.Template template;

            if (!parameters.Filters.ContainsKey("businessUnitId"))
            {
                template = HandleSearchParameters(parameters, null).template;
            }
            else
            {
                template = HandleSearchParameters(parameters, Guid.Parse(parameters.Filters["businessUnitId"])).template;
            }

            using var connection = connectionFactory.CreateConnection();
            var transportOrderLines = (await transportOrderLineRepository.GetAll())
                .GroupBy(p => p.TransportOrderPK)
                .ToDictionary(g => g.Key, g => g.ToList());

            Dictionary<Guid, List<TransportOrderLine>> consolidatedTransportOrderLines = new();

            var transportOrders =
                await connection
                .QueryAsync<TransportOrder, Currency, Agent, BusinessUnit, ContactInformation, ContactInformation, Order, TransportOrder>(template.RawSql, (to, cu, a, abu, fci, tci, o) =>
                {
                    to.Currency = cu;

                    transportOrderLines.TryGetValue(to.TransportOrderPK, out var transportOrderLine);

                    to.TransportOrderLines = new List<TransportOrderLine>
                    {
                            transportOrderLine?.FirstOrDefault()
                    };

                    if (to.IsConsolidation == false)
                    {
                        if (to.ConsolidatedParentPK is not null)
                        {
                            var pk = to.ConsolidatedParentPK.GetValueOrDefault();
                            if (!consolidatedTransportOrderLines.ContainsKey(pk))
                            {
                                consolidatedTransportOrderLines.Add(pk, new List<TransportOrderLine>(to.TransportOrderLines));
                            }
                            else
                            {
                                consolidatedTransportOrderLines[pk].AddRange(to.TransportOrderLines);
                            }
                        }
                    }

                    to.Order = o;

                    return to;

                }, template.Parameters);

            foreach (var to in transportOrders)
            {
                if (to.IsConsolidation)
                {
                    // Can't use distinctby as it would result in data loss when printing excel values
                    consolidatedTransportOrderLines.TryGetValue(to.TransportOrderPK, out List<TransportOrderLine> lines);
                    to.TransportOrderLines = lines;
                }
            }

            return transportOrders.Where(p => !String.IsNullOrWhiteSpace(p.OrderIds)).OrderBy(p => p.TransportOrderID).ToList();
        }
        catch (Exception e)
        {
            throw new Exception("Failed with excel export for transport orders: " + e.Message);
        }
    }

    public async Task<IEnumerable<TransportOrder>> GetForOrder(Guid PK)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var transportOrders = await connection.QueryAsync<TransportOrder, Currency, TransportOrder>(@"SELECT " + defaultFields + @"
                          ,[cu].[CurrencyPK] AS Id
                          ,[cu].[CurrencyPK]
                          ,[cu].[Name]
                          ,[cu].[ShortName]
                    FROM [dbo].[TransportOrder] [to]
                    JOIN [Currency] cu ON [to].[CurrencyPK] = cu.[CurrencyPK]
                        WHERE [TransportOrderPK] IN
                            (SELECT TransportOrderPK
                                FROM [dbo].[OrderTransportOrder]
                                WHERE OrderPK = @PK)",
                (to, cu) =>
                {
                    to.Currency = cu;

                    return to;
                },
                new
                {
                    PK
                });

            var orderIds = await connection.QueryAsync<OrderTransportOrder>(@"
                        SELECT [OrderPK]
                            ,[TransportOrderPK]
                        FROM [dbo].[OrderTransportOrder]
                        WHERE [TransportOrderPK] IN @PKs",
                new
                {
                    PKs = transportOrders.Select(x => x.TransportOrderPK)
                });

            var transportOrderLines = await transportOrderLineRepository.GetLinesForTransportOrders(transportOrders.Select(to => to.TransportOrderPK));

            foreach (var transportOrder in transportOrders)
            {
                transportOrder.OrderTransportOrders = orderIds.Where(x => x.TransportOrderPK == transportOrder.TransportOrderPK).ToList();
                transportOrder.TransportOrderLines = transportOrderLines.Where(tol => tol.TransportOrderPK == transportOrder.TransportOrderPK);
            }

            return transportOrders;
        }
    }

    private (SqlBuilder.Template template, bool historical) HandleSearchParameters(SearchParameters parameters, Guid? businessUnitId, SqlBuilder builder = null)
    {
        if (builder == null)
            builder = new SqlBuilder();

        bool historical = false;
        if (parameters.Filters.Keys.Contains("historical"))
        {
            if (parameters.Filters["historical"] == "true")
            {
                historical = true;
            }

            parameters.Filters.Remove("historical");
        }

        string searchTransportOrders = strPalletsAndConsolidatedPallets + strSearchTransportOrders;
        //sorting becomes incredibly slow at big results when the columned ordered by is no indexed for the given query
        //todo fix for also the pagination queries. as there is a 400+ mb memory grant for a 30mb query
        if (parameters.Limit < int.MaxValue)
        {
            searchTransportOrders += @"
                /**orderby**/
                OFFSET @offset ROWS
                FETCH NEXT @limit ROWS ONLY";
        }

        var template = builder.AddTemplate(searchTransportOrders,
            new
            {
                offset = parameters.Offset,
                limit = parameters.Limit,
                businessUnitPK = businessUnitId,
            });

        //,[abu].[BusinessUnitPK] AS Id
        //,[abu].[Name]
        builder.Select(defaultFields + @"
                      ,ISNULL(ConsolidatedPallets.Sea, [pallets].[Sea]) AS PalletQty
                      ,ISNULL(ConsolidatedPallets.Eur, [pallets].[Eur]) AS EurPalletQty

	                  ,[cu].[CurrencyPK] AS Id
	                  ,[cu].[CurrencyPK]
	                  ,[cu].[Name]
	                  ,[cu].[ShortName]

	                  ,[a].[AgentPK] AS Id

	                  ,[abu].[BusinessUnitPK] AS Id
                      ,[cu].[CurrencyPK]
	                  ,[cu].[Name]
                      ,[cu].[ShortName]

	                  ,[fci].[ContactInformationPK] AS Id
	                  ,[fci].[City]
	                  ,[fci].[Country]

	                  ,[tci].[ContactInformationPK] AS Id
	                  ,[tci].[City]
	                  ,[tci].[Country]

	                  ,[o].OrderID AS Id
	                  ,[o].OrderID
	                  ,[o].[Status]
                    ");

        if (historical == false)
        {
            // builder.Where("([to].[OrderStatus] > 3)");
            builder.Where("[to].[Orderstatus] < 7");
        }
        else
        {
            //builder.Where("([to].[OrderStatus] <= 3)");
            builder.Where("[to].[Orderstatus] >= 7");
        }

        if (businessUnitId != null)
        {
            builder.Where("a.[BusinessUnitPK] = @businessUnitPK");
        }

        var dateTimeFiltersWithNoColumn = GetFiltersWithNoColumn();

        if (parameters.Filters.Any(c => dateTimeFiltersWithNoColumn.Contains(c.Key)))
        {
            HandleNonColumnDataTimeFilter(parameters, dateTimeFiltersWithNoColumn, builder);

        }
        foreach (var filter in parameters.Filters)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(columnMapping[filter.Key].search, filter.Value);
            builder.Where(sb.ToString());
        }

        if (parameters.SortOrders != null && parameters.SortOrders.Any())
        {
            foreach (var column in parameters.SortOrders)
            {
                if (column.Key == "FromCity")
                {
                    // Dapper adds up OrderBy() like this. (unlike IEnumerable where you must use ThenBy() for the second orderby.)
                    builder.OrderBy("ISNULL([fci].[City], '')" + " " + (column.Value ? "ASC" : "DESC"));
                    builder.OrderBy("ISNULL([tci].[City], '')" + " " + (column.Value ? "ASC" : "DESC"));
                }
                else if (column.Key == "ToCity")
                {
                    builder.OrderBy("ISNULL([tci].[City], '')" + " " + (column.Value ? "ASC" : "DESC"));
                    builder.OrderBy("ISNULL([fci].[City], '')" + " " + (column.Value ? "ASC" : "DESC"));
                }
                else
                {
                    builder.OrderBy(columnMapping[column.Key].row + " " + (column.Value ? "ASC" : "DESC"));
                }
            }
        }
        else // Forced default sort, OFFSET won't work otherwise
        {
            builder.OrderBy("[TransportOrderID] ASC");
        }

        return (template, historical);
    }

    private async Task<List<TransportOrder>> HandleTransportOrder(List<TransportOrder> transportOrders)
    {
        var pk = transportOrders.Select(c => c.TransportOrderPK);
        var orderRelations = await GetOrderRelations(pk);
        var orderLines = await transportOrderLineRepository.GetLinesForTransportOrders(pk);
        foreach (TransportOrder transportOrder in transportOrders)
        {
            if (transportOrder == null)
            {
                return new List<TransportOrder>();
            }

            transportOrder.TransportOrderLines = orderLines.Where(c => c.TransportOrderPK == transportOrder.TransportOrderPK);

            transportOrder.OrderTransportOrders = orderRelations.Where(c => c.TransportOrderPK == transportOrder.TransportOrderPK);

            if (transportOrder.IsConsolidation)
            {
                // Fetch orders for the children transport-orders.
                //TODO -- IF SLOW REMOVE FROM FOREACH
                await PopulateConsolidatedTransportOrder(transportOrder);
            }
        }

        return transportOrders;
    }

    private async Task PopulateConsolidatedTransportOrder(TransportOrder consolidatedTransportOrder)
    {
        //await GetPalletQtyForConsolidatedTransportOrder(consolidatedTransportOrder);

        var transportOrders = await GetConsolidatedTransportOrders(consolidatedTransportOrder.TransportOrderPK);

        if (transportOrders.ToList().Count == 0)
        {
            return;
        }

        using (var connection = connectionFactory.CreateConnection())
        {
            var transportOrderLines = await transportOrderLineRepository.GetLinesForTransportOrders(transportOrders.Select(to => to.TransportOrderPK));

            // re-number them..
            int LineNumber = 1;
            foreach (var tol in transportOrderLines)
            {
                tol.LineNumber = LineNumber;
                LineNumber++;
            }

            var orderRelations = await GetOrderRelations(transportOrders.Select(c => c.TransportOrderPK));
            foreach (var transportOrder in transportOrders)
            {
                transportOrder.OrderTransportOrders = orderRelations.Where(c => c.TransportOrderPK == transportOrder.TransportOrderPK);
            }

            consolidatedTransportOrder.TransportOrderLines = transportOrderLines;
            consolidatedTransportOrder.OrderTransportOrders = transportOrders.SelectMany(x => x.OrderTransportOrders);
        }

        if (consolidatedTransportOrder.OrderTransportOrders.Count() > 0)
        {
            // Consolidated tos don't have an order status of its own, so copy.
            consolidatedTransportOrder.Order = new Order();
            consolidatedTransportOrder.Order.Status = consolidatedTransportOrder.OrderTransportOrders.First().Order.Status;
        }
    }

    private async Task GetPalletQtyForConsolidatedTransportOrder(TransportOrder consolidatedTransportOrder)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            string query = $@"
                SELECT ol.PalletQty, ol.PalletTypeId
                FROM TransportOrder as tro

                INNER JOIN OrderTransportOrder as oto ON oto.TransportOrderPK=tro.TransportOrderPK
                INNER JOIN [Order] as o ON o.OrderPK=oto.OrderPK
                INNER JOIN OrderLine as ol ON ol.OrderPK=o.OrderPK

                WHERE tro.ConsolidatedParentPK='{consolidatedTransportOrder.TransportOrderPK}'";

            var palletResult = await connection.QueryAsync<OrderLine>(query);

            foreach (var item in palletResult)
            {
                switch (item.PalletTypeId)
                {
                    case 2: // ID for EuroPallets is 2.
                        consolidatedTransportOrder.EurPalletQty += item.PalletQty;
                        break;

                    case 8: // ID for Sea Pallets is 8.
                        consolidatedTransportOrder.PalletQty += item.PalletQty;
                        break;
                }
            }
        }
    }

    private async Task<IEnumerable<OrderTransportOrder>> GetOrderRelations(IEnumerable<Guid> pk)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            return await connection.QueryAsync<OrderTransportOrder, Order, OrderLine, OrderTransportOrder>(@"
                SELECT [oto].[OrderPK]
                      ,[oto].[TransportOrderPK]

                      ,[o].[OrderPK] AS Id
                      ,[o].[Origin]

                      ,'' AS Id
                      ,(SELECT SUM(PalletQty) FROM OrderLine WHERE OrderPK = [o].OrderPK) AS PalletQty
                  FROM [dbo].[OrderTransportOrder] [oto]
                LEFT JOIN [Order] [o] ON [oto].[OrderPK] = [o].[OrderPK]
                WHERE [TransportOrderPK] IN @pk
                ", (oto, o, ol) =>
            {
                o.Lines = new List<OrderLine>
                        {
                            ol
                        };
                oto.Order = o;

                return oto;
            },
                new
                {
                    pk
                });
        }
    }

    private async Task<List<TransportOrder>> GetListTransportOrders(List<Guid> pkList)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            string pks = String.Join(",", pkList.Select(x => "'" + x.ToString() + "'"));

            string sql = @"SELECT " + defaultFields + @"
                      ,[cu].[CurrencyPK] AS Id
                      ,[cu].[CurrencyPK]
                      ,[cu].[Name]
                      ,[cu].[ShortName]
                FROM [dbo].[TransportOrder] [to]
                JOIN [Currency] cu ON [to].[CurrencyPK] = cu.[CurrencyPK]
                WHERE [to].[TransportOrderPK] IN (" + pks + ")";

            var transportOrders = connection.Query<TransportOrder, Currency, TransportOrder>(
                sql,
                (to, cu) =>
                {
                    try
                    {
                        to.Currency = cu;
                        return to;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                });

            var result = await HandleTransportOrder(transportOrders.ToList());

            return result;
        }
    }

    private static List<string> GetFiltersWithNoColumn()
    {
        return new List<string>
            {
                "EtaTo",
                "EtaWeekDay",
                "CollectionDateWeekDay",
                "CollectionDateTo",
                "UpdateWeekDay",
                "UpdateTo"
            };
    }

    private void HandleNonColumnDataTimeFilter(SearchParameters parameters, List<string> dateTimeFiltersWithNoColumn, SqlBuilder builder)
    {
        foreach (var filterNoColumn in dateTimeFiltersWithNoColumn)
        {
            if (parameters.Filters.ContainsKey(filterNoColumn))
            {
                var sb = new StringBuilder();

                var paraFilter = parameters.Filters[filterNoColumn];
                switch (filterNoColumn)
                {
                    case "EtaTo":
                        if (parameters.Filters.ContainsKey("DeliveryDate"))
                        {
                            sb.Append(GetBetweenSQLSyntax(columnMapping["DeliveryDate"].row,
                                parameters.Filters["DeliveryDate"], paraFilter));
                            parameters.Filters.Remove("DeliveryDate");
                        }
                        else
                        {
                            sb.AppendFormat(columnMapping["DeliveryDate"].search, paraFilter);
                        }
                        break;
                    case "EtaWeekDay":
                        sb.Append(GetWeekDaySQLSyntax(columnMapping["DeliveryDate"].row, paraFilter));
                        break;
                    case "CollectionDateWeekDay":
                        sb.Append(GetWeekDaySQLSyntax(columnMapping["CollectionDate"].row, paraFilter));
                        break;
                    case "CollectionDateTo":
                        if (parameters.Filters.ContainsKey("CollectionDate"))
                        {
                            sb.Append(GetBetweenSQLSyntax(columnMapping["CollectionDate"].row,
                                parameters.Filters["CollectionDate"], paraFilter));
                            parameters.Filters.Remove("CollectionDate");
                        }
                        else
                        {
                            sb.AppendFormat(columnMapping["CollectionDate"].search, paraFilter);
                        }
                        break;
                    case "UpdateWeekDay":
                        sb.Append(GetWeekDaySQLSyntax(columnMapping["Updated"].row, paraFilter));
                        break;
                    case "UpdateTo":
                        if (parameters.Filters.ContainsKey("Updated"))
                        {
                            sb.Append(GetBetweenSQLSyntax(columnMapping["Updated"].row,
                                parameters.Filters["Updated"], paraFilter));
                            parameters.Filters.Remove("Updated");
                        }
                        else
                        {
                            sb.AppendFormat(columnMapping["Updated"].search, paraFilter);
                        }
                        break;
                }

                parameters.Filters.Remove(filterNoColumn);
                builder.Where(sb.ToString());
            }
        }
    }

    private string GetWeekDaySQLSyntax(string row, string paraFilter)
    {
        return $"DATEPART(WEEKDAY ,{row}) LIKE'%{paraFilter}%'";
    }

    private string GetBetweenSQLSyntax(string column, string dateTimeFrom, string dateTimeTo)
    {
        return $"{column} BETWEEN '{dateTimeFrom}' AND '{dateTimeTo}'";
    }

    private async Task<int> SearchTransportOrdersCount(SqlBuilder builder)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var template = builder.AddTemplate(strPalletsAndConsolidatedPallets + strCountSearchTransportOrders);

            var result = await connection.QueryAsync<int>(template.RawSql, template.Parameters);

            return result.FirstOrDefault();
        }
    }
}

public class SearchResponse
{
    public List<TransportOrder> TOrders { get; set; }
    public int Count { get; internal set; }
}