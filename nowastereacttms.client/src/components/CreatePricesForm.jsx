import React, { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import createPrice from "./APICalls/Prices/CreatePrice";
import getAllPrices from "./APICalls/Prices/GetAllPrices";
import getAllAgents from "./APICalls/Agents/GetAllAgents";
import fetchAllLocations from "./APICalls/AllLocations";
import fetchAllCurrencies from "./APICalls/AllCurrencies";

const CreatePriceForm = () => {
  const [agents, setAgents] = useState([]);
  const [fromZone, setFromZone] = useState("");
  const [fromZoneList, setFromZoneList] = useState([]);
  const [toZone, setToZone] = useState("");
  const [toZoneList, setToZoneList] = useState([]);
  const [uniqueAllZones, setUniqueAllZones] = useState([]);
  const [currency, setCurrency] = useState("");
  const [currencyList, setCurrencyList] = useState([]);
  const [transportTypes, setTransportTypes] = useState("");
  const [transportTypeList, setTransportTypeList] = useState([])
  const [validFrom, setValidFrom] = useState("");
  const [validTo, setValidTo] = useState("");
  const [price, setPrice] = useState("");
  const [description, setDescription] = useState("");

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();

  const navigate = useNavigate();

  useEffect(() => {
    const fetchData = async () => {
      try {
        const agentsData = await getAllAgents();
        const uniqueAgents = agentsData.map((agent) => ({
          agentPK: agent.agentPK,
          name: agent.businessUnit?.name || "",
        }));

        const priceData = await getAllPrices();
        const uniqueTransportTypes = [...new Map(priceData.map(item => [item.transportType.transportTypePK, item.transportType])).values()];
        const validFromDates = priceData.map((item) => item?.validFrom || "");
        const validToDates = priceData.map((item) => item?.validTo || "");

        const allLocations = await fetchAllLocations();
        const uniqueAllZones = Array.from(new Set(allLocations.map(location => location.name)));

        const currencies = await fetchAllCurrencies();
        
        console.log("Fetched Content:", transportTypeList);

        setAgents(uniqueAgents);
        setUniqueAllZones(uniqueAllZones);
        setFromZoneList(uniqueAllZones);
        setToZoneList(uniqueAllZones);
        setCurrencyList(currencies);
        setTransportTypeList(uniqueTransportTypes);
        setValidFrom(validFromDates[0]);
        setValidTo(validToDates[0]);

      } catch (error) {
        console.error("Failed to fetch data:", error);
      }
    };

    fetchData();
  }, []);

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
        <div className="flex flex-wrap mx-3 mb-4">
          <div className="w-full md:w-1/2 px-3 text-center font-bold">
            <label htmlFor="agent">Agent</label>
            <select
              id="agent"
              {...register("agent")}
              className="appearance-none block w-full border rounded py-3 px-4 mb-2 leading-tight focus:bg-white text-center"
            >
              {agents.map((agent) => (
                <option key={agent.agentPK} value={agent.agentPK}>
                  {agent.name}
                </option>
              ))}
            </select>
            {errors.agent && (
              <p className="text-sm text-red">Agent is required</p>
            )}
          </div>
          <div className="w-full md:w-1/2 px-3 text-center font-bold">
            <label htmlFor="transportType">Transport Type</label>
            <select
              id="transportType"
              {...register("transportType.transportTypePK")}
              value={transportTypes?.transportTypePK}
              onChange={(e) => setTransportTypes(e.target.value)}
              className="appearance-none block w-full border rounded py-3 px-4 mb-2 leading-tight focus:bg-white text-center"
            >
              {transportTypeList.map((type) => (
                <option key={type.transportTypePK} value={type.transportTypePK}>
                  {type.description}
                </option>
              ))}
            </select>
            {errors.transportTypes?.transportTypePK && (
              <p className="text-sm text-red">Transport Type is required</p>
            )}
          </div>
          </div>
        
        <div className="flex flex-wrap mx-3 mb-4">
          <div className="w-full md:w-1/2 px-3 text-center font-bold">
            <label htmlFor="fromZone">From Zone</label>
            <select
              id="fromZone"
              {...register("fromZoneName")}
              value={fromZone}
              onChange={(e) => setFromZone(e.target.value)}
              className="appearance-none block w-full border rounded py-3 px-4 mb-2 leading-tight focus:bg-white text-center"
            >
              {fromZoneList.map((zone, index) => (
                <option key={index} value={zone}>
                  {zone}
                </option>
              ))}
            </select>
            {errors.price?.fromTransportZone && (
              <p className="text-sm text-red">From Zone is required</p>
            )}
          </div>
          <div className="w-full md:w-1/2 px-3 text-center font-bold">
            <label htmlFor="toZone">To</label>
            <select
              id="toZone"
              {...register("toZoneName")}
              value={toZone}
              onChange={(e) => setToZone(e.target.value)}
              className="appearance-none block w-full border rounded py-3 px-4 mb-2 leading-tight focus:bg-white text-center"
            >
              {toZoneList.map((zone, index) => (
                <option key={index} value={zone}>
                  {zone}
                </option>
              ))}
            </select>
            {errors.toTransportZone && (
              <p className="text-sm text-red">To Zone is required</p>
            )}
          </div>
        </div>

        <div className="flex flex-wrap mx-3 mb-4">
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

        <div className="flex flex-wrap mx-3 mb-4">
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
            <select
              id="currency"
              {...register("name", { required: true })}
              value={currency.name}
              onChange={(e) => setCurrency(e.target.value)}
              className="appearance-none block w-full border rounded py-3 px-4 mb-2 leading-tight focus:bg-white text-center"
            >
              {currencyList.map((zone, index) => (
                <option key={index} value={zone.name}>
                  {zone.name}
                </option>
              ))}
            </select>
            {errors.currency && (
              <p className="text-sm text-red">Currency is required</p>
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
            className="hover:bg-medium-green hover:text-white border mt-3 text-dark-green py-1 px-2 rounded"
            type="submit"
          >
            Save
          </button>
          <button
            className="hover:hover:bg-red hover:text-white bg-gray border mt-3 text-dark-green py-1 px-2 rounded"
            type="button"
            onClick={() => navigate(-1)}
          >
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
};

export default CreatePriceForm;