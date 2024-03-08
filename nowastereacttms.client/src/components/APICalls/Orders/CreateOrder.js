//import baseUrl from './API'
var baseUrl ="https://localhost:7253";

const createOrder = async (orderData) => {
  try {
    const response = await fetch(`${baseUrl}/Order`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(orderData),
    });

    if (!response.ok) {
      throw new Error('Failed to create order');
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error('Error creating order:', error);
    throw error;
  }
};

export default createOrder;