import React from 'react'
import CustomerTable from '../../components/table/CustomerTable'

const Customers = () => {
  return (
    <>
    <div className="m-7">
      <div className="text-3xl text-center mt-6 text-dark-green">Customers</div>

      <div className=" m-3 text-xl flex justify-end  text-medium-green">
        
        <div className="flex gap-3">
        </div>
      </div>
      <CustomerTable/>
    </div>
    </>
  );
}

export default Customers