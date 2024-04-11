import { baseUrl } from "../API";

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
        throw new Error('Failed to update supplier');
      }
    } catch (error) {
      throw new Error('Error updating supplier: ' + error.message);
    }
  };
  
  export default updateSupplier;
  