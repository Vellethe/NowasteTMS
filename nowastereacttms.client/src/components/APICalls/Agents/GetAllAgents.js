var baseUrl ="https://localhost:7253";

const getAllAgents = async (includeInactive = false) => {
    try {
      const response = await fetch(`${baseUrl}/agents?includeInactive=${includeInactive}`);
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