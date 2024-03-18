var baseUrl ="https://localhost:7253/api";

const updateService = async (id, updatedService) => {
    try {
      const response = await fetch(`${baseUrl}/Service/${id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(updatedService),
      });
  
      if (response.ok) {
        const data = await response.json();
        return data;
      } else {
        throw new Error('Failed to update service');
      }
    } catch (error) {
      throw new Error('Error updating service: ' + error.message);
    }
  };
  
  export default updateService;
  