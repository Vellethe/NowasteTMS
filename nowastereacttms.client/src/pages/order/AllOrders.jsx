import { Link, NavLink, useNavigate } from "react-router-dom";
import OrderTable from "../../components/table/OrderTable";



const AllOrders = () => {
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
      <OrderTable/>
    </div>
    </>
  );
}

export default AllOrders
