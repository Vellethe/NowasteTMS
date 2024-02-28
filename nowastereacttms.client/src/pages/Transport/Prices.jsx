import React from 'react'
import PriceTable from '../../components/table/PriceTable'

const Prices = () => {
  return (
    <>
    <div className="m-7">
      <div className="text-3xl text-center mt-6 text-dark-green">Prices</div>

      <div className=" m-3 text-xl flex justify-end  text-medium-green">
        
        <div className="flex gap-3">
        </div>
      </div>
      <PriceTable/>
    </div>
    </>
  );
}

export default Prices
