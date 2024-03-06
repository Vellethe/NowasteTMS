using Dapper;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NowasteReactTMS.Server;
using NowasteTms.Model;
using System.Text;
using WMan.Data.ConnectionFactory;
using IConnectionFactory = WMan.Data.ConnectionFactory.IConnectionFactory;
public class OrderRepository : IOrderRepository
{
    private readonly IConnectionFactory connectionFactory;
    private readonly IOrderLineRepository orderLineRepository;
    private readonly IBusinessUnitRepository businessUnitRepository;
    private readonly ITransportOrderRepository transportOrderRepository;
    //private readonly NowastePalletPortalContext portalContext;

    private readonly Dictionary<string, ColumnMapping> columnMapping = new Dictionary<string, ColumnMapping>
        {
            { "DeliveryDate", new ColumnMapping("po.[DeliveryDate]",useLike:false, isDate:true)},
            { "DeliveryDateWD", new ColumnMapping("DATEPART(WEEKDAY ,po.[DeliveryDate])")},
            { "OrderId", new ColumnMapping("po.[OrderId]")},

            { "OrderTransportStatus", new ColumnMapping() {row="[to].[Orderstatus]", search="([to].[Orderstatus]={0} OR ({0}=0 AND [to].[Orderstatus] IS NULL))" } },

            { "Origin", new ColumnMapping("po.[Origin]")},
            { "CollectionDate", new ColumnMapping("po.[CollectionDate]",useLike:false, isDate:true)},
            { "CollectionDateWD", new ColumnMapping("DATEPART(WEEKDAY ,po.[CollectionDate])")},
            { "PalletExchange", new ColumnMapping("po.[PalletExchange]")},
            { "Created", new ColumnMapping("po.[Created]",useLike:false, isDate:true)},
            { "Updated", new ColumnMapping("po.[Updated]",useLike:false, isDate:true)},
            { "UpdatedWD", new ColumnMapping("DATEPART(WEEKDAY ,po.[Updated])")},
            { "Comment", new ColumnMapping("po.[Comment]")},
            { "InternalComment", new ColumnMapping("po.[InternalComment]")},
            { "CustomerName", new ColumnMapping("cbu.[Name]")},
            { "SupplierName", new ColumnMapping("sbu.[Name]")},
            { "LineCount", new ColumnMapping("(SELECT COUNT(*) FROM OrderLine ol1 WHERE ol1.OrderPK = po.OrderPK)")},
            { "PalletQty", new ColumnMapping("(SELECT SUM(ol1.PalletQty) FROM OrderLine ol1 WHERE ol1.OrderPK = po.OrderPK)")},
            { "OrderLinesTypeId2", new ColumnMapping("(SELECT SUM(ol2.PalletQty) FROM OrderLine ol2 WHERE ol2.OrderPK = po.OrderPK AND ol2.PalletTypeId=2)")},
            { "OrderLinesTypeId8", new ColumnMapping("(SELECT SUM(ol8.PalletQty) FROM OrderLine ol8 WHERE ol8.OrderPK = po.OrderPK AND ol8.PalletTypeId=8)")},
            { "PalletType", new ColumnMapping("pt.Description")},
            { "ItemID", new ColumnMapping("i.[ItemID]")},
            { "ItemName", new ColumnMapping("i.[Name]")},
            { "ItemCompany", new ColumnMapping("i.[Company]")},
            { "ItemQty", new ColumnMapping("ol.[ItemQty]")},
            { "Orderstatus", new ColumnMapping("[to].[Orderstatus]")},
            { "CustomerCountry", new ColumnMapping("[ci].[Country]")},
            { "SupplierCountry", new ColumnMapping("[sci].[Country]")},
            { "Email", new ColumnMapping("[po].[Email]")},
            { "UpdatedByUserId", new ColumnMapping("[po].[UpdatedByUserId]")},
            { "TransportBooking", new ColumnMapping("[po].[TransportBooking]")},
            { "CustomerAddress", new ColumnMapping("[ci].[Address]")}
        };



    public OrderRepository(IConnectionFactory connectionFactory, IOrderLineRepository orderLineRepository, IBusinessUnitRepository businessUnitRepository,
        ITransportOrderRepository transportOrderRepository/*, NowastePalletPortalContext portalContext*/)
    {
        this.connectionFactory = connectionFactory;
        this.orderLineRepository = orderLineRepository;
        this.businessUnitRepository = businessUnitRepository;
        this.transportOrderRepository = transportOrderRepository;
        //this.portalContext = portalContext;
    }

