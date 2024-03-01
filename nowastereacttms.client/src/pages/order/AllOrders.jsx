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
      <div className="m-7">
        <div className="text-3xl text-center mt-6 text-dark-green">Orders</div>

        <div className=" m-3 text-xl flex justify-end  text-medium-green">
          <div className="flex gap-3">
            <Link
              to="/Transport/Order/AllHistoricalOrders"
              className=" p-1 duration-200 hover:bg-medium-green hover:text-white rounded-lg"
            >
              Historical Orders
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

        <div className="flex gap-2 justify-between mr-7">
          <Link
            to="/Transport/Order/Create"
            className=" p-2 duration-300 hover:bg-medium-green hover:text-white rounded-lg mb-5 ml-6 text-2xl border border-medium-green "
          >
            Edit ETA
          </Link>
          <button
            onClick={exportToExcel}
            className="flex items-center gap-2 text-2xl hover:text-3xl duration-200"
          >
            <SiMicrosoftexcel />
            Excel{" "}
            {/*  Detta kan tas bort eller bytas mot en tydligare excel icon*/}
          </button>
        </div>
      </div>
    </>
  );
}

export default AllOrders
