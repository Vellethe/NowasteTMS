var baseUrl ="https://localhost:7253";

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
    throw new Error('Error updating price: ' + error.message);
  }
};

  export default updatePrice;