import React, { useState, useEffect, useRef } from 'react'
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
import getAllServices from '../APICalls/Service/GetAllServices';
import updateService from '../APICalls/Service/UpdateService';
import EditServiceForm from '../EditForms/EditServiceForm';
import ServiceDisplayView from '../DetailsViews/ServiceDetail';

const ServiceTable = () => {
  const [selectedColumns, setSelectedColumns] = useState([]);
  const [data, setData] = useState([])
  const [sorting, setSorting] = useState([]);
  const [columnFilters, setColumnFilters] = useState({});
  const [editItem, setEditItem] = useState(null);
  const [isEditFormOpen, setIsEditFormOpen] = useState(false);
  const [detailsItem, setDetailsItem] = useState(null);
  const [isDetailsViewOpen, setIsDetailsViewOpen] = useState(false);

  /**@type import('@tanstack/react-table').ColumnDef<any> */
  const columns = [
    {
      header: "Name",
      accessorKey: "name",
    },
    {
      header: "Agent",
      accessorKey: "",
    },
    {
      header: "Price",
      accessorKey: "price",
    },
    {
      header: "Currency",
      accessorKey: "currency.name",
    },
    {
        header: "Currency",
        accessorKey: "currency.shortName",
      },
    {
      header: "Created/Updated",
      accessorKey:"timestamp",
    }
  ];

  const handleDetails = (item) => {
    setDetailsItem(item);
    setIsDetailsViewOpen(true);
  };

  const closeDetailsView = () => {
    setIsDetailsViewOpen(false);
  };

  const handleEdit = (item) => {
    console.log(item);
    setEditItem(item);
    setIsEditFormOpen(true);
  };

  const handleSave = async (updatedItem) => {
    try {
      await updateService(updatedItem.id, updatedItem);
      fetchServices();
      setIsEditFormOpen(false);
    } catch (error) {
      console.error('Error updating service: ', error.message);
    }
  };

  const handleCancel = () => {
    setIsEditFormOpen(false);
  };

  const fetchServices = async () => {
    try {
      const services = await getAllServices();
      setData(services);
    } catch (error) {
      console.error('Error fetching services: ', error.message);
    }
  };

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
    fetchServices();
  }, []); // Empty dependency array so it only runs once

  const exportToExcel = () => {
    const ws = XLSX.utils.json_to_sheet(data);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, "Services");
    XLSX.writeFile(wb, "Services.xlsx");
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
      {isEditFormOpen && (
        <EditServiceForm item={editItem} onSave={handleSave} onCancel={handleCancel} />
      )}
      {isDetailsViewOpen && (
       <ServiceDisplayView item={detailsItem} onClose={closeDetailsView} />
      )}
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
                  <button className="appearance-none font-bold border rounded px-2 mr-5 ml-5 cursor-pointer" onClick={() => handleEdit(row.original)}>Edit</button>
                  <button className="appearance-none font-bold border rounded px-2 cursor-pointer" onClick={() => handleDetails(row.original)}>Details</button>
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
export default ServiceTable;