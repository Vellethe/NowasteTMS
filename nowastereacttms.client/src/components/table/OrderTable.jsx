import React, { useState, useMemo, useEffect, useRef } from "react";
import Select from "react-select";
import {
  useReactTable,
  getCoreRowModel,
  flexRender,
  getPaginationRowModel,
  getSortedRowModel,
} from "@tanstack/react-table";
import mData from "../../data/MOCK_DATA.json";
import { LuChevronsUpDown } from "react-icons/lu";
import { IoIosWarning } from "react-icons/io";
import { FaRegComment } from "react-icons/fa6";
import { MdOutlineStopCircle } from "react-icons/md";
import { BiSolidPlusSquare } from "react-icons/bi";
import SearchBar from "../Searchbar";

const OrderTable = () => {
  const [selectedColumns, setSelectedColumns] = useState([]);
  const data = useMemo(() => mData, []);

  /**@type import('@tanstack/react-table').ColumnDef<any> */
  const columns = [
    {
      header: "Status",
      accessorKey: "OrderTransportStatusString",
    },
    {
      header: "Pickup from ",
      accessorKey: "CollectionDate",
    },
    {
      header: "Pickup to",
      accessorKey: "CollectionDateTo",
    },
    {
      header: "Pickup weekday",
      accessorKey: "CollectionDateWD",
    },
    {
      header: "OrderId",
      accessorKey: "OrderId",
    },
    {
      header: "Supplier",
      accessorKey: "SupplierName",
    },
    {
      header: "From Country",
      accessorKey: "SupplierCountry",
    },
    {
      header: "Eur Pallets",
      accessorKey: "OrderLinesTypeId2",
    },
    {
      header: "Sea Pallets",
      accessorKey: "OrderLinesTypeId8",
    },
    {
      header: "Pallet Exch",
      accessorKey: "PalletExchange",
    },
    {
      header: "Item Id",
      accessorKey: "ItemID",
    },
    {
      header: "Item",
      accessorKey: "ItemName",
    },
    {
      header: "Item Qty",
      accessorKey: "ItemQty",
    },
    {
      header: "Lines",
      accessorKey: "LineCount",
    },
    {
      header: "Temp",
      accessorKey: "TransportTemp",
    },
    {
      header: "ETA From",
      accessorKey: "DeliveryDate",
    },
    {
      header: "ETA To",
      accessorKey: "DeliveryDateTo",
    },
    {
      header: "ETA Weekday",
      accessorKey: "DeliveryDateWD",
    },
    {
      header: "Customer",
      accessorKey: "CustomerName",
    },
    {
      header: "To Country",
      accessorKey: "CustomerCountry",
    },
    {
      header: "Adress",
      accessorKey: "CustomerAddress",
    },
    {
      header: "Item Company",
      accessorKey: "ItemCompany",
    },
    {
      header: "Origin",
      accessorKey: "Origin",
    },
    {
      header: "Comment",
      accessorKey: "InternalComment",
    },
    {
      header: "Created",
      accessorKey: "Created",
    },
    {
      header: "Update from",
      accessorKey: "Updated",
    },
    {
      header: "Update to",
      accessorKey: "UpdatedTo",
    },
    {
      header: "Update weekday",
      accessorKey: "UpdatedWD",
    },
    {
      header: "Updated by",
      accessorKey: "Email",
    },
    {
      header: "Transport booking",
      accessorKey: "TransportBooking",
    },
  ];

  const [sorting, setSorting] = useState([]);
  const [searchValue, setSearchValue] = useState("");

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
  }, []); //Empty dependency array so it only runs once

  const defaultSelectedAccessorKeys = [
    "OrderPK",
    "OrderId",
    "SupplierName",
    "TransportBooking",
    "OrderTransportStatusString",
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

  const [columnFilters, setColumnFilters] = useState({});

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
        ? row[columnId].toString().toLowerCase()
        : "";
      if (cellValue.indexOf(filterValue.toLowerCase()) === -1) {
        return false; // Do not include row if any filter does not match
      }
    }
    return true; // Include row if all filters match
  };
  const [orders, setOrders] = useState([]);

  useEffect(() => {
    // Fetch data from your API
    fetch(
      "https://localhost:7253/api/Order?pk=2A8D6E5A-7CBC-4F0B-BAAC-007206E994B1"
    )
      .then((response) => response.json())
      .then((data) => {
        // Update state with fetched data
        setOrders(data);
        console.log(data);
      })
      .catch((error) => {
        console.error("Error fetching data:", error);
      });
  }, []);

  return (
    <div className="text-dark-green w-full">
      <Select
        className="text-dark-green duration-500  ring-medium-green ring-2 bg-medium-green font-medium rounded text-xl mb-2 inline-flex"
        options={options}
        isMulti
        onChange={handleColumnSelection}
        placeholder="Columns"
        defaultValue={selectedColumns.map((column) => ({
          value: column.accessorKey,
          label: column.header,
        }))}
      />

      <div className="mb-5">
        <table ref={tableRef} className="table-fixed border-x border-b w-full">
          <thead className="border ">
            {table.getHeaderGroups().map((headerGroup) => (
              <React.Fragment key={headerGroup.id}>
                <tr>
                  <th className="border p-2 bg-white relative"></th>
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
                          asc: <LuChevronsUpDown />,
                          desc: <LuChevronsUpDown />,
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
                    <th className=""key={header.id}>
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
                  <td className=" border-b p-1 text-center flex gap-2 truncate">
                    <input
                      className="accent-medium-green h-5 w-5 rounded-xl ml-1"
                      type="checkbox"
                    />

                    <div className="relative group">
                      <IoIosWarning className="text-2xl text-red" />
                      <span className="absolute top-5 left-5 bg-white border w-40 text-dark-green p-2 rounded opacity-0 transition-opacity duration-700 group-hover:opacity-100 z-10">
                        This order has transport booking set to no in M3 and cannot be transport booked.
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
                    <td className="border p-1 text-center truncate" key={cell.id}>
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

      <div className="flex justify-center gap-3 mt-2">
        <button
          onClick={() => table.setPageIndex(0)}
          className="bg-medium-green hover:bg-brown duration-200 text-gray-800 py-2 px-3 rounded-lg text-white"
        >
          First
        </button>
        <button
          disabled={!table.getCanPreviousPage()}
          onClick={() => table.previousPage()}
          className="bg-medium-green hover:bg-brown duration-200 text-gray-800 py-2 px-3 rounded-lg text-white"
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
          className="bg-medium-green hover:bg-brown duration-200 text-gray-800 py-2 px-3 rounded-lg text-white"
        >
          Next
        </button>
        <button
          onClick={() => table.setPageIndex(table.getPageCount() - 1)}
          className="bg-medium-green duration-200 hover:bg-brown text-gray-800 py-2 px-3 rounded-lg text-white"
        >
          Last
        </button>
        <select
          id="showbutton"
          className="cursor-pointer"
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
    </div>
  );
};
export default OrderTable;
