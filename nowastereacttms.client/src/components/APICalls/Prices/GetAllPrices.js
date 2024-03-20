var baseUrl ="https://localhost:7253/api";

const getAllPrices = async() => {
    try {
      const response = await fetch(`${baseUrl}/Price`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({Size: 400, Page: 0, Filter: {}, Column: {}}),
      });
      
      if (!response.ok) {
        throw new Error('Failed to fetch prices');
      }
      const data = await response.json();
      return data;
    } catch (error) {
      throw new Error('Error fetching prices: ' + error.message);
    }
  };
  
  export default getAllPrices;