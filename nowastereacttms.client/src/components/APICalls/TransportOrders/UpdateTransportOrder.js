import { baseUrl } from "../API";

const updateTransportOrder = async (id, updateSupplier) => {
    try {
      const response = await fetch(`${baseUrl}/Transport/${id}`, {
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
        throw new Error('Failed to update transportorder');
      }
    } catch (error) {
      throw new Error('Error updating transportorder: ' + error.message);
    }
  };
  
  export default updateTransportOrder;
  