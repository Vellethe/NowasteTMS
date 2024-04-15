import React from 'react';

const PriceDeleteForm = ({ item, onDelete, onCancel }) => {
  const { agent, price, currency } = item;
  const agentName = agent.businessUnit.name;
  const priceName = price.price;
  const currencyName = price.currency.name;
  const handleDelete = async () => {
    await onDelete(item);
    alert("Delete successful");
  };

  return (
    <div className="fixed inset-0 flex justify-center items-center bg-gray-800 bg-opacity-50 z-50">
      <div className="bg-white p-6 rounded-lg w-96">
        <h2 className="text-lg font-semibold mb-6">Delete Price</h2>
        <p>Are you sure you want to delete the following price?</p>
        <div className="mt-4">
          <p><strong>Agent:</strong> {String(agentName)}</p>
          <p><strong>Price:</strong> {String(priceName)}</p>
          <p><strong>Currency:</strong> {String(currencyName)}</p>
        </div>
        <div className="flex justify-end mt-5">
          <button onClick={handleDelete} className="bg-red-500 text-black font-bold mr-3 rounded-md">Delete</button>
          <button onClick={onCancel} className="bg-gray-300 text-black font-bold rounded-md">Cancel</button>
        </div>
      </div>
    </div>
  );
};

export default PriceDeleteForm;