import React, { useState, useEffect, useRef } from "react";
import Select from "react-select";
import {
  useReactTable,
  getCoreRowModel,
  flexRender,
  getPaginationRowModel,
  getSortedRowModel,
} from "@tanstack/react-table";
import { FaArrowUp, FaArrowDown } from "react-icons/fa";
import { SiMicrosoftexcel } from "react-icons/si";
import * as XLSX from "xlsx";
import { IoIosWarning } from "react-icons/io";
import { FaRegComment } from "react-icons/fa6";
import { MdOutlineStopCircle } from "react-icons/md";
import { BiSolidPlusSquare } from "react-icons/bi";
import SearchBar from "../Searchbar";
import getAllOrders from "../APICalls/Orders/GetAllHistoricalOrders";
import { Link } from "react-router-dom";
// import updateOrder from './APICalls/Orders/UpdateOrder'

const HistoricalOrderTable = () => {
  const [selectedColumns, setSelectedColumns] = useState([]);
  const [data, setData] = useState([]);
  const [sorting, setSorting] = useState([]);
  const [columnFilters, setColumnFilters] = useState({});
  const [selectedRow, setSelectedRow] = useState(null);
  const [tableData, setTableData] = useState([]);
  const [loading, setLoading] = useState(false);

  const columns = [
    {
      header: "Status",
      accessorKey: "status",
    },
    {
      header: "Pickup from ",
      accessorKey: "collectionDate",
    },
    {
      header: "Pickup to",
      accessorKey: "deliveryDate",
    },
    {
      header: "Pickup weekday",
      accessorKey: "collectionDateWD",
    },
    {
      header: "OrderId",
      accessorKey: "orderId",
    },
    {
      header: "Supplier",
      accessorKey: "supplier.businessUnit.name",
    },
    {
      header: "From Country",
      accessorKey: "supplier.businessUnit.contactInformations.0.country",
    },
    {
      header: "Eur Pallets",
      accessorKey: "euQty",
    },
    {
      header: "Sea Pallets",
      accessorKey: "seaQty",
    },
    {
      header: "Pallet Exch",
      accessorKey: "palletExchange",
    },
    {
      header: "Item Id",
      accessorKey: "lines.0.item.itemID",
    },
    {
      header: "Item",
      accessorKey: "lines.0.item.name",
    },
    {
      header: "Item Qty",
      accessorKey: "lines.0.itemQty",
    },
    {
      header: "Lines",
      accessorKey: "lines.length",
    },
    {
      header: "Temp",
      accessorKey: "lines.0.item.transportTemp",
    },
    {
      header: "ETA From",
      accessorKey: "transportOrder.arrivalDate",
    },
    {
      header: "ETA To",
      accessorKey: "transportOrder.deliveryDate",
    },
    {
      header: "ETA Weekday",
      accessorKey: "DeliveryDateWD",
    },
    {
      header: "Customer",
      accessorKey: "customer.businessUnit.name",
    },
    {
      header: "To Country",
      accessorKey: "customer.businessUnit.contactInformations.0.country",
    },
    {
      header: "Address",
      accessorKey: "customer.businessUnit.contactInformations.0.address",
    },
    {
      header: "Item Company",
      accessorKey: "lines.0.item.company",
    },
    {
      header: "Origin",
      accessorKey: "origin",
    },
    {
      header: "Comment",
      accessorKey: "internalComment",
    },
    {
      header: "Created",
      accessorKey: "created",
    },
    {
      header: "Updated",
      accessorKey: "updated",
    },
    {
      header: "Update weekday",
      accessorKey: "updatedWD",
    },
    {
      header: "Updated by",
      accessorKey: "updatedByUserId",
    },
    {
      header: "Transport booking",
      accessorKey: "transportBooking",
    },
  ];

  useEffect(() => {
    const fetchData = async () => {
      try {
        const data = await getAllOrders();
        const transformedData = data.orders.map(row => ({
          ...row,
          collectionDateWD: getWeekdayFromDate(row.collectionDate), // Pickup Weekday
          DeliveryDateWD: getWeekdayFromDate(row.transportOrder.deliveryDate), // ETA Weekday
          updatedWD: getWeekdayFromDate(row.updated) // Update Weekday
        }));
        setTableData(transformedData);
      } catch (error) {
        console.error('Error fetching orders:', error);
      }
    };

    fetchData();
  }, []);

  const exportToExcel = () => {
    const ws = XLSX.utils.json_to_sheet(data);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, "Orders");
    XLSX.writeFile(wb, "Orders.xlsx");
  };

  useEffect(() => {
    setLoading(true);
    setTimeout(() => {
      setLoading(false);
    }, 5000);
  }, []);

  const fetchOrders = async () => {
    try {
      const orders = await getAllOrders();
      setData(orders.orders);
    } catch (error) {
      console.error('Error fetching orders: ', error.message);
    }
  };

  const toggleRowSelection = (orderPK) => {
    if (selectedRow === orderPK) {
      setSelectedRow(null);
    } else {
      setSelectedRow(orderPK);
    }
  };

  useEffect(() => {
    console.log("RowID:", selectedRow);
  }, [selectedRow]);

  const table = useReactTable({
    data,
    columns: selectedColumns,
    initialState: {
      pagination: {
        pageSize: 25,
      },
    },
    state: {
      sorting: sorting,
    },
    getCoreRowModel: getCoreRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    getSortedRowModel: getSortedRowModel(),
    onSortingChange: setSorting,
  });

  const tableRef = useRef(null);

  useEffect(() => {
    setSelectedColumns(defaultSelectedColumns);
    fetchOrders();
  }, []); // Empty dependency array so it only runs once

  const defaultSelectedAccessorKeys = [
    "status",
    "orderId",
    "supplier.businessUnit.name",
    "transportBooking"
  ];
  const defaultSelectedColumns = columns.filter((column) =>
    defaultSelectedAccessorKeys.includes(column.accessorKey)
  );

  const handleColumnSelection = (selectedOptions) => {
    if (!selectedOptions) {
      return;
    }

    if (selectedOptions.some((option) => option.value === "select-all")) {
      // Select All
      setSelectedColumns(columns);
      return;
    }

    const selectedColumns = selectedOptions
      .filter((option) => !defaultSelectedAccessorKeys.includes(option.value)) // Filter out default selected columns
      .map((option) =>
        columns.find((column) => column && column.accessorKey === option.value)
      )
      .filter((column) => column); // Filter out undefined or null columns

    // Merge default  columns with new columns
    const updatedSelectedColumns = [
      ...defaultSelectedColumns,
      ...selectedColumns,
    ];
    setSelectedColumns(updatedSelectedColumns);
  };

  const selectAllOption = { value: "select-all", label: "Select All" };
  const options = [
    selectAllOption,
    ...columns.map((column) => ({
      value: column.accessorKey,
      label: column.header,
    })),
  ];
  // Function to update filter value for a column
  const handleColumnFilterChange = (columnId, value) => {
    setColumnFilters({
      ...columnFilters,
      [columnId]: value,
    });
  };

  const filterData = (row) => {
    for (const columnId in columnFilters) {
      const filterValue = columnFilters[columnId];
      const cellValue = row[columnId]
        ? row[columnId].toString()
        : "";

      console.log("Filter Value:", filterValue);
      console.log("Cell Value:", cellValue);

      if (cellValue.toLowerCase().indexOf(filterValue.toLowerCase()) === -1) {
        return false; // Do not include row if any filter does not match
      }
    }
    return true; // Include row if all filters match
  };

  return (
    <div className="text-dark-green w-full">
      <div className="flex justify-between mb-3">
        <Select
          className="text-dark-green duration-500 cursor-pointer ring-medium-green ring-2 bg-medium-green font-medium rounded text-xl mb-2 inline-flex "
          options={options}
          isMulti
          onChange={handleColumnSelection}
          placeholder="Columns"
          defaultValue={selectedColumns.map((column) => ({
            value: column.accessorKey,
            label: column.header,
          }))}
        />
      </div>

      {loading ? (
        <div className="top-1/2 left- h-screen w-screen z-50 flex justify-center items-top">
          <div className="animate-spin rounded-full h-48 w-48 border-t-2 border-b-2 border-medium-green"></div>
        </div>
      ) : (
        <div className="mb-5">
          <table
            ref={tableRef}
            className="table-fixed border-x border-b w-full"
          >
            <thead className="border">
              {table.getHeaderGroups().map((headerGroup) => (
                <React.Fragment key={headerGroup.id}>
                  <tr>
                    <th className="border p-2 bg-white relative w-32"></th>
                    {headerGroup.headers.map((header) => (
                      <th
                        className="border p-2 bg-white relative truncate"
                        key={header.id}
                        onClick={header.column.getToggleSortingHandler()}
                      >
                        {flexRender(
                          header.column.columnDef.header,
                          header.getContext()
                        )}
                        {
                          {
                            asc: <FaArrowUp />,
                            desc: <FaArrowDown />,
                          }[header.column.getIsSorted()]
                        }
                      </th>
                    ))}
                  </tr>
                  <tr>
                    <th>
                      <SearchBar disabled></SearchBar>
                    </th>
                    {headerGroup.headers.map((header) => (
                      <th className="" key={header.id}>
                        <SearchBar
                          onFilterChange={(value) =>
                            handleColumnFilterChange(header.id, value)
                          }
                        />
                      </th>
                    ))}
                  </tr>
                </React.Fragment>
              ))}
            </thead>
            <tbody className="text-dark-green">
              {table
                .getRowModel()
                .rows.filter((row) => filterData(row.original)) // Apply filtering
                .map((row) => (
                  <tr className="odd:bg-gray hover:bg-brown" key={row.id}>
                    <td className="border-b p-1 text-center flex gap-2 truncate">
                      <input
                        className="accent-medium-green h-5 w-5 rounded-xl ml-1"
                        type="checkbox"
                        checked={selectedRow === row.original.orderPK}
                        onChange={() => toggleRowSelection(row.original.orderPK)}
                      />
                      <div className="relative group">
                        <IoIosWarning className="text-2xl text-red relative group cursor-pointer" />
                        <span className="absolute top-5 left-5 bg-white border w-40 text-dark-green p-2 rounded opacity-0 transition-opacity duration-700 group-hover:opacity-100 z-10">
                          This order has transport booking set to no in M3 and
                          cannot be transport booked.
                        </span>
                      </div>

                      <div className="relative group">
                        <FaRegComment className="text-2xl relative group cursor-pointer" />
                        <span className="absolute top-6 left-6 bg-white border w-40 text-dark-green p-2 rounded hover:border opacity-0 transition-opacity duration-700 group-hover:opacity-100 z-10">
                          Edit internal comment.
                        </span>
                      </div>
                      <div className="relative group">
                        <BiSolidPlusSquare className="text-2xl relative group text-blue cursor-pointer " />
                        <span className="absolute top-6 left-6 bg-white border w-40 text-dark-green p-2 rounded opacity-0 transition-opacity duration-700 group-hover:opacity-100 z-10">
                          Create new transport order.
                        </span>
                      </div>
                      <div className="relative group">
                        <MdOutlineStopCircle className="text-2xl" />
                        <span className="absolute top-6 left-6 bg-white border w-40 text-dark-green p-2 rounded opacity-0 transition-opacity duration-700 group-hover:opacity-100 z-10">
                          Note! Order is already transport booked.
                        </span>
                      </div>
                    </td>
                    {row.getVisibleCells().map((cell) => (
                      <td
                        className="border p-1 text-center truncate"
                        key={cell.id}
                      >
                        {flexRender(
                          cell.column.columnDef.cell,
                          cell.getContext()
                        )}
                      </td>
                    ))}
                  </tr>
                ))}
            </tbody>
          </table>
        </div>
      )}

      <div className="flex justify-center gap-3 mt-2">
        <button
          onClick={() => table.setPageIndex(0)}
          className="bg-medium-green hover:bg-brown duration-200 py-2 px-3 rounded text-white"
        >
          First
        </button>
        <button
          disabled={!table.getCanPreviousPage()}
          onClick={() => table.previousPage()}
          className="bg-medium-green hover:bg-brown duration-200 py-2 px-3 rounded text-white"
        >
          Prev
        </button>
        <span className="flex items-center gap-1">
          <strong>
            {table.getState().pagination.pageIndex + 1} of{" "}
            {table.getPageCount()}
          </strong>
        </span>
        <button
          disabled={!table.getCanNextPage()}
          onClick={() => table.nextPage()}
          className="bg-medium-green hover:bg-brown duration-200 py-2 px-3 rounded text-white"
        >
          Next
        </button>
        <button
          onClick={() => table.setPageIndex(table.getPageCount() - 1)}
          className="bg-medium-green duration-200 hover:bg-brown  py-2 px-3 rounded text-white"
        >
          Last
        </button>
      </div>
      <div className="flex justify-center mt-4">
        <select
          id="showbutton"
          className="cursor-pointer bg-medium-green duration-200 hover:bg-brown  py-2 rounded text-white text-center"
          value={table.getState().pagination.pageSize}
          onChange={(e) => {
            table.setPageSize(Number(e.target.value));
          }}
        >
          {[25, 50, 75, 100].map((pageSize) => (
            <option key={pageSize} value={pageSize}>
              Show {pageSize}
            </option>
          ))}
        </select>
      </div>
      <hr className="m-4"></hr>
        <div className="flex justify-between">
          <Link
            to={`/Transport/Order/Editorder?selectedIds=${selectedRow || ''}`}
            className="p-2 duration-300 hover:bg-medium-green hover:text-white rounded-lg ml-6 text-2xl border border-medium-green"
          >
            Edit Order
          </Link>
          <div className="flex gap-2">
            <button
              onClick={exportToExcel}
              className="p-2 duration-200 hover:bg-medium-green hover:text-white rounded-lg mr-3 text-2xl border border-medium-green flex items-center"
            >
              <SiMicrosoftexcel className="mr-2" />
              Excel
            </button>

            <Link
              to="/Transport/OrderDetails/Details"
              className="p-2 duration-300 hover:bg-medium-green hover:text-white rounded-lg mr-6 text-2xl border border-medium-green"
            >
              Group
            </Link>
          </div>
        </div>
    </div>
    
  );
};
export default HistoricalOrderTable;