import { baseUrl } from "../API";

const updateAgent = async (id, updateAgent) => {
  try {
    const response = await fetch(`${baseUrl}/Agent/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(updateAgent),
    });

    if (response.ok) {
      const data = await response.json();
      return data;
    } else {
      throw new Error('Failed to update agent');
    }
  } catch (error) {
    throw new Error('Error updating agent: ' + error.message);
  }
};

  export default updateAgent;