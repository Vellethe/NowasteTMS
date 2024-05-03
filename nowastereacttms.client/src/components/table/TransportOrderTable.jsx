import React, { useState, useMemo, useEffect, useRef } from 'react'
import Select from 'react-select';
import {
  useReactTable,
  getCoreRowModel,
  flexRender,
  getPaginationRowModel,
  getSortedRowModel
} from "@tanstack/react-table";
import { FaArrowUp, FaArrowDown } from "react-icons/fa";
import { SiMicrosoftexcel } from "react-icons/si";
import * as XLSX from "xlsx";
import SearchBar from '../Searchbar';
import getAllTransportOrders from '../APICalls/TransportOrders/GetAllTransportOrders';
import getAllServices from '../APICalls/Service/GetAllServices';

const TransportOrderTable = () => {
  const [selectedColumns, setSelectedColumns] = useState([]);
  const [sorting, setSorting] = useState([]);
  const [data, setData] = useState([]);
  const [services, setServices] = useState([]);
  const [tableData, setTableData] = useState([]);
  const [columnFilters, setColumnFilters] = useState({});

  /**@type import('@tanstack/react-table').ColumnDef<any> */
  const columns = [
    {
      header: "OrderId",
      accessorKey: "order.orderId",
    },
    {
      header: "Status",
      accessorKey: "orderstatus",
    },
    {
      header: "Pickup from ",
      accessorKey: "arrivalDate",
    },
    {
      header: "Pickup to",
      accessorKey: "collectionDate",
    },
    {
      header: "Pickup weekday",
      accessorKey: "collectionDateWD",
    },
    {
      header: "Ref",
      accessorKey: "transportOrderID",
    },
    {
      header: "From Country",
      accessorKey: "transportOrderLines.0.fromContactInformation.country",
    },
    {
      header: "From City",
      accessorKey: "transportOrderLines.0.fromContactInformation.city",
    },
    {
      header: "Lines",
      accessorKey: "transportOrderLines.length",
    },
    {
      header: "Eur Pallets",
      accessorKey: "eurPalletQty",
    },
    {
      header: "Sea Pallets",
      accessorKey: "PalletQty",
    },
    {
      header: "Transporter",
      accessorKey: "transportOrderLines.0.agent.businessUnit.name",
    },
    {
      header: "Vehicle",
      accessorKey: "vehicleRegistrationPlate",
    },
    {
      header: "Services",
      accessorKey: "Services",
    },
    {
      header: "Price",
      accessorKey: "price",
    },
    {
      header: "Currency",
      accessorKey: "currency.shortName",
    },
    {
      header: "ETA From",
      accessorKey: "createdOn",
    },
    {
      header: "ETA To",
      accessorKey: "deliveryDate",
    },
    {
      header: "ETA Weekday",
      accessorKey: "deliveryDateWD",
    },
    {
      header: "To Country",
      accessorKey: "transportOrderLines.0.toContactInformation.country",
    },
    {
      header: "To City",
      accessorKey: "transportOrderLines.0.toContactInformation.city",
    },
    {
      header: "Customer Name",
      accessorKey: "transportOrderLines.0.toCustomerName",
    },
    {
      header: "Supplier name",
      accessorKey: "transportOrderLines.0.fromSupplierName",
    },
    {
      header: "Internal comment",
      accessorKey: "internalComment",
    },
    {
      header: "Updated by",
      accessorKey: "updatedByUserId",
    },
    {
      header: "Update from",
      accessorKey: "originalDeliveryDate",
    },
    {
      header: "Update to",
      accessorKey: "updated",
    },
    {
      header: "Update Week Day",
      accessorKey: "updatedWD",
    },
  ];

  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await getAllTransportOrders();
        const serviceResponse = await getAllServices();
        setData(response.transportOrders); // Set the fetched transport orders to data
        setServices(serviceResponse);
      } catch (error) {
        console.error('Error fetching transportorders:', error);
      }
    };

    fetchData();
  }, []);

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
    setSelectedColumns(columns);
  }, []); // Empty dependency array so it only runs once

  const defaultSelectedAccessorKeys = ['OrderIds', 'OrderTransportStatusString', 'Price', 'EtaTo', 'VehicleRegistrationPlate'];
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

    // Merge default columns with new columns
    const updatedSelectedColumns = [...defaultSelectedColumns, ...selectedColumns];
    setSelectedColumns(updatedSelectedColumns);
  };

  const selectAllOption = { value: 'select-all', label: 'Select All' };
  const options = [selectAllOption, ...columns.map(column => ({
    value: column.accessorKey,
    label: column.header
  }))];

  const exportToExcel = () => {
    const ws = XLSX.utils.json_to_sheet(data);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, "TransportOrders");
    XLSX.writeFile(wb, "TransportOrders.xlsx");
  };

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
      <div className="overflow-auto relative">
        <table ref={tableRef} className="table-fixed border-x border-b w-full">
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
            {table.getRowModel().rows
              .filter((row) => filterData(row.original)) // Apply filtering
              .map((row) => (
                <tr className="odd:bg-gray hover:bg-brown" key={row.id}>
                  <td className=""></td>
                  {row.getVisibleCells().map((cell) => (
                    <td className="border p-1 text-center truncate" key={cell.id}>
                      {flexRender(cell.column.columnDef.cell, cell.getContext())}
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
            {table.getState().pagination.pageIndex + 1} of {table.getPageCount()}
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
      <div className="flex gap-2 justify-end mr-2">
        <button onClick={exportToExcel} className="flex items-center gap-2 text-2xl">
          <SiMicrosoftexcel />
          Excel {/* Detta kan tas bort eller bytas mot en tydligare excel icon */}
        </button>
      </div>
    </div>
  );
}
export default TransportOrderTable;