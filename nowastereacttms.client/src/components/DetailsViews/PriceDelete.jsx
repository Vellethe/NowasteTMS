import React from 'react';

const PriceDeleteForm = ({ item, onDelete, onCancel }) => {
  const handleDelete = () => {
    onDelete(item);
  };

  return (
    <div className="fixed inset-0 flex justify-center items-center bg-gray-800 bg-opacity-50 z-50">
      <div className="bg-white p-6 rounded-lg w-96">
        <h2 className="text-lg font-semibold mb-6">Delete Price</h2>
        <p>Are you sure you want to delete this price?</p>
        <div className="flex justify-end mt-5">
          <button onClick={handleDelete} className="bg-red-500 text-white font-bold mr-3 rounded-md">Delete</button>
          <button onClick={onCancel} className="bg-gray-300 text-black font-bold rounded-md">Cancel</button>
        </div>
      </div>
    </div>
  );
};

export default PriceDeleteForm;