import { Link } from "react-router-dom";
import OrderTable from "../../components/table/OrderTable";
import * as XLSX from "xlsx";
import { SiMicrosoftexcel } from "react-icons/si";

const AllOrders = () => {

  const exportToExcel = () => {
    const ws = XLSX.utils.json_to_sheet(data);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, "Orders");
    XLSX.writeFile(wb, "Orders.xlsx");
  };
  return (
    <>
      <div className="m-4">
        <div className="text-3xl text-center text-dark-green">Orders</div>
        <div className=" m-3 text-xl flex justify-end  text-medium-green">

<div className="flex gap-3">
  <Link
    to="/Transport/Order/AllOrders"
    className=" p-1 duration-200 hover:bg-medium-green hover:text-white rounded-lg"
  >
    Orders
  </Link>
  <Link
    to="/Transport/Order/Create"
    className=" p-1 duration-200 hover:bg-medium-green hover:text-white rounded-lg"
  >
    Create New
  </Link>
</div>
</div>
        <OrderTable />
      </div>
      <div>
        <hr className="m-4"></hr>
        <div className="flex justify-between">
  <Link
    to="/Transport/Order/Create"
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
    </>
  );
}

export default AllOrders
