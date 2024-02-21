import { useMemo, useState, useEffect } from 'react';
import { Link } from "react-router-dom";
import OrderTable from "../../components/table/OrderTable";
import {
  MaterialReactTable,
  useMaterialReactTable,
} from 'material-react-table';
import { DisabledByDefault } from '@mui/icons-material';



const data = [
    {
        "Id": 3357,
        "Status": "Created",
        "Pickup from ": "23/10/2024 ",
        "Pickup to": "06/04/2023",
        "Pickup weekday": "Wednesday",
        "OrderId": "74854-037",
        "Supplier": "Brightdog",
        "From Country": "Poland",
        "Eur Pallets": 9,
        "Sea Pallets": 3,
        "Pallet exch": "Yes",
        "Item Id": "CXT-689-DB",
        "Item": "OnePlus 7 Pro",
        "Item Qty": 14,
        "Lines": 3,
        "ETA From": "28/01/2023",
        "ETA To": "30/03/2023",
        "ETA Weekday": "Monday",
        "Customer": "Jane Smith",
        "Adress": "123 Main St",
        "Item Company": 2,
        "Origin": "Canada",
        "Created": "24/06/2023",
        "Update from": "06/04/2023",
        "Update to": "20/03/2023",
        "Update weekday": "Monday",
        "Updated by": "John",
        "Transportbooking": "Yes"
    },
    {
      "Id": 4130,
      "Status": "Saved",
      "Pickup from ": "25/06/2024",
      "Pickup to": "04/03/2023",
      "Pickup weekday": "Tuesday",
      "OrderId": "79243-984",
      "Supplier": "Linktype",
      "From Country": "Indonesia",
      "Eur Pallets": 7,
      "Sea Pallets": 9,
      "Pallet exch": "No",
      "Item Id": "TEK-962-IU",
      "Item": "Google Pixel",
      "Item Qty": 20,
      "Lines": 6,
      "ETA From": "06/03/2023",
      "ETA To": "28/01/2023",
      "ETA Weekday": "Monday",
      "Customer": "David Wilson",
      "Adress": "987 Maple Blvd",
      "Item Company": 8,
      "Origin": "Germany",
      "Created": "16/11/2023",
      "Update from": "29/09/2023",
      "Update to": "05/06/2023",
      "Update weekday": "Saturday",
      "Updated by": "Mike",
      "Transportbooking": "No"
  },
  {
    "Id": 6681,
    "Status": "Created",
    "Pickup from ": "06/07/2024",
    "Pickup to": "09/03/2023",
    "Pickup weekday": "Friday",
    "OrderId": "38109-241",
    "Supplier": "Wordtune",
    "From Country": "Indonesia",
    "Eur Pallets": 5,
    "Sea Pallets": 9,
    "Pallet exch": "No",
    "Item Id": "SPJ-699-PE",
    "Item": "Motorola Moto G7",
    "Item Qty": 5,
    "Lines": 4,
    "ETA From": "13/10/2023",
    "ETA To": "11/12/2023",
    "ETA Weekday": "Tuesday",
    "Customer": "John Doe",
    "Adress": "987 Maple Blvd",
    "Item Company": 2,
    "Origin": "Germany",
    "Created": "09/02/2023",
    "Update from": "17/02/2023",
    "Update to": "22/08/2023",
    "Update weekday": "Tuesday",
    "Updated by": "Mike",
    "Transportbooking": "No"
},
{
    "Id": 7680,
    "Status": "Created",
    "Pickup from ": "31/01/2024",
    "Pickup to": "17/02/2023",
    "Pickup weekday": "Monday",
    "OrderId": "42708-652",
    "Supplier": "BlogXS",
    "From Country": "Rwanda",
    "Eur Pallets": 9,
    "Sea Pallets": 8,
    "Pallet exch": "Yes",
    "Item Id": "VUO-915-TZ",
    "Item": "Huawei P30",
    "Item Qty": 17,
    "Lines": 7,
    "ETA From": "26/08/2023",
    "ETA To": "12/07/2023",
    "ETA Weekday": "Saturday",
    "Customer": "Emily Davis",
    "Adress": "123 Main St",
    "Item Company": 4,
    "Origin": "Mexico",
    "Created": "26/09/2023",
    "Update from": "10/05/2023",
    "Update to": "27/05/2023",
    "Update weekday": "Tuesday",
    "Updated by": "John",
    "Transportbooking": "Yes"
}
];

const AllHistoricalOrders = () => {
  // const [data, setData] = useState([]);

  // useEffect(() => {
  //   fetchData();
  // }, []);

  // const fetchData = async () => {
  //   try {
  //     const response = await fetch(
  //       "https://transportservice-development.nowastelogistics.com/api/Order"
  //     );
  //     const orders = await response.json();
  //     setData(orders);
  //   } catch (error) {
  //     console.error("Error fetching data:", error);
  //   }
  // };

  return (
    <div className="m-7">
      <div className="text-3xl text-center mt-6 text-dark-green">Historical Orders</div>
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
      <OrderTable/>
    </div>
  );
};

export default AllHistoricalOrders;