import { baseUrl } from "../API";

const getOrderById = async (pk) => {
  try {
    const response = await fetch(`${baseUrl}/Order/${pk}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    if (response.ok) {
      const data = await response.json();
      return data;
    } else {
      throw new Error('Failed to fetch order');
    }
  } catch (error) {
    throw new Error('Error fetching order: ' + error.message);
  }
};

export default getOrderById;