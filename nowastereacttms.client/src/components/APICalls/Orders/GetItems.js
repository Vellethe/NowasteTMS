import { baseUrl } from "../API";

const getItems = async() => {
    try {
      const response = await fetch(`${baseUrl}/Order/Items`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(),
      });
      
      if (!response.ok) {
        throw new Error('Failed to fetch items');
      }
      const data = await response.json();
      return data;
    } catch (error) {
      throw new Error('Error fetching itemss: ' + error.message);
    }
  };

export default getItems;

