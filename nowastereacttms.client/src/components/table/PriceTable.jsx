import React, { useState, useEffect, useRef } from 'react'
import Select from 'react-select';
import {
  useReactTable,
  getCoreRowModel,
  flexRender,
  getPaginationRowModel,
  getSortedRowModel
} from "@tanstack/react-table";
import { LuChevronsUpDown } from "react-icons/lu";
import { SiMicrosoftexcel } from "react-icons/si";
import * as XLSX from "xlsx";
import SearchBar from '../Searchbar';
import getAllPrices from '../APICalls/Prices/GetAllPrices';

const OrderTable = () => {
  const [sorting, setSorting] = useState([]);
  const [searchValue, setSearchValue] = useState("");
  const [selectedColumns, setSelectedColumns] = useState([]);
  const [data, setData] = useState([])

  /**@type import('@tanstack/react-table').ColumnDef<any> */
  const columns = [
    {
      header: "From",
      accessorKey: "price.fromTransportZone.name",
    },
    {
      header: "To",
      accessorKey: "price.toTransportZone.name",
    },
    {
      header: "Price",
      accessorKey: "price.price",
    },
    {
      header: "Currency",
      accessorKey: "price.currency.name", //shortName
    },
    {
      header: "Agent",
      accessorKey: "agent.businessUnit.name",
    },
    {
      header: "Type",
      accessorKey: "transportType.description",
    },
    {
      header: "Valid from",
      accessorKey: "price.validFrom",
    },
    {
      header: "Valid to",
      accessorKey: "price.validTo",
    },
    {
      header: "Description",
      accessorKey: "price.description",
    },
  ];

  const table = useReactTable({
    data,
    columns,
    initialState: {
      pagination: {
        pageSize: 150,
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

  const exportToExcel = () => {
    const ws = XLSX.utils.json_to_sheet(data);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, "Prices");
    XLSX.writeFile(wb, "Prices.xlsx");
  };

  const fetchPrices = async () => {
    try {
      const prices = await getAllPrices();
      setData(prices);
    } catch (error) {
      console.error('Error fetching prices: ', error.message);
    }
  };

  useEffect(() => {
    setSelectedColumns(columns);
    fetchPrices();
  }, []); // Empty dependency array so it only runs once

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

  return (
    <div className="text-dark-green text-sm w-full">
      <div className="overflow-auto relative">
        <table ref={tableRef} className="border-x border-b w-full">
          <thead className="border">
            {table.getHeaderGroups().map((headerGroup) => (
              <React.Fragment key={headerGroup.id}>
                <tr>
                  <th className="border p-2 bg-white relative">
                    <p>Buttons</p>
                  </th>
                  {headerGroup.headers.map((header) => (
                    <th
                      className="border p-2 bg-white relative"
                      key={header.id}
                      onClick={header.column.getToggleSortingHandler()}
                    >
                      {flexRender(header.column.columnDef.header, header.getContext())}
                      {({ asc: <LuChevronsUpDown />, desc: <LuChevronsUpDown /> })[
                        header.column.getIsSorted()
                      ]}
                    </th>
                  ))}
                </tr>
                <tr>
                  <th>
                    <SearchBar disabled></SearchBar>
                  </th>
                  {headerGroup.headers.map((header) => (
                    <th key={header.id}>
                      <SearchBar onFilterChange={(value) => handleColumnFilterChange(header.id, value)} />
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
                  <td className="border p-1 text-center">
                    <button className="appearance-none font-bold border rounded px-2 mr-10">Button 1</button>
                    <button className="appearance-none font-bold border rounded px-2 mr-10">Button 2</button>
                    <input type="checkbox" />
                  </td>
                  {row.getVisibleCells().map((cell) => (
                    <td className="border p-1 text-center" key={cell.id}>
                      {flexRender(cell.column.columnDef.cell, cell.getContext())}
                    </td>
                  ))}
                </tr>
              ))}
          </tbody>
        </table>
      </div>
      <div className="flex gap-2 justify-end mr-2">
        <button onClick={exportToExcel} className="flex items-center gap-2 text-2xl">
          <SiMicrosoftexcel />
          Excel {/*  Detta kan tas bort eller bytas mot en tydligare excel icon*/}
        </button>
      </div>
    </div>
  );
}
export default OrderTable;