import React, { useState, useEffect } from 'react';

const EditCustomerForm = ({ item, onSave, onCancel }) => {
  const [editedItem, setEditedItem] = useState({});

  // Update editedItem when item prop changes
  useEffect(() => {
    setEditedItem({ ...item });
  }, [item]);

  const handleSave = () => {
    onSave(editedItem);
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setEditedItem({ ...editedItem, [name]: value });
  };

  return (
    <div className="fixed inset-0 flex justify-center items-center bg-gray-800 bg-opacity-50 z-50">
      <div className="bg-white p-6 rounded-lg w-96">
        <h2 className="text-lg font-semibold mb-6">Edit Customer</h2>
        <div>
          {/* Input fields for each property */}
          <label className="block mb-2">
            Name:
            <input type="text" name="name" value={editedItem.businessUnit?.name || ''} onChange={handleChange} className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full" />
          </label>
          <label className="block mb-2">
            Country:
            <input type="text" name="country" value={editedItem.country || ''} onChange={handleChange} className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full" />
          </label>
          <label className="block mb-2">
            Currency:
            <input type="text" name="currency" value={editedItem.businessUnit?.financeInformation.currency.shortName || ''} onChange={handleChange} className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full" />
          </label>
        </div>
        <div className="flex justify-end mt-5">
          <button onClick={handleSave} className="bg-blue-500 text-black font-bold mr-5 rounded-md">Save</button>
          <button onClick={onCancel} className="bg-gray-300 text-black font-bold rounded-md">Cancel</button>
        </div>
      </div>
    </div>
  );
};

export default EditCustomerForm;