import React, { useState, useEffect } from 'react';
import updateAgent from '../APICalls/Agents/UpdateAgent';

const EditAgentsForm = ({ item, onSave, onCancel }) => {
  const [editedItem, setEditedItem] = useState({});

  useEffect(() => {
    console.log("Item:", item);
    setEditedItem({ ...item });
  }, [item]);

  const handleSave = async () => {
    console.log("Edited ID:", editedItem);
    try {
      const updatedData = await updateAgent(editedItem.id, editedItem);
      console.log("Updated ID:", updatedData);
      onSave(updatedData);
    } catch (error) {
      console.error('Error updating agent:', error.message);
    }
  };

  return (
    <div className="fixed inset-0 flex justify-center items-center bg-gray-800 bg-opacity-50 z-50">
      <div className="bg-white p-6 rounded-lg w-1/2">
        <h2 className="text-lg text-center font-semibold mb-6">Editing Agent: {editedItem.businessUnit?.name || ''}</h2>
        <div>
        <div className="flex justify-between mb-2">
        <label className="w-1/2 mr-2">
            Name:
            <input
              type="text"
              name="name"
              value={editedItem.businessUnit?.name || ''}
              disabled
              className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
            />
          </label>
          <label className="w-1/2 ml-2">
            Self billing:
            <input
              type="text"
              name="selfbilling"
              value={editedItem.isSelfBilling !== undefined ? editedItem.isSelfBilling ? 'Yes' : 'No' : ''}
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
          <h3 className="text-lg text-center font-semibold mt-4 mb-4">Contact Information</h3>
          {editedItem.businessUnit?.contactInformations?.map((contact, index) => (
            <div key={index} className="mb-4">
              <div className="flex justify-between mb-2">
                <label className="w-1/2 mr-2">
                  Phone:
                  <input
                    type="text"
                    value={contact.phone || ''}
                    onChange={(e) => setEditedItem({
                      ...editedItem,
                      businessUnit: {
                        ...editedItem.businessUnit,
                        contactInformations: [
                          ...editedItem.businessUnit.contactInformations.slice(0, index),
                          { ...contact, phone: e.target.value },
                          ...editedItem.businessUnit.contactInformations.slice(index + 1),
                        ],
                      },
                    })}
                    className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
                  />
                </label>
                <label className="w-1/2 ml-2">
                  Cellular Phone:
                  <input
                    type="text"
                    value={contact.cellularPhone || ''}
                    onChange={(e) => setEditedItem({
                      ...editedItem,
                      businessUnit: {
                        ...editedItem.businessUnit,
                        contactInformations: [
                          ...editedItem.businessUnit.contactInformations.slice(0, index),
                          { ...contact, cellularPhone: e.target.value },
                          ...editedItem.businessUnit.contactInformations.slice(index + 1),
                        ],
                      },
                    })}
                    className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
                  />
                </label>
              </div>
              <div className="flex justify-between mb-2">
                <label className="w-1/2 mr-2">
                  Email:
                  <input
                    type="text"
                    value={contact.email || ''}
                    onChange={(e) => setEditedItem({
                      ...editedItem,
                      businessUnit: {
                        ...editedItem.businessUnit,
                        contactInformations: [
                          ...editedItem.businessUnit.contactInformations.slice(0, index),
                          { ...contact, email: e.target.value },
                          ...editedItem.businessUnit.contactInformations.slice(index + 1),
                        ],
                      },
                    })}
                    className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
                  />
                </label>
                <label className="w-1/2 ml-2">
                  Fax:
                  <input
                    type="text"
                    value={contact.fax || ''}
                    onChange={(e) => setEditedItem({
                      ...editedItem,
                      businessUnit: {
                        ...editedItem.businessUnit,
                        contactInformations: [
                          ...editedItem.businessUnit.contactInformations.slice(0, index),
                          { ...contact, fax: e.target.value },
                          ...editedItem.businessUnit.contactInformations.slice(index + 1),
                        ],
                      },
                    })}
                    className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
                  />
                </label>
              </div>
              {/* Additional fields grouped similarly */}
              <div className="flex justify-between mb-2">
                <label className="w-1/2 mr-2">
                  Address:
                  <input
                    type="text"
                    value={contact.address || ''}
                    onChange={(e) => setEditedItem({
                      ...editedItem,
                      businessUnit: {
                        ...editedItem.businessUnit,
                        contactInformations: [
                          ...editedItem.businessUnit.contactInformations.slice(0, index),
                          { ...contact, address: e.target.value },
                          ...editedItem.businessUnit.contactInformations.slice(index + 1),
                        ],
                      },
                    })}
                    className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
                  />
                </label>
                <label className="w-1/2 ml-2">
                  Zipcode:
                  <input
                    type="text"
                    value={contact.zipcode || ''}
                    onChange={(e) => setEditedItem({
                      ...editedItem,
                      businessUnit: {
                        ...editedItem.businessUnit,
                        contactInformations: [
                          ...editedItem.businessUnit.contactInformations.slice(0, index),
                          { ...contact, zipcode: e.target.value },
                          ...editedItem.businessUnit.contactInformations.slice(index + 1),
                        ],
                      },
                    })}
                    className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
                  />
                </label>
              </div>
              <div className="flex justify-between mb-2">
                <label className="w-1/2 mr-2">
                  City:
                  <input
                    type="text"
                    value={contact.city || ''}
                    onChange={(e) => setEditedItem({
                      ...editedItem,
                      businessUnit: {
                        ...editedItem.businessUnit,
                        contactInformations: [
                          ...editedItem.businessUnit.contactInformations.slice(0, index),
                          { ...contact, city: e.target.value },
                          ...editedItem.businessUnit.contactInformations.slice(index + 1),
                        ],
                      },
                    })}
                    className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
                  />
                </label>
                <label className="w-1/2 ml-2">
                  Country:
                  <input
                    type="text"
                    value={contact.country || ''}
                    onChange={(e) => setEditedItem({
                      ...editedItem,
                      businessUnit: {
                        ...editedItem.businessUnit,
                        contactInformations: [
                          ...editedItem.businessUnit.contactInformations.slice(0, index),
                          { ...contact, country: e.target.value },
                          ...editedItem.businessUnit.contactInformations.slice(index + 1),
                        ],
                      },
                    })}
                    className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
                  />
                </label>
              </div>
            </div>
          ))}
        </div>
        <div className="flex justify-end mt-5">
          <button onClick={handleSave} className="bg-blue-500 text-black font-bold mr-5 rounded-md">Save</button>
          <button onClick={onCancel} className="bg-gray-300 text-black font-bold rounded-md">Cancel</button>
        </div>
      </div>
    </div>
  );
};

export default EditAgentsForm;