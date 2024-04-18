import { baseUrl } from "./API";

const fetchAllLocations = async () => {
    try {
      const response = await fetch(`${baseUrl}/Transport/zones`);
      if (!response.ok) {
        throw new Error('Failed to fetch all locations');
      }
      const data = await response.json();
      return data;
    } catch (error) {
      console.error('Error fetching all locations:', error);
      throw error;
    }
  };

  export default fetchAllLocations;