import { baseUrl } from "../API";

const deletePrice = async (id) => {
  try {
    const response = await fetch(`${baseUrl}/Price/${id}`, {
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