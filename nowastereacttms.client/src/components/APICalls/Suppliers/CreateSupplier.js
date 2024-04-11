import { baseUrl } from "../API";

const createSupplier = async (supplierData) => {
  try {
    const response = await fetch(`${baseUrl}/Supplier`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(supplierData),
    });

    if (!response.ok) {
      throw new Error('Failed to create supplier');
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error('Error creating supplier:', error);
    throw error;
  }
};

export default createSupplier;