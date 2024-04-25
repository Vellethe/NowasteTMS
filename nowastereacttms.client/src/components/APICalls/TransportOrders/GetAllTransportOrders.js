import { baseUrl } from "../API";

// const getAllTransportOrders = async() => {
//     try {
//       const response = await fetch(`${baseUrl}/Transport`, {
//         method: 'POST',
//         headers: {
//           'Content-Type': 'application/json',
//         },
//         body: JSON.stringify({Size: 999999, Page: 0, Filter: {}, Column: {}, "historical": true }),
//       });
      
//       if (!response.ok) {
//         throw new Error('Failed to fetch transportorders');
//       }
//       const data = await response.json();

//       return data;
//     } catch (error) {
//       throw new Error('Error fetching transportorders: ' + error.message);
//     }
//   };

//   function formatDatetime(datetimeString) {
//     const datetime = new Date(datetimeString);
//     const day = datetime.getDate().toString().padStart(2, '0');
//     const month = (datetime.getMonth() + 1).toString().padStart(2, '0');
//     const year = datetime.getFullYear();
//     const hours = datetime.getHours().toString().padStart(2, '0'); 
//     const minutes = datetime.getMinutes().toString().padStart(2, '0');
//     const seconds = datetime.getSeconds().toString().padStart(2, '0'); 
//     return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
//   }
  
//   export default getAllTransportOrders;

const getAllTransportOrders = async() => {
  try {
    console.log('Fetching transport orders...');
    const response = await fetch(`${baseUrl}/Transport`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({Size: 999999, Page: 0, Filter: {}, Column: {}, "historical": true }),
    });

    console.log('Response status:', response.status);

    if (!response.ok) {
      throw new Error('Failed to fetch transport orders');
    }

    const data = await response.json();

    console.log('Received data:', data);

    return data;
  } catch (error) {
    console.error('Error fetching transport orders:', error.message);
    throw new Error('Error fetching transport orders: ' + error.message);
  }
};

export default getAllTransportOrders;