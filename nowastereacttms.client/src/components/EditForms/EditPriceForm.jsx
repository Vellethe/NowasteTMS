import React, { useState, useEffect } from 'react';
import updatePrice from '../APICalls/Prices/UpdatePrice';

const EditPriceForm = ({ item, onSave, onCancel }) => {
  const [editedItem, setEditedItem] = useState({});

  // Update editedItem when item prop changes
  useEffect(() => {
    setEditedItem({ ...item });
  }, [item]);

  const handleSave = () => {
    const { price, validFrom, validTo, description } = editedItem.price;
  
    const formatDatetime = (datetime) => {
      const date = new Date(datetime);
      return date.toISOString();
    };
  
    const updatedData = {
      price: parseFloat(price),
      currencyPK: editedItem.price.currencyPK,
      validFrom: formatDatetime(validFrom),
      validTo: formatDatetime(validTo),
      fromTransportZonePK: editedItem.price.fromTransportZonePK,
      toTransportZonePK: editedItem.price.toTransportZonePK,
      transportTypePK: editedItem.price.transportTypePK,
      description: description,
      transportZonePricePK: editedItem.price.transportZonePricePK,
      agentPK: editedItem.agent?.agentPK || ''
    };
    
    return updatePrice(editedItem.price.transportZonePricePK, updatedData)
    .then(() => {
      window.alert("Price edited");
      window.location.reload();
    });
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    if (name === 'price' || name === 'validFrom' || name === 'validTo' || name === 'description') {
      let parsedValue = value;
      if (name === 'price') {
        parsedValue = parseFloat(value); // Parse to float
      }
      setEditedItem(prevState => ({
        ...prevState,
        price: {
          ...prevState.price,
          [name]: parsedValue
        }
      }));
    }
  };

  return (
    <div className="fixed inset-0 flex justify-center items-center bg-gray-800 bg-opacity-50 z-50">
      <div className="bg-white p-6 rounded-lg w-96">
        <h2 className="text-lg font-semibold mb-6">Edit Price</h2>
        <div>
          {/* Input fields for each property */}
          <label className="block mb-2">
            From:
            <input type="text" name="from" value={editedItem.price?.fromTransportZone.name || ''} disabled className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full" />
          </label>
          <label className="block mb-2">
            To:
            <input type="text" name="to" value={editedItem.price?.toTransportZone.name || ''} disabled className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full" />
          </label>
          <label className="block mb-2">
            Price:
            <input type="number" name="price" value={editedItem.price?.price || ''} onChange={handleChange} className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full" />
          </label>
          <label className="block mb-2">
            Currency:
            <input type="text" name="currency" value={editedItem.price?.currency.name || ''} disabled className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full" />
          </label>
          <label className="block mb-2">
            Agent:
            <input type="text" name="agent" value={editedItem.agent?.businessUnit.name || ''} disabled className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full" />
          </label>
          <label className="block mb-2">
            Type:
            <input type="text" name="type" value={editedItem.transportType?.description || ''} disabled className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full" />
          </label>
          <label className="block mb-2">
            Valid from:
            <input type="datetime-local" name="validFrom" value={editedItem.price?.validFrom || ''} onChange={handleChange} className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full" />
          </label>
          <label className="block mb-2">
            Valid to:
            <input type="datetime-local" name="validTo" value={editedItem.price?.validTo || ''} onChange={handleChange} className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full" />
          </label>
          <label className="block mb-2">
            Description:
            <input type="text" name="description" value={editedItem.price?.description || ''} onChange={handleChange} className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full" />
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

export default EditPriceForm;