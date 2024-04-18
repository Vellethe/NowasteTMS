import { baseUrl } from "./API";

const fetchAllCurrencies = async () => {
    try {
      const response = await fetch(`${baseUrl}/Transport/currency`);
      if (!response.ok) {
        throw new Error('Failed to fetch all currencies');
      }
      const data = await response.json();
      return data;
    } catch (error) {
      console.error('Error fetching all currencies:', error);
      throw error;
    }
  };

  export default fetchAllCurrencies;