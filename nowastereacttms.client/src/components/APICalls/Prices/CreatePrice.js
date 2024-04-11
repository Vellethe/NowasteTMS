import { baseUrl } from "../API";

const createPrice = async (orderData) => {
  try {
    const response = await fetch(`${baseUrl}/Price/Create`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(orderData),
    });

    if (!response.ok) {
      throw new Error('Failed to create price');
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error('Error creating price:', error);
    throw error;
  }
};

export default createPrice;