    //public async Task<IEnumerable<Order>> GetAllOrders()
    //{
    //    return await portalContext.Orders.ToListAsync();
    //}

    public async Task<Order> Get(Guid pk)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var order = await connection.QueryAsync<Order, Customer, BusinessUnit, Supplier, BusinessUnit, Order>(@"
                SELECT 
                       po.[OrderPK] AS Id
                      ,po.[OrderPK]
                      ,po.[OrderId]
                      ,po.[Type]
                      ,po.[Status]
                      ,po.[Origin]
                      ,po.[CollectionDate]
                      ,po.[DeliveryDate]
                      ,po.[PalletExchange]
                      ,po.[SupplierPK]
                      ,po.[CustomerPK]
                      ,po.[HandlerID]
                      ,po.[Comment]
                      ,po.[InternalComment]
                      ,po.[Created]
                      ,po.[Updated]
                      ,po.[InterchangeReference]
                      ,po.[Email]
                      ,po.[TransportBooking]
                      ,po.[UpdatedByUserId]
                      ,cbu.[DivisionId]

                      ,c.[CustomerPK] AS Id
                      ,c.[CustomerPK]
                      ,c.[CustomerID]
                      ,c.[BusinessUnitPK]

                      ,cbu.[BusinessUnitPK] AS Id
                      ,cbu.[BusinessUnitPK]
                      ,cbu.[Name]
                      ,cbu.[Company]
                      ,cbu.[DivisionId]
                      ,cbu.[FinanceInformationPK]

                      ,s.[SupplierPK] AS Id
                      ,s.[SupplierPK]
                      ,s.[SupplierID]
                      ,s.[BusinessUnitPK]

                      ,sbu.[BusinessUnitPK] AS Id
                      ,sbu.[BusinessUnitPK]
                      ,sbu.[Name]
                      ,sbu.[Company]
                      ,sbu.[DivisionId]
                      ,sbu.[FinanceInformationPK]

					  ,ol.[PalletTypeId]

					  ,pt.[Description]

                FROM [dbo].[Order] po
                LEFT JOIN [Customer] c ON po.[CustomerPK] = c.[CustomerPK]
                LEFT JOIN [BusinessUnit] cbu ON c.[BusinessUnitPK] = cbu.[BusinessUnitPK]
                LEFT JOIN [Supplier] s ON po.[SupplierPK] = s.[SupplierPK]
                LEFT JOIN [BusinessUnit] sbu ON s.[BusinessUnitPK] = sbu.[BusinessUnitPK]
				LEFT JOIN [OrderLine] ol ON po.[OrderPK] = ol.[OrderPK]
				LEFT JOIN [PalletType] pt ON ol.[PalletTypeId] = pt.[Id]
				WHERE po.[OrderPK] = @pk",
        (po, c, cbu, s, sbu) =>
        {
            if (po.CustomerPK != Guid.Empty)
            {
                po.Customer = c;
                c.BusinessUnit = cbu;
            }

            if (po.SupplierPK != Guid.Empty)
            {
                po.Supplier = s;
                s.BusinessUnit = sbu;
            }

            return po;
        },
        new
        {
            pk
        }
        );

