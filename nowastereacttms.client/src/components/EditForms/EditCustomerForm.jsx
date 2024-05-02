import React, { useState, useEffect } from 'react';
import updateCustomer from '../APICalls/Customers/UpdateCustomer';
import getContactInformation from '../APICalls/Agents/GetContactInfoAgent'

const EditCustomerForm = ({ item, onSave, onCancel }) => {
  const [editedItem, setEditedItem] = useState({});
  const [contactInfo, setContactInfo] = useState(null);
  const [expandedContacts, setExpandedContacts] = useState([]);

  useEffect(() => {
    const fetchContactInfo = async () => {
      try {
        const data = await getContactInformation(item.businessUnit.businessUnitPK);
        setContactInfo(data);
        setExpandedContacts(Array(data.length).fill(true)); //false to show customers closed, true if you show them opened
        setEditedItem(prevItem => ({
          ...prevItem,
          businessUnit: {
            ...prevItem.businessUnit,
            contactInformations: data
          }
        }))
      } catch (error) {
        console.error("Error fetching contact information". error.message);
      }
    };
    fetchContactInfo();
    setEditedItem({ ...item });
  }, [item]);

  const handleSave = async () => {
    console.log("Edited ID:", editedItem);
    console.log ("Edited ID:", editedItem.customerPK);
    try {
      const updatedData = await updateCustomer(editedItem.customerPK, editedItem);
      onSave(updatedData); //Failed to update order: undefined (funkar ändå)
    } catch (error) {
      console.error('Error updating customer:', error.message);
    }
  };

  const toggleContact = (index) => {
    setExpandedContacts((prevExpanded) => {
      const newExpanded = [...prevExpanded];
      newExpanded[index] = !newExpanded[index];
      return newExpanded;
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
          <h3 className="text-lg text-center font-semibold mt-4 mb-4">Contact Information</h3>
          {contactInfo?.map((contact, index) => (
            <div key={index} className="mb-4">
              <div className="flex justify-between items-center font-semibold mb-2">
                <h4>Contact {index + 1}</h4>
                <button onClick={() => toggleContact(index)}>
                  {expandedContacts[index] ? 'Hide' : 'Show'}
                </button>
              </div>
              {expandedContacts[index] && (
                <div>
                  <div className="flex justify-between mb-2">
                    <label className="w-1/2 mr-2">
                      Phone:
                      <input
                        type="text"
                        value={contact.phone || ''}
                        onChange={(e) => {
                          const userInput = e.target.value;
                          if (/^[0-9+]*$/.test(userInput)) {
                            const newContactInfo = [...contactInfo];
                            newContactInfo[index].phone = userInput;
                            setEditedItem((prevItem) => ({
                              ...prevItem,
                              businessUnit: {
                                ...prevItem.businessUnit,
                                contactInfo: newContactInfo,
                              },
                            }));
                          }
                        }}
                        className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
                      />
                    </label>
                    <label className="w-1/2 ml-2">
                      Cellular Phone:
                      <input
                        type="text"
                        value={contact.cellularPhone || ''}
                        onChange={(e) => {
                          const userInput = e.target.value;
                          if (/^[0-9+]*$/.test(userInput)) {
                          const newContactInfo = [...contactInfo];
                          newContactInfo[index].cellularPhone = e.target.value;
                          setEditedItem((prevItem) => ({
                            ...prevItem,
                            businessUnit: {
                              ...prevItem.businessUnit,
                              contactInfo: newContactInfo,
                            },
                          }));
                        }
                      }}
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
                        onChange={(e) => {
                          const newContactInfo = [...contactInfo];
                          newContactInfo[index].email = e.target.value;
                          setEditedItem((prevItem) => ({
                            ...prevItem,
                            businessUnit: {
                              ...prevItem.businessUnit,
                              contactInfo: newContactInfo,
                            },
                          }));
                        }}
                        className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
                      />
                    </label>
                    <label className="w-1/2 ml-2">
                      Fax:
                      <input
                        type="text"
                        value={contact.fax || ''}
                        onChange={(e) => {
                          const newContactInfo = [...contactInfo];
                          newContactInfo[index].fax = e.target.value;
                          setEditedItem((prevItem) => ({
                            ...prevItem,
                            businessUnit: {
                              ...prevItem.businessUnit,
                              contactInfo: newContactInfo,
                            },
                          }));
                        }}
                        className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
                      />
                    </label>
                  </div>
                  <div className="flex justify-between mb-2">
                    <label className="w-1/2 mr-2">
                      Address:
                      <input
                        type="text"
                        value={contact.address || ''}
                        onChange={(e) => {
                          const newContactInfo = [...contactInfo];
                          newContactInfo[index].address = e.target.value;
                          setEditedItem((prevItem) => ({
                            ...prevItem,
                            businessUnit: {
                              ...prevItem.businessUnit,
                              contactInfo: newContactInfo,
                            },
                          }));
                        }}
                        className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
                      />
                    </label>
                    <label className="w-1/2 ml-2">
                      Zipcode:
                      <input
                        type="text"
                        value={contact.zipcode || ''}
                        onChange={(e) => {
                          const newContactInfo = [...contactInfo];
                          newContactInfo[index].zipcode = e.target.value;
                          setEditedItem((prevItem) => ({
                            ...prevItem,
                            businessUnit: {
                              ...prevItem.businessUnit,
                              contactInfo: newContactInfo,
                            },
                          }));
                        }}
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
                        onChange={(e) => {
                          const newContactInfo = [...contactInfo];
                          newContactInfo[index].city = e.target.value;
                          setEditedItem((prevItem) => ({
                            ...prevItem,
                            businessUnit: {
                              ...prevItem.businessUnit,
                              contactInfo: newContactInfo,
                            },
                          }));
                        }}
                        className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
                      />
                    </label>
                    <label className="w-1/2 ml-2">
                      Country:
                      <input
                        type="text"
                        value={contact.country || ''}
                        onChange={(e) => {
                          const newContactInfo = [...contactInfo];
                          newContactInfo[index].country = e.target.value;
                          setEditedItem((prevItem) => ({
                            ...prevItem,
                            businessUnit: {
                              ...prevItem.businessUnit,
                              contactInfo: newContactInfo,
                            },
                          }));
                        }}
                        className="border-gray-400 border rounded-md px-4 py-2 mt-1 block w-full"
                      />
                    </label>
                  </div>
                </div>
              )}
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

export default EditCustomerForm;