var baseUrl ="https://localhost:7253/api";

const createTransportOrder = async (transportOrderData) => {
  try {
    const response = await fetch(`${baseUrl}/Transport`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(transportOrderData),
    });

    if (!response.ok) {
      throw new Error('Failed to create transportorder');
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error('Error creating transportorder:', error);
    throw error;
  }
};

export default createTransportOrder;