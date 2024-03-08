
import { BrowserRouter as Router, Routes, Route} from "react-router-dom";
import AllTransportOrder from './pages/transportOrder/AllTransportOrders';
import AllHistoricalOrders from './pages/order/AllHistoricalOrders';
import Create from './pages/order/Create';
import AllHistoricalTransportOrders from './pages/transportOrder/AllHistoricalTransportOrders';
import MasterLayout from './pages/MasterLayout/MasterLayout';
import AllOrders from './pages/order/AllOrders';
import Login from './pages/Identity/Account/Login';
import Agents from './pages/Transport/Agents';
import Customers from './pages/Transport/Customers';
import Prices from './pages/Transport/Prices';
import Suppliers from './pages/Transport/Suppliers';
import TransportOrderService from './pages/Transport/TransportOrderService';
import Home from "./pages/Home/Home";




function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/" element={<MasterLayout />}>
          <Route path="/Transport/Order/AllOrders" element={<AllOrders />} />
          
          <Route
            path="/Transport/Order/AllHistoricalOrders"
            element={<AllHistoricalOrders />}
          />
          <Route path="/Transport/Order/Create" element={<Create />} />
          <Route
            path="/Transport/TransportOrder/AllTransportOrders"
            element={<AllTransportOrder />}
          />
          <Route
            path="/Transport/TransportOrder/AllHistoricalTransportOrders"
            element={<AllHistoricalTransportOrders />}
          />
          <Route path="/Transport/TransportZonePrices" element={<Prices />} />
          <Route path="/Transport/Agents/AllAgents" element={<Agents />} />
          <Route
            path="/Transport/Customers/AllCustomers"
            element={<Customers />}
          />
          <Route
            path="/Transport/Suppliers/AllSuppliers"
            element={<Suppliers />}
          />
          <Route
            path="/Transport/TransportOrderService"
            element={<TransportOrderService />}
          />
        </Route>
        <Route path="/Identity/Account/Login" element={<Login />} />
      </Routes>
    </Router>
  );
}
export default App
