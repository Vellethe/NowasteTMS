import React from 'react'
import ServiceTable from '../../components/table/ServiceTable'

const TransportOrderService = () => {
    return (
      <>
      <div className="m-7">
        <div className="text-3xl text-center mt-6 text-dark-green">Services</div>
  
        <div className=" m-3 text-xl flex justify-end  text-medium-green">
          
          <div className="flex gap-3">
          </div>
        </div>
        <ServiceTable/>
      </div>
      </>
    );
  }

export default TransportOrderService