import React, { useState, useMemo, useEffect, useRef } from 'react'
import Select from 'react-select';
import {
  useReactTable,
  getCoreRowModel,
  flexRender,
  getPaginationRowModel,
  getSortedRowModel
} from "@tanstack/react-table";
import mData from "../../data/MOCK_DATA.json";
import { LuChevronsUpDown } from "react-icons/lu";
import { IoIosWarning } from "react-icons/io";
import { FaRegComment } from "react-icons/fa6";
import { MdOutlineSmartDisplay } from "react-icons/md";





import SearchBar from '../Searchbar';


const OrderTable = () => {
  const [selectedColumns, setSelectedColumns] = useState([]);
  const data = useMemo(() => mData, [])

  /**@type import('@tanstack/react-table').ColumnDef<any> */
  const columns = [
    {
      header: "Id",
      accessorKey: "OrderPK",
    },
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
    getCoreRowModel: getCoreRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    getSortedRowModel: getSortedRowModel(),
    state: {
      sorting: sorting,
    },
    onSortingChange: setSorting,
  });

  const tableRef = useRef(null);

  useEffect(() => {
    setSelectedColumns(defaultSelectedColumns);
  }, []); //Empty dependency array so it only runs once

  const defaultSelectedAccessorKeys = ['OrderPK', 'OrderId', 'SupplierName', 'TransportBooking', 'OrderTransportStatusString'];
  const defaultSelectedColumns = columns.filter(column => defaultSelectedAccessorKeys.includes(column.accessorKey));

  const handleColumnSelection = (selectedOptions) => {
    if (!selectedOptions) {
      return;
    }

    if (selectedOptions.some(option => option.value === 'select-all')) {
      // Select All
      setSelectedColumns(columns);
      return;
    }

    const selectedColumns = selectedOptions
      .filter(option => !defaultSelectedAccessorKeys.includes(option.value)) // Filter out default selected columns
      .map(option => columns.find(column => column && column.accessorKey === option.value))
      .filter(column => column); // Filter out undefined or null columns

    // Merge default  columns with new columns
    const updatedSelectedColumns = [...defaultSelectedColumns, ...selectedColumns];
    setSelectedColumns(updatedSelectedColumns);
  };

  const selectAllOption = { value: 'select-all', label: 'Select All' };
  const options = [selectAllOption, ...columns.map(column => ({
    value: column.accessorKey,
    label: column.header
  }))];




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
      const cellValue = row[columnId] ? row[columnId].toString().toLowerCase() : "";
      if (cellValue.indexOf(filterValue.toLowerCase()) === -1) {
        return false; // Do not include row if any filter does not match
      }
    }
    return true; // Include row if all filters match
  };
  const [orders, setOrders] = useState([]);

  useEffect(() => {
    // Fetch data from your API
    fetch('https://localhost:7253/api/Order?pk=2A8D6E5A-7CBC-4F0B-BAAC-007206E994B1')
      .then(response => response.json())
      .then(data => {
        // Update state with fetched data
        setOrders(data);
        console.log(data)
      })
      .catch(error => {
        console.error('Error fetching data:', error);
      });
  }, []);
  return (
    <div className="text-dark-green text-sm w-full">
      <div>Order: {orders.orderPK}</div>
      <button
        className="text-medium-blue duration-200 bg-blue hover:bg-medium-green focus:ring-2 focus:outline-none focus:ring-dark-green mb-3 font-medium rounded-lg text-xl px-5 py-2.5 text-center inline-flex items-center"
        type="button"
      >
        Columns
        <Select
          options={options}
          isMulti
          onChange={handleColumnSelection}
          defaultValue={selectedColumns.map((column) => ({
            value: column.accessorKey,
            label: column.header,
          }))}
        />
      </button>
      <div className="overflow-auto relative">
        <table ref={tableRef} className=" border-x border-b w-full">
          <thead className="border">
            {table.getHeaderGroups().map((headerGroup) => (
              <React.Fragment key={headerGroup.id}>
                <tr>
                  <th className="border p-2 bg-white relative">
                    
                  </th>
                  {headerGroup.headers.map((header) => (
                    <th
                      className="border p-2 bg-white relative"
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
                    <th key={header.id}>
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
                  <td className=" border p-1 text-center flex gap-2">
                    <IoIosWarning className="text-2xl text-red" />
                    <FaRegComment className="text-2xl" />
                    <MdOutlineSmartDisplay className="text-2xl" />
                    <input className="accent-medium-green h-5 w-5 rounded-xl ml-1" type="checkbox" />
                  </td>
                  {row.getVisibleCells().map((cell) => (
                    <td className="border p-1 text-center" key={cell.id}>
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
}
export default OrderTable;