var baseUrl ="https://localhost:7253/api";

const getPalletTypes = async() => {
    try {
      const response = await fetch(`${baseUrl}/Order/PalletTypes`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(),
      });
      
      if (!response.ok) {
        throw new Error('Failed to fetch pallettypes');
      }
      const data = await response.json();
      return data;
    } catch (error) {
      throw new Error('Error fetching pallettypes: ' + error.message);
    }
  };

export default getPalletTypes;