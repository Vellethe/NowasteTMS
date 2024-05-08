import React from 'react';
//Show supplierName, Company, Phone, CellularPhone, Email, Fax, Address, Zipcode, City, Country

const SupplierDisplayView = ({ item, onClose }) => {
  return (
    <div className="fixed inset-0 flex justify-center items-center bg-gray-800 bg-opacity-50 z-50">
      <div className="bg-white p-6 rounded-lg w-96">
        <h2 className="text-lg font-semibold mb-6">Supplier Details</h2>
        <div>
          {/* Display fields for each property */}
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
        </div>
        <div className="flex justify-end mt-5">
          <button onClick={onClose} className="bg-gray-300 text-black font-bold rounded-md">Close</button>
        </div>
      </div>
    </div>
  );
};

export default SupplierDisplayView;
