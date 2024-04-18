import { baseUrl } from "../API";

const updatePrice = async (id, updatePrice) => {
  try {
    const response = await fetch(`${baseUrl}/Price/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(updatePrice),
    });

    if (response.ok) {
      const data = await response.json();
      return data;
    } else {
      throw new Error('Failed to update price');
    }
  } catch (error) {
    console.error('Error updating price:', error.message);
    throw new Error('Error updating price: ' + error.message);
  }
};

export default updatePrice;