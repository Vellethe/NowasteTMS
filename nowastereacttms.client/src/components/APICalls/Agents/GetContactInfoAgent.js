import { baseUrl } from "../API";

const getContactInformation = async (id) => {
  try {
    const response = await fetch(`${baseUrl}/Agent/BusinessUnitPK?businessUnitPK=${id}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      throw new Error('Failed to fetch contact information');
    }

    const data = await response.json();
    return data;
  } catch (error) {
    throw new Error('Error fetching contact information: ' + error.message);
  }
};

export default getContactInformation;