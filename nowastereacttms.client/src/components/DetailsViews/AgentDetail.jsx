import React, { useState, useEffect } from 'react';
import getContactInformation from '../APICalls/Agents/GetContactInfoAgent';

const AgentDisplayView = ({ item, onClose }) => {
  const [contactInfo, setContactInfo] = useState([]);
  const [expandedContacts, setExpandedContacts] = useState([]);
  const [data, setData] =useState([]);

  useEffect(() => {
    const fetchContactInfo = async () => {
      try {
        const contactData = await getContactInformation(item.businessUnit?.businessUnitPK);
        setContactInfo(contactData);
        setExpandedContacts(Array(data.length).fill(true)); //false to show customers closed, true if you show them opened
      } catch (error) {
        console.error('Error fetching contact information:', error);
      }
    };

    fetchContactInfo();
  }, [item.businessUnit?.businessUnitPK]);

  const toggleContact = (index) => {
    setExpandedContacts((prevExpanded) => {
      const newExpanded = [...prevExpanded];
      newExpanded[index] = !newExpanded[index];
      return newExpanded;
    });
  };

  return (
    <div className="fixed inset-0 flex justify-center items-center bg-gray-800 bg-opacity-50 z-50">
      <div className="bg-white p-6 rounded-lg w-1/3">
        <h2 className="text-lg font-semibold mb-4">Agent Details</h2>
        <div>
          <div className="mb-2">
            <span className="font-semibold mr-2">Name:</span>
            <span>{item.businessUnit?.name || ''}</span>
          </div>
          <div className="mb-2">
            <span className="font-semibold mr-2">Self billing:</span>
            <span>{item.isSelfBilling ? 'Yes' : 'No'}</span>
          </div>
          <div className="mb-2">
            <span className="font-semibold mr-2">Country:</span>
            <span>{item.businessUnit?.contactInformations[0]?.country || ''}</span>
          </div>
          <div className="mb-2">
            <span className="font-semibold mr-2">Currency:</span>
            <span>{item.businessUnit?.financeInformation.currency.shortName || ''}</span>
          </div>

          <h3 className="text-lg font-semibold mt-4 mb-4">Contact Information</h3>
          {contactInfo.map((contact, index) => (
            <div key={index} className="mb-4">
              <div className="flex justify-between items-center font-semibold mb-2">
                <h4>Contact {index + 1}</h4>
                <button onClick={() => toggleContact(index)}>
                  {expandedContacts[index] ? 'Hide' : 'Show'}
                </button>
              </div>
              {expandedContacts[index] && (
                <div>
                  <div className="mb-2">
                    <span className="font-semibold mr-2">Phone:</span>
                    <span>{contact.phone || "None"}</span>
                  </div>
                  <div className="mb-2">
                    <span className="font-semibold mr-2">Cellular Phone:</span>
                    <span>{contact.cellularPhone || "None"}</span>
                  </div>
                  <div className="mb-2">
                    <span className="font-semibold mr-2">Email:</span>
                    <span>{contact.email || "None"}</span>
                  </div>
                  <div className="mb-2">
                    <span className="font-semibold mr-2">Fax:</span>
                    <span>{contact.fax || "None"}</span>
                  </div>
                  <div className="mb-2">
                    <span className="font-semibold mr-2">Address:</span>
                    <span>{contact.address || "None"}</span>
                  </div>
                  <div className="mb-2">
                    <span className="font-semibold mr-2">Zipcode:</span>
                    <span>{contact.zipcode || "None"}</span>
                  </div>
                  <div className="mb-2">
                    <span className="font-semibold mr-2">City:</span>
                    <span>{contact.city || "None"}</span>
                  </div>
                  <div className="mb-2">
                    <span className="font-semibold mr-2">Country:</span>
                    <span>{contact.country || "None"}</span>
                  </div>
                </div>
              )}
            </div>
          ))}
          <div className="flex justify-end">
            <button onClick={onClose} className="bg-gray-300 text-black font-bold rounded-md">Close</button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default AgentDisplayView;  