            return order.FirstOrDefault();
        }
    }
    public async Task<Order> GetOrder(Guid id)
    {
        var order = await Get(id);
        if (order == null)
            throw new Exception("Order not found");

        order.Lines = await orderLineRepository.GetByOrder(order.OrderPK);

        if (order.Customer?.BusinessUnitPK != null)
            order.Customer.BusinessUnit = await businessUnitRepository.Get(order.Customer.BusinessUnitPK);

        if (order.Supplier?.BusinessUnitPK != null)
            order.Supplier.BusinessUnit = await businessUnitRepository.Get(order.Supplier.BusinessUnitPK);

        order.TransportOrders = await transportOrderRepository.GetForOrder(id);

        return order;
    }

    public async Task<Order> GetById(string orderId)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var order = await connection.QueryAsync<Order>(@"
                SELECT 
                       o.[OrderPK]

                  FROM [dbo].[Order] o
                WHERE [OrderID] = @orderId",
                new
                {
                    orderId
                }
            );
            if (order.Any())
                return await this.Get(order.First().OrderPK);

            return null;
        }
    }

    public async Task<int> UpdateOrder(Order order)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            order.Updated = DateTime.Now;

            return await connection.ExecuteAsync(@"
                UPDATE [dbo].[Order]
                   SET [OrderPK] = @OrderPK
                      ,[OrderId] = @OrderId
                      ,[Type] = @Type
                      ,[Status] = @Status
                      ,[Origin] = @Origin
                      ,[CollectionDate] = @CollectionDate
                      ,[DeliveryDate] = @DeliveryDate
                      ,[PalletExchange] = @PalletExchange
                      ,[SupplierPK] = @SupplierPK
                      ,[CustomerPK] = @CustomerPK
                      ,[HandlerID] = @HandlerID
                      ,[Updated] = @Updated
                      ,[Comment] = @Comment
                      ,[InterchangeReference] = @InterchangeReference
                      ,[InternalComment] = @InternalComment
                      ,[UpdatedByUserId] = @UpdatedByUserId
                      ,[Email] = @Email
                      ,[TransportBooking] = @TransportBooking
                 WHERE [OrderPK] = @OrderPK ", order);
        }
    }

    public async Task<int> AddOrder(Order order)
    {

        using (var connection = connectionFactory.CreateConnection())
        {
            order.Created = DateTime.Now;
            order.Updated = order.Created;

            await connection.ExecuteAsync(@"
                INSERT INTO [dbo].[Order]
                           ([OrderPK]
                           ,[OrderId]
                           ,[Type]
                           ,[Status]
                           ,[Origin]
                           ,[CollectionDate]
                           ,[DeliveryDate]
                           ,[PalletExchange]
                           ,[SupplierPK]
                           ,[CustomerPK]
                           ,[HandlerID]
                           ,[Created]
                           ,[Updated]
                           ,[Comment]  
                           ,[InterchangeReference]
                           ,[InternalComment]
                           ,[UpdatedByUserId]
                           ,[Email]
                           ,[TransportBooking])
                     VALUES
                           (@OrderPK
                           ,@OrderId
                           ,@Type
                           ,@Status
                           ,@Origin
                           ,@CollectionDate
                           ,@DeliveryDate
                           ,@PalletExchange
                           ,@SupplierPK
                           ,@CustomerPK
                           ,@HandlerID
                           ,@Created
                           ,@Updated
                           ,@Comment
                           ,@InterchangeReference
                           ,@InternalComment
                           ,@UpdatedByUserId
                           ,@Email
                           ,@TransportBooking)", order);
        }

        if (order.Lines == null)
            return 1;
        foreach (var orderLine in order.Lines)
            await orderLineRepository.Add(orderLine);

        return 1;
    }

    public async Task<SearchOrderResponse> SearchOrders(SearchParameters parameters)
    {
        var response = new SearchOrderResponse();

        using (var connection = connectionFactory.CreateConnection())
        {
            var builder = new SqlBuilder();

            var sqlTemplate = @"
                SELECT 
                       po.[OrderPK] AS Id
                      ,po.[OrderPK]     /* view */
                      ,po.[OrderId]     /* view */
                      ,po.[Type]
                      ,po.[Status]     /* view */
                      ,po.[Origin]     /* view */
                      ,po.[CollectionDate]     /* view */
                      ,po.[DeliveryDate]     /* view */
                      ,po.[PalletExchange]     /* view */
                      ,po.[SupplierPK]     /* view */
                      ,po.[CustomerPK]     /* view */
                      ,po.[HandlerID]
                      ,po.[Comment]
                      ,po.[InternalComment]     /* view */
                      ,po.[Created]     /* view */
                      ,po.[Updated]     /* view */
                      ,po.[InterchangeReference]
                      ,po.[Email]     /* view */
                      ,po.[TransportBooking]     /* view */
                      ,po.[UpdatedByUserId]    /* view */

                      ,c.[CustomerPK] AS Id
                      ,c.[CustomerPK]
                      ,c.[CustomerID]
                      ,c.[BusinessUnitPK]

                      ,cbu.[BusinessUnitPK] AS Id
                      ,cbu.[BusinessUnitPK]
                      ,cbu.[Name]            /* view, CustomerName */

                      ,s.[SupplierPK] AS Id
                      ,s.[SupplierPK]
                      ,s.[SupplierID]
                      ,s.[BusinessUnitPK]

                      ,sbu.[BusinessUnitPK] AS Id
                      ,sbu.[BusinessUnitPK]
                      ,sbu.[Name]        /* view, SupplierName */
                    
                      ,[to].[TransportOrderPk] AS Id
                      ,[to].[Orderstatus]     /* view */

                  FROM [dbo].[Order] po
                LEFT JOIN [Customer] c ON po.[CustomerPK] = c.[CustomerPK]
                LEFT JOIN [BusinessUnit] cbu ON c.[BusinessUnitPK] = cbu.[BusinessUnitPK]
                LEFT JOIN [ContactInformation] ci ON ci.[ContactInformationPK] = 
                    ( -- zero to many contact rows, so join only first contact line
                        SELECT TOP 1 c.[ContactInformationPK]
                        FROM [ContactInformation] c
                        WHERE c.[IsDefault] = 1 AND c.[IsActive] = 1
                        AND c.[BusinessUnitPK] = cbu.[BusinessUnitPK]
                    )
                LEFT JOIN [Supplier] s ON po.[SupplierPK] = s.[SupplierPK]
                LEFT JOIN [BusinessUnit] sbu ON s.[BusinessUnitPK] = sbu.[BusinessUnitPK]
                LEFT JOIN [ContactInformation] sci ON sci.[ContactInformationPK] = 
                    ( -- join first contact line
                        SELECT TOP 1 c.[ContactInformationPK]
                        FROM [ContactInformation] c
                        WHERE c.[IsDefault] = 1 AND c.[IsActive] = 1
                        AND c.[BusinessUnitPK] = sbu.[BusinessUnitPK]
                    )
                LEFT JOIN [OrderTransportOrder] oto ON po.[OrderPK] = oto.[OrderPK]
                LEFT JOIN [TransportOrder] [to] ON oto.[TransportOrderPK] = [to].[TransportOrderPK]
                LEFT JOIN [OrderLine] ol ON ol.[OrderLinePK] =
                    ( -- join first order line
                        SELECT TOP 1 OrderLinePK
                        FROM OrderLine
                        WHERE OrderPK = po.OrderPK
                    )
                LEFT JOIN [PalletType] pt ON ol.[PalletTypeId] = pt.[Id]
                LEFT JOIN [Item] i ON ol.[ItemPK] = i.[ItemPK]
                 /**where**/";

            if (parameters.Limit < int.MaxValue)
                sqlTemplate += @"
                        /**orderby**/
                        OFFSET @offset ROWS
                        FETCH NEXT @limit ROWS ONLY";

            var template = builder.AddTemplate(sqlTemplate,
                new
                {
                    offset = parameters.Offset,
                    limit = parameters.Limit
                }
            );

            var historical = false;
            if (parameters.Filters.Keys.Contains("historical"))
            {
                historical = parameters.Filters["historical"] == "true";
                parameters.Filters.Remove("historical");
            }

            if (historical == false)
            {
                builder.Where("([po].[Status] < 75)");
            }
            else
            {
                builder.Where("([po].[Status] >= 75)");
            }

            var dateTimeFiltersWithNoColumn = GetFiltersWithNoColumn();
            if (parameters.Filters.Any(c => dateTimeFiltersWithNoColumn.Contains(c.Key)))
                HandleNonColumnDataTimeFilter(parameters, dateTimeFiltersWithNoColumn, builder);

            foreach (var filter in parameters.Filters)
            {
                var sb = new StringBuilder();
                sb.AppendFormat(columnMapping[filter.Key].search, filter.Value);
                builder.Where(sb.ToString());
            }

            if (parameters.SortOrders != null && parameters.SortOrders.Any())
            {
                foreach (var column in parameters.SortOrders)
                    builder.OrderBy(columnMapping[column.Key].row + " " + (column.Value ? "ASC" : "DESC"));
            }
            else if (parameters.Limit < int.MaxValue) // Forced default sort, OFFSET won't work otherwise
                builder.OrderBy("[Created] DESC");


            var orders = await connection.QueryAsync<Order, Customer, BusinessUnit, Supplier, BusinessUnit, TransportOrder, Order>(template.RawSql, (po, c, cbu, s, spu, to) =>
            {
                po.Customer = c;
                po.Supplier = s;

                po.Customer.BusinessUnit = cbu;
                po.Supplier.BusinessUnit = spu;

                po.TransportOrder = to;

                return po;
            }, template.Parameters);

            if (parameters.Limit == int.MaxValue)
                orders = orders.OrderBy(p => p.Status).ThenByDescending(p => p.Created);

            var ordersIterated = orders.ToList();

            var linksId = ordersIterated.Select(v => v.OrderPK);

            var links = linksId.Count() < 2000 ?
                await connection.QueryAsync<OrderTransportOrder>(
                    @"SELECT [OrderPK], [TransportOrderPK]
                            FROM [dbo].[OrderTransportOrder]
                            WHERE [OrderPK] IN @linksId", new { linksId }) :
                await connection.QueryAsync<OrderTransportOrder>(
                    @"SELECT [OrderPK], [TransportOrderPK]
                            FROM [dbo].[OrderTransportOrder]");

            foreach (var order in ordersIterated)
            {
                order.TransportOrders = links.Where(x => x.OrderPK == order.OrderPK)
                    .Select(x => new TransportOrder
                    {
                        TransportOrderPK = x.TransportOrderPK
                    });
            }

            response.Orders = ordersIterated.ToList();
            response.SearchCount = await SearchOrdersCount(builder);

            return response;
        }
    }

    private static List<string> GetFiltersWithNoColumn()
    {
        return new List<string>
            {
                "DeliveryDateTo",
                "CollectionDateTo",
                "UpdatedTo"
            };
    }

    private async Task<int> SearchOrdersCount(SqlBuilder builder)
    {
        using var connection = connectionFactory.CreateConnection();
        var template = builder.AddTemplate(@"
                SELECT 
                      COUNT(*)
                  FROM [dbo].[Order] po
                LEFT JOIN [Customer] c ON po.[CustomerPK] = c.[CustomerPK]
                LEFT JOIN [BusinessUnit] cbu ON c.[BusinessUnitPK] = cbu.[BusinessUnitPK]
                LEFT JOIN [ContactInformation] ci ON ci.[BusinessUnitPK] = cbu.[BusinessUnitPK] AND ci.[IsDefault] = 1 AND ci.[IsActive] = 1

                LEFT JOIN [Supplier] s ON po.[SupplierPK] = s.[SupplierPK]
                LEFT JOIN [BusinessUnit] sbu ON s.[BusinessUnitPK] = sbu.[BusinessUnitPK]
                LEFT JOIN [ContactInformation] sci ON sci.[BusinessUnitPK] = sbu.[BusinessUnitPK] AND sci.[IsDefault] = 1 AND sci.[IsActive] = 1

                LEFT JOIN [OrderTransportOrder] oto ON po.[OrderPK] = oto.[OrderPK]
                LEFT JOIN [TransportOrder] [to] ON oto.[TransportOrderPK] = [to].[TransportOrderPK]
                LEFT JOIN [OrderLine] ol ON ol.[OrderLinePK] =
                    ( -- join first order line
                        SELECT TOP 1 OrderLinePK
                        FROM OrderLine
                        WHERE OrderPK = po.OrderPK
                    )
                LEFT JOIN [PalletType] pt ON ol.[PalletTypeId] = pt.[Id]
                LEFT OUTER JOIN [Item] i ON ol.[ItemPK] = i.[ItemPK]
                 /**where**/");

        return await connection.QueryFirstAsync<int>(template.RawSql, template.Parameters);
    }

    private void HandleNonColumnDataTimeFilter(SearchParameters parameters, List<string> dateTimeFiltersWithNoColumn, SqlBuilder builder)
    {
        foreach (var filterNoColumn in dateTimeFiltersWithNoColumn)
        {
            if (!parameters.Filters.ContainsKey(filterNoColumn)) continue;

            var sb = new StringBuilder();
            var paraFilter = parameters.Filters[filterNoColumn];

            switch (filterNoColumn)
            {
                case "DeliveryDateTo":
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
                case "UpdatedTo":
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

    private string GetBetweenSQLSyntax(string column, string dateTimeFrom, string dateTimeTo)
    {
        return $"{column} BETWEEN '{dateTimeFrom}' AND '{dateTimeTo}'";
    }
}
