import React from 'react'
import SupplierTable from '../../components/table/SupplierTable'

const Suppliers = () => {
  return (
    <>
    <div className="m-7">
      <div className="text-3xl text-center mt-6 text-dark-green">Suppliers</div>

      <div className=" m-3 text-xl flex justify-end  text-medium-green">
        
        <div className="flex gap-3">
        </div>
      </div>
      <SupplierTable/>
    </div>
    </>
  );
}

export default Suppliers