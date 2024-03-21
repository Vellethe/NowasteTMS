var baseUrl ="https://localhost:7253/api";

const getAllOrders = async() => {
    try {
      const response = await fetch(`${baseUrl}/Order`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({Size: 400, Page: 0, Filter: {}, Column: {}}),
      });
      
      if (!response.ok) {
        throw new Error('Failed to fetch orders');
      }
      const data = await response.json();
      return data;
    } catch (error) {
      throw new Error('Error fetching orders: ' + error.message);
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
    const dayOfWeek = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'][datetime.getDay()];
    return `${dayOfWeek}, ${day}-${month}-${year} ${hours}:${minutes}:${seconds}`;
  }

  export default getAllOrders;