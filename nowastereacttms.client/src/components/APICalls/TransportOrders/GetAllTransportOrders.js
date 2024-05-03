import { baseUrl } from "../API";

const getAllTransportOrders = async () => {
  try {
    console.log('Fetching transport orders...');
    const response = await fetch(`${baseUrl}/Transport`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ Size: 999999, Page: 0, Filter: {}, Column: {}, "historical": true }),
    });

    console.log('Response status:', response.status);

    if (!response.ok) {
      throw new Error('Failed to fetch transport orders');
    }

    const data = await response.json();

    console.log('Received data:', data);

    data.transportOrders = data.transportOrders.map(o => {
      // Format datetime fields
      o.collectionDate = formatDatetime(o.collectionDate);
      o.deliveryDate = formatDatetime(o.deliveryDate);
      o.arrivalDate = formatDatetime(o.arrivalDate);
      o.created = formatDatetime(o.created);
      o.updated = formatDatetime(o.updated);

      // Calculate weekdays
      o.collectionDateWD = getWeekdayFromDate(o.collectionDate);
      o.deliveryDateWD = getWeekdayFromDate(o.deliveryDate);
      o.updatedWD = getWeekdayFromDate(o.updated);
      // Add more fields if needed
      return o;
    });

    return data;
  } catch (error) {
    console.error('Error fetching transport orders:', error.message);
    throw new Error('Error fetching transport orders: ' + error.message);
  }
};

function formatDatetime(datetimeString) {
  const datetime = new Date(datetimeString);
  const day = datetime.getDate().toString().padStart(2, '0');
  const month = (datetime.getMonth() + 1).toString().padStart(2, '0');
  const year = datetime.getFullYear();
  const hours = datetime.getHours().toString().padStart(2, '0');
  const minutes = datetime.getMinutes().toString().padStart(2, '0');
  const seconds = datetime.getSeconds().toString().padStart(2, '0');
  return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
}

function getWeekdayFromDate(dateString) {
  const daysOfWeek = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
  const date = new Date(dateString);
  const weekdayIndex = date.getDay();
  return daysOfWeek[weekdayIndex];
}

export default getAllTransportOrders;