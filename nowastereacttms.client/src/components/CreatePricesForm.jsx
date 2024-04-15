import React, { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import createPrice from "../APICalls/Prices/CreatePrice";

const CreatePriceForm = () => {
  const [fromZone, setFromZone] = useState([]);
  const [toZone, setToZone] = useState([]);
  const [price, setPrice] = useState([]);
  const [currency, setCurrency] = useState([]);
  const [agent, setAgent] = useState([]);
  const [transportType, setTransportType] = useState([]);
  const [validFrom, setValidFrom] = useState([]);
  const [validTo, setValidTo] = useState([]);
  const [description, setDescription] = useState([]);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();

  const navigate = useNavigate();

  const onSubmit = async (data) => {
    try {
      await createPrice(data);
      window.alert("Price created");
      navigate("/Transport/TransportZonePrices");
    } catch (error) {
      console.error("Failed to create price:", error);
    }
  };

  return (
    <div>
      <form onSubmit={handleSubmit(onSubmit)} className="w-full mb-10 border" noValidate>
        {/* Price details */}
        <div className="w-full bg-gray h-12 text-center font-bold mb-3 pt-3">
          Price details
        </div>
        
        <div className="flex flex-wrap mx-3 mb-6">
          <div className="w-full md:w-1/2 px-3 text-center font-bold">
            <label htmlFor="fromZone">From Zone</label>
            <input
              type="text"
              id="fromZone"
              {...register("fromTransportZone.name", { required: true })}
              value={fromZone}
              onChange={(e) => setFromZone(e.target.value)}
              className="appearance-none block w-full border rounded py-3 px-4 mb-2 leading-tight focus:bg-white text-center"
            />
            {errors.fromTransportZone && (
              <p className="text-sm text-red">From Zone is required</p>
            )}
          </div>
          <div className="w-full md:w-1/2 px-3 text-center font-bold">
            <label htmlFor="toZone">To Zone</label>
            <input
              type="text"
              id="toZone"
              {...register("toTransportZone.name", { required: true })}
              value={toZone}
              onChange={(e) => setToZone(e.target.value)}
              className="appearance-none block w-full border rounded py-3 px-4 mb-2 leading-tight focus:bg-white text-center"
            />
            {errors.toTransportZone && (
              <p className="text-sm text-red">To Zone is required</p>
            )}
          </div>
        </div>

        <div className="flex flex-wrap mx-3 mb-6">
          <div className="w-full md:w-1/2 px-3 text-center font-bold">
            <label htmlFor="price">Price</label>
            <input
              type="number"
              id="price"
              {...register("price", { required: true })}
              value={price}
              onChange={(e) => setPrice(e.target.value)}
              className="appearance-none block w-full border rounded py-3 px-4 mb-2 leading-tight focus:bg-white text-center"
            />
            {errors.price && (
              <p className="text-sm text-red">Price is required</p>
            )}
          </div>
          <div className="w-full md:w-1/2 px-3 text-center font-bold">
            <label htmlFor="currency">Currency</label>
            <input
              type="text"
              id="currency"
              {...register("currency.name", { required: true })}
              value={currency}
              onChange={(e) => setCurrency(e.target.value)}
              className="appearance-none block w-full border rounded py-3 px-4 mb-2 leading-tight focus:bg-white text-center"
            />
            {errors.currency && (
              <p className="text-sm text-red">Currency is required</p>
            )}
          </div>
        </div>

        <div className="flex flex-wrap mx-3 mb-6">
          <div className="w-full md:w-1/2 px-3 text-center font-bold">
            <label htmlFor="agent">Agent</label>
            <input
              type="text"
              id="agent"
              {...register("agent.businessUnit.name", { required: true })}
              value={agent}
              onChange={(e) => setAgent(e.target.value)}
              className="appearance-none block w-full border rounded py-3 px-4 mb-2 leading-tight focus:bg-white text-center"
            />
            {errors.agent && (
              <p className="text-sm text-red">Agent is required</p>
            )}
          </div>
          <div className="w-full md:w-1/2 px-3 text-center font-bold">
            <label htmlFor="transportType">Transport Type</label>
            <input
              type="text"
              id="transportType"
              {...register("transportType.description", { required: true })}
              value={transportType}
              onChange={(e) => setTransportType(e.target.value)}
              className="appearance-none block w-full border rounded py-3 px-4 mb-2 leading-tight focus:bg-white text-center"
            />
            {errors.transportType && (
              <p className="text-sm text-red">Transport Type is required</p>
            )}
          </div>
        </div>

        <div className="flex flex-wrap mx-3 mb-6">
          <div className="w-full md:w-1/2 px-3 text-center font-bold">
            <label htmlFor="validFrom">Valid From</label>
            <input
              type="date"
              id="validFrom"
              {...register("validFrom", { required: true })}
              value={validFrom}
              onChange={(e) => setValidFrom(e.target.value)}
              className="appearance-none block w-full border rounded py-3 px-4 mb-2 leading-tight focus:bg-white text-center"
            />
            {errors.validFrom && (
              <p className="text-sm text-red">Valid From date is required</p>
            )}
          </div>
          <div className="w-full md:w-1/2 px-3 text-center font-bold">
            <label htmlFor="validTo">Valid To</label>
            <input
              type="date"
              id="validTo"
              {...register("validTo", { required: true })}
              value={validTo}
              onChange={(e) => setValidTo(e.target.value)}
              className="appearance-none block w-full border rounded py-3 px-4 mb-2 leading-tight focus:bg-white text-center"
            />
            {errors.validTo && (
              <p className="text-sm text-red">Valid To date is required</p>
            )}
          </div>
        </div>

        <div className="m-6 text-center font-bold">
          <label htmlFor="description">Description</label>
          <textarea
            id="description"
            type="text"
            {...register("description")}
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            className="w-full border rounded pl-2 pt-2"
          ></textarea>
        </div>

        {/* Buttons */}
        <div className="flex gap-5 justify-center m-6 ">
          <button
            className="hover:bg-medium-green hover:text-white bg-gray border mt-3 text-dark-green py-1 px-2 rounded"
            type="button"
            onClick={() => navigate(-1)}
          >
            Cancel
          </button>
          <button
            className="hover:bg-medium-green hover:text-white border mt-3 text-dark-green py-1 px-2 rounded"
            type="submit"
          >
            Save
          </button>
        </div>
      </form>
    </div>
  );
};

export default CreatePriceForm;