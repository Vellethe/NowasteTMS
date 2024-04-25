import TransportOrderTable from '../../components/table/TransportOrderTable'
import { Link } from "react-router-dom";

const AllHistoricalTransportOrders = () => {
  return (
    <div className="m-4">
      <div className="text-3xl text-center text-dark-green">Historical Transport Orders</div>
      <div className="text-xl flex justify-end text-medium-green">

        <div className="flex gap-3">
          <Link
            to="/Transport/TransportOrder/AllTransportOrders"
            className=" p-1 duration-200 hover:bg-medium-green hover:text-white rounded-lg"
          >
            Transport Orders
          </Link>
        </div>
      </div>
      <TransportOrderTable />
    </div>
  )
}

export default AllHistoricalTransportOrders