import React, { useState, useEffect } from 'react';

const EditPriceForm = ({ item, onSave, onCancel }) => {
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
      <div className="bg-white p-4 rounded-lg">
        <h2 className="text-lg font-semibold mb-4">Edit Price</h2>
        <div>
          {/* Input fields for each property */}
          <label className="block mb-2">
            From:
            <input type="text" name="from" value={editedItem.from || ''} onChange={handleChange} className="border-gray-400 border rounded-md px-2 py-1 mt-1 block w-full" />
          </label>
          <label className="block mb-2">
            To:
            <input type="text" name="to" value={editedItem.to || ''} onChange={handleChange} className="border-gray-400 border rounded-md px-2 py-1 mt-1 block w-full" />
          </label>
          <label className="block mb-2">
            Price:
            <input type="text" name="price" value={editedItem.price || ''} onChange={handleChange} className="border-gray-400 border rounded-md px-2 py-1 mt-1 block w-full" />
          </label>
          <label className="block mb-2">
            Currency:
            <input type="text" name="currency" value={editedItem.currency || ''} onChange={handleChange} className="border-gray-400 border rounded-md px-2 py-1 mt-1 block w-full" />
          </label>
          <label className="block mb-2">
            Agent:
            <input type="text" name="agent" value={editedItem.agent || ''} onChange={handleChange} className="border-gray-400 border rounded-md px-2 py-1 mt-1 block w-full" />
          </label>
          <label className="block mb-2">
            Type:
            <input type="text" name="type" value={editedItem.type || ''} onChange={handleChange} className="border-gray-400 border rounded-md px-2 py-1 mt-1 block w-full" />
          </label>
          <label className="block mb-2">
            Valid from:
            <input type="text" name="validFrom" value={editedItem.validFrom || ''} onChange={handleChange} className="border-gray-400 border rounded-md px-2 py-1 mt-1 block w-full" />
          </label>
          <label className="block mb-2">
            Valid to:
            <input type="text" name="validTo" value={editedItem.validTo || ''} onChange={handleChange} className="border-gray-400 border rounded-md px-2 py-1 mt-1 block w-full" />
          </label>
          <label className="block mb-2">
            Description:
            <input type="text" name="description" value={editedItem.description || ''} onChange={handleChange} className="border-gray-400 border rounded-md px-2 py-1 mt-1 block w-full" />
          </label>
        </div>
        <div className="flex justify-end mt-4">
          <button onClick={handleSave} className="bg-blue-500 text-black px-4 py-2 mr-2 rounded-md">Save</button>
          <button onClick={onCancel} className="bg-gray-300 text-gray-700 px-4 py-2 rounded-md">Cancel</button>
        </div>
      </div>
    </div>
  );
};

export default EditPriceForm;