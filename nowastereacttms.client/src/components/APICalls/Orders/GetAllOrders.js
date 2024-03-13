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
  
  export default getAllOrders;