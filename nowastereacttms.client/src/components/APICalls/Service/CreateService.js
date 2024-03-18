//import baseUrl from './API'
var baseUrl ="https://localhost:7253/api";

const createService = async (orderData) => {
  try {
    const response = await fetch(`${baseUrl}/Service/Create`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(orderData),
    });

    if (!response.ok) {
      throw new Error('Failed to create service');
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error('Error creating service:', error);
    throw error;
  }
};

export default createService;