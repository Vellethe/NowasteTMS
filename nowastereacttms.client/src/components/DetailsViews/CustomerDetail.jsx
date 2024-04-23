import React from 'react';

const CustomerDisplayView = ({ item, onClose }) => {
  return (
    <div className="fixed inset-0 flex justify-center items-center bg-gray-800 bg-opacity-50 z-50">
      <div className="bg-white p-6 rounded-lg w-1/3">
        <h2 className="text-lg font-semibold mb-4">Customer Details</h2>
        <div>
          <div className="mb-2">
            <span className="font-semibold mr-2">Name:</span>
            <span>{item.businessUnit?.name || ''}</span>
          </div>
          <div className="mb-2">
            <span className="font-semibold mr-2">Country:</span>
            <span>{item.businessUnit.contactInformations[0].country || ''}</span>
          </div>
          <div className="mb-2">
            <span className="font-semibold mr-2">Currency:</span>
            <span>{item.businessUnit?.financeInformation.currency.shortName || ''}</span>
          </div>
          {/* Contact Information */}
          <h3 className="text-lg font-semibold mb-4 mt-4">Contact Information</h3>
          {item.businessUnit?.contactInformations?.map((contact, index) => (
            <div key={index} className="mb-4">
              <div className="mb-2">
                <span className="font-semibold mr-2">Phone:</span>
                <span>{contact.phone || ''}</span>
              </div>
              <div className="mb-2">
                <span className="font-semibold mr-2">Cellular Phone:</span>
                <span>{contact.cellularPhone || ''}</span>
              </div>
              <div className="mb-2">
                <span className="font-semibold mr-2">Email:</span>
                <span>{contact.email || ''}</span>
              </div>
              <div className="mb-2">
                <span className="font-semibold mr-2">Fax:</span>
                <span>{contact.fax || ''}</span>
              </div>
              <div className="mb-2">
                <span className="font-semibold mr-2">Address:</span>
                <span>{contact.address || ''}</span>
              </div>
              <div className="mb-2">
                <span className="font-semibold mr-2">Zipcode:</span>
                <span>{contact.zipcode || ''}</span>
              </div>
              <div className="mb-2">
                <span className="font-semibold mr-2">City:</span>
                <span>{contact.city || ''}</span>
              </div>
              <div className="mb-2">
                <span className="font-semibold mr-2">Country:</span>
                <span>{contact.country || ''}</span>
              </div>
            </div>
          ))}
        </div>
        <div className="flex justify-end mt-5">
          <button onClick={onClose} className="bg-gray-300 text-black font-bold rounded-md">Close</button>
        </div>
      </div>
    </div>
  );
};

export default CustomerDisplayView;