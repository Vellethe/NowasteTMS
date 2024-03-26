import React, { useState, useMemo, useEffect, useRef } from 'react'
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
import getAllSuppliers from '../APICalls/Suppliers/GetAllSuppliers';
import updateSupplier from '../APICalls/Suppliers/UpdateSupplier';
import EditSupplierForm from '../EditForms/EditSupplierForm';

const OrderTable = () => {
  const [selectedColumns, setSelectedColumns] = useState([]);
  const [data, setData] = useState([]);
  const [sorting, setSorting] = useState([]);
  const [editItem, setEditItem] = useState(null);
  const [isEditFormOpen, setIsEditFormOpen] = useState(false);

  /**@type import('@tanstack/react-table').ColumnDef<any> */
  const columns = [
    {
      header: "SupplierID",
      accessorKey: "supplierID",
    },
    {
      header: "Name",
      accessorKey: "businessUnit.name",
    },
    {
      header: "Country",
      accessorKey: "businessUnit.contactInformations.0.country",
    },
    {
      header: "Currency",
      accessorKey: "businessUnit.financeInformation.currency.shortName",
    },
  ];

  const handleEdit = (item) => {
    setEditItem(item);
    setIsEditFormOpen(true);
  };

  const handleSave = async (updatedItem) => {
    try {
      await updateSupplier(updatedItem.id, updatedItem);
      fetchSupplier();
      setIsEditFormOpen(false);
    } catch (error) {
      console.error('Error updating supplier: ', error.message);
    }
  };

  const handleCancel = () => {
    setIsEditFormOpen(false);
  };

  const fetchSupplier = async () => {
    try {
      const suppliers = await getAllSuppliers();
      setData(suppliers);
    } catch (error) {
      console.error('Error fetching suppliers: ', error.message);
    }
  };

  const table = useReactTable({
    data,
    columns,
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
    fetchSupplier();
  }, []); // Empty dependency array so it only runs once

  const exportToExcel = () => {
    const ws = XLSX.utils.json_to_sheet(data);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, "Suppliers");
    XLSX.writeFile(wb, "Suppliers.xlsx");
  };

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
            {isEditFormOpen && (
        <EditSupplierForm item={editItem} onSave={handleSave} onCancel={handleCancel} />
      )}
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
                <td className=" border-b p-1 text-center flex gap-2 truncate">
                  <input
                    className="accent-medium-green h-5 w-5 rounded-xl ml-1"
                    type="checkbox"
                  />
              <button className="appearance-none font-bold border rounded px-2 mr-5 ml-5" onClick={() => handleEdit(row.original)}>Edit</button>
                <button className="appearance-none font-bold border rounded px-2">Details</button>
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