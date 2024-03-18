var baseUrl ="https://localhost:7253/api";

const getAllServices = async() => {
    try {
      const response = await fetch(`${baseUrl}/Service`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({Size: 400, Page: 0, Filter: {}, Column: {}}),
      });
      
      if (!response.ok) {
        throw new Error('Failed to fetch services');
      }
      const data = await response.json();
      return data;
    } catch (error) {
      throw new Error('Error fetching services: ' + error.message);
    }
  };
  
  export default getAllServices;