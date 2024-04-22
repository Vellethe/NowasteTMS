import React, { useState, useEffect } from 'react';
import updateCustomer from '../APICalls/Customers/UpdateCustomer';

const EditCustomerForm = ({ item, onSave, onCancel }) => {
  const [editedItem, setEditedItem] = useState({});

  useEffect(() => {
    console.log("Item:", item);
    setEditedItem({ ...item });
  }, [item]);

  const handleSave = async () => {
    console.log("Edited ID:", editedItem);
    try {
      const updatedData = await updateCustomer(editedItem.id, editedItem);
      console.log("Updated ID:", updatedData);
      onSave(updatedData);
    } catch (error) {
      console.error('Error updating customer:', error.message);
    }
  };

  const handleChange = (e, index) => {
    const { name, value } = e.target;
    const updatedContactInfo = [...editedItem.businessUnit?.contactInformations];
    updatedContactInfo[index] = { ...updatedContactInfo[index], [name]: value };
    setEditedItem({
      ...editedItem,
      businessUnit: {
        ...editedItem.businessUnit,
        contactInformations: updatedContactInfo,
      },
    });
  };

  return (
    <div className="fixed inset-0 flex justify-center items-center bg-gray-800 bg-opacity-50 z-50">
      <div className="bg-white p-6 rounded-lg w-1/2">
        <h2 className="text-lg text-center font-semibold mb-6">Editing Customer: {editedItem.businessUnit?.name || ''}</h2>
        <div>
          <div className="flex justify-center w-full">
            <label className="w-1/2">
              Name:
              <input
                type="text"
                name="name"
                value={editedItem.businessUnit?.name || ''}
                disabled
                className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
              />
            </label>
          </div>
          <div className="flex justify-between mb-2">
            <label className="w-1/2 mr-2">
              Country:
              <input
                type="text"
                name="country"
                value={editedItem.businessUnit?.contactInformations[0]?.country || ''}
                disabled
                className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
              />
            </label>
            <label className="w-1/2 ml-2">
              Currency:
              <input
                type="text"
                name="currency"
                value={editedItem.businessUnit?.financeInformation.currency.shortName || ''}
                disabled
                className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
              />
            </label>
          </div>
          <h3 className="text-lg text-center font-semibold mb-4 mt-4">Contact Information</h3>
          <div>
            {editedItem.businessUnit?.contactInformations?.map((contact, index) => (
              <div key={index} className="mb-4">
                <div className="flex justify-between mb-2">
                  <label className="w-1/2 mr-2">
                    Phone:
                    <input
                      type="text"
                      name="phone"
                      value={contact.phone || ''}
                      onChange={(e) => handleChange(e, index)}
                      className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
                    />
                  </label>
                  <label className="w-1/2 ml-2">
                    Cellular Phone:
                    <input
                      type="text"
                      name="cellularPhone"
                      value={contact.cellularPhone || ''}
                      onChange={(e) => handleChange(e, index)}
                      className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
                    />
                  </label>
                </div>
                <div className="flex justify-between mb-2">
                  <label className="w-1/2 mr-2">
                    Email:
                    <input
                      type="text"
                      name="email"
                      value={contact.email || ''}
                      onChange={(e) => handleChange(e, index)}
                      className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
                    />
                  </label>
                  <label className="w-1/2 ml-2">
                    Fax:
                    <input
                      type="text"
                      name="fax"
                      value={contact.fax || ''}
                      onChange={(e) => handleChange(e, index)}
                      className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
                    />
                  </label>
                </div>
                <div className="flex justify-between mb-2">
                  <label className="w-1/2 mr-2">
                    Address:
                    <input
                      type="text"
                      name="address"
                      value={contact.address || ''}
                      onChange={(e) => handleChange(e, index)}
                      className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
                    />
                  </label>
                  <label className="w-1/2 ml-2">
                    Zipcode:
                    <input
                      type="text"
                      name="zipcode"
                      value={contact.zipcode || ''}
                      onChange={(e) => handleChange(e, index)}
                      className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
                    />
                  </label>
                </div>
                <div className="flex justify-between mb-2">
                  <label className="w-1/2 mr-2">
                    City:
                    <input
                      type="text"
                      name="city"
                      value={contact.city || ''}
                      onChange={(e) => handleChange(e, index)}
                      className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
                    />
                  </label>
                  <label className="w-1/2 ml-2">
                    Country:
                    <input
                      type="text"
                      name="country"
                      value={contact.country || ''}
                      onChange={(e) => handleChange(e, index)}
                      className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
                    />
                  </label>
                </div>
              </div>
            ))}
          </div>
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