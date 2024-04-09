var baseUrl ="https://localhost:7253";

const deletePrice = async (id) => {
  try {
    const response = await fetch(`${baseUrl}/Price/pk=${id}`, {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      throw new Error('Failed to delete resource');
    }
  } catch (error) {
    console.error('Error:', error);
  }
};

  export default deletePrice;