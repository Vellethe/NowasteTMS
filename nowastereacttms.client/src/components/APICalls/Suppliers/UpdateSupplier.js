var baseUrl ="https://localhost:7253/api";

const updateSupplier = async (id, updateSupplier) => {
    try {
      const response = await fetch(`${baseUrl}/Supplier/${id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(updateSupplier),
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
  
  export default updateSupplier;
  