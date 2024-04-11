import { baseUrl } from "../API";

const getAllSupplier = async() => {
    try {
      const response = await fetch(`${baseUrl}/Supplier`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({Size: 400, Page: 0, Filter: {}, Column: {}}),
      });
      
      if (!response.ok) {
        throw new Error('Failed to fetch supplier');
      }
      const data = await response.json();
      return data;
    } catch (error) {
      throw new Error('Error fetching supplier: ' + error.message);
    }
  };
  
  export default getAllSupplier;