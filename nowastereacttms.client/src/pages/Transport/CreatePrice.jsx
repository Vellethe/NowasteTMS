import React from 'react'
import CreatePriceForm from '../../components/CreatePricesForm';

const CreatePrice = () => {
  return (
    <div className="container">
      <h1 className="text-center my-6 text-3xl">New Price</h1>
      <CreatePriceForm />
    </div>
  );
}

export default CreatePrice