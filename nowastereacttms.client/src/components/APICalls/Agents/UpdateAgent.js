var baseUrl ="https://localhost:7253";

const updateAgent = async (id, updateAgent) => {
    try {
      const response = await fetch(`${baseUrl}/agent/${id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(updateAgent),
      });
  
      if (response.ok) {
        const data = await response.json();
        return data;
      } else {
        throw new Error('Failed to update order');
      }
    } catch (error) {
      throw new Error('Error updating order: ' + error.message);
    }
  };
  
  export default updateAgent;
  