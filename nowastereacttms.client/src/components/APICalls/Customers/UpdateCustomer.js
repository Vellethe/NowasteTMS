var baseUrl = "https://localhost:7253/api";

const updateCustomer = async (id, updatedCustomer) => {
  try {
    const response = await fetch(`${baseUrl}/Customer/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(updatedCustomer),
    });

    if (response.ok) {
      const data = await response.json();
      return data;
    } 
    else 
    {
      const errorData = await response.json();
      throw new Error('Failed to update order: ' + errorData.message);
    }
  } 
  catch (error) 
  {
    throw new Error('Error updating order: ' + error.message);
  }
};

export default updateCustomer;