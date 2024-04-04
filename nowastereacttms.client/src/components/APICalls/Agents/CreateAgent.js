var baseUrl ="https://localhost:7253/api";

const createAgent = async (agentData) => {
  try {
    const response = await fetch(`${baseUrl}/Agent`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(agentData),
    });

    if (!response.ok) {
      throw new Error('Failed to create agent');
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error('Error creating agent:', error);
    throw error;
  }
};

export default createAgent;