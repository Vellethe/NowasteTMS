import { baseUrl } from "../API";

const getAllAgents = async (includeInactive = false) => {
    try {
      const response = await fetch(`${baseUrl}/Agent?includeInactive=${includeInactive}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({Size: 400, Page: 0, Filter: {}, Column: {}}),
      });
      
      if (!response.ok) {
        throw new Error('Failed to fetch agents');
      }
      const data = await response.json();
      return data;
    } catch (error) {
      throw new Error('Error fetching agents: ' + error.message);
    }
  };
  
  export default getAllAgents;