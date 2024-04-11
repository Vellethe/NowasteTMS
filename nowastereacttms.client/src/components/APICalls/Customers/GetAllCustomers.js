import { baseUrl } from "../API";

const getAllCustomers = async() => {
    try {
      const response = await fetch(`${baseUrl}/Customer`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({Size: 999999, Page: 0, Filter: {}, Column: {}}),
      });
      
      if (!response.ok) {
        throw new Error('Failed to fetch customers');
      }
      const data = await response.json();
      return data;
    } catch (error) {
      throw new Error('Error fetching customers: ' + error.message);
    }
  };
  
  export default getAllCustomers;