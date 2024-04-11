import { baseUrl } from "../API";

const createCustomer = async (customerData) => {
  try {
    const response = await fetch(`${baseUrl}/Customer`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(customerData),
    });

    if (!response.ok) {
      throw new Error('Failed to create customer');
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error('Error creating customer:', error);
    throw error;
  }
};

export default createCustomer;