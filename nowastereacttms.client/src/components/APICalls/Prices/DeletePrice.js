var baseUrl ="https://localhost:7253";

const deletePrice = async (id) => {
  try {
    const response = await fetch(`${baseUrl}/Price/${id}`, {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(deletePrice),
    });

    if (!response.ok) {
        throw new Error('Failed to delete resource');
      }
      
      const data = await response.json();
      console.log(data); // "Deleted successfully"
    } catch (error) {
      console.error('Error:', error);
    }
  };

  export default deletePrice;