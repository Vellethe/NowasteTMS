import { baseUrl } from "../API";

const updateOrder = async (id, updatedOrder) => {
    try {
      const response = await fetch(`${baseUrl}/Order/${id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(updatedOrder),
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
  
  export default updateOrder;
  