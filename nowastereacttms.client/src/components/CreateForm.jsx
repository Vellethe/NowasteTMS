import React, { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import createOrder from "./APICalls/Orders/CreateOrder";
import getAllSupplier from "./APICalls/Suppliers/GetAllSuppliers";
import getAllCustomers from "./APICalls/Customers/GetAllCustomers";

const CreateForm = () => {
  const [formData, setFormData] = useState({
    orderId: "",
    handlerid: "",
    palletexchange: true,
    origin: "",
    collectiondate: "",
    deliverydate: "",
    customer: "",
    supplier: "",
    internalcomment: "",
    commenttoagent: "",
    lines: [
      {
        itemqty: "",
        itemno: "",
        description: "",
        palletqty: "",
        pallettype: "",
      },
    ],
  });

  const [suppliers, setSuppliers] = useState([]);

  useEffect(() => {
    const fetchSuppliers = async () => {
      try {
        const data = await getAllSupplier();
        console.log(data)
        setSuppliers(data);
      } catch (error) {
        console.error("Error fetching suppliers:", error);
      }
    };

    fetchSuppliers();
  }, []);

  const [customer, setCustomers] = useState([]);

  useEffect(() => {
    const fetchCustomers = async () => {
      try {
        const data = await getAllCustomers();
        console.log(data)
        setCustomers(data);
      } catch (error) {
        console.error("Error fetching customers:", error);
      }
    };

    fetchCustomers();
  }, []);

  const { register, handleSubmit, formState: { errors } } = useForm();
  const navigate = useNavigate();

  const onSubmit = async (data) => {
    try {
      const pk = await createOrder(data);
      console.log('Order created with primary key:', pk);
      window.alert("Order created")
      navigate('/success');
    } catch (error) {
      // Handle errors
      console.error('Failed to create order:', error);
    }
  };

  const handleInputChange = (index, event) => {
    const { name, value } = event.target;
    const list = [...formData.lines];
    list[index] = {
      ...list[index],
      [name]: value
    };
    setFormData({ ...formData, lines: list });
  };

  const handleAddLine = () => {
    setFormData({
      ...formData,
      lines: [
        ...formData.lines,
        {
          itemqty: "",
          itemno: "",
          description: "",
          palletqty: "",
          pallettype: "",
        },
      ],
    });
  };

  const handleRemoveLine = (index) => {
    const list = [...formData.lines];
    list.splice(index, 1);
    setFormData({ ...formData, lines: list });
  };

  const [collectionDate, setCollectionDate] = useState(getTodayDate());

  function getTodayDate() {
    const today = new Date();
    const year = today.getFullYear();
    var month = today.getMonth() + 1;
    var day = today.getDate();

    // Add zero if month or day is less than 10
    month = month < 10 ? `0${month}` : month;
    day = day < 10 ? `0${day}` : day;

    return `${year}-${month}-${day}`;
  }

  function handleDateChange(event) {
    setCollectionDate(event.target.value);
  }

  return (
    <div>
      <form
        onSubmit={handleSubmit(onSubmit)}
        className="w-full mb-10 border"
        noValidate
      >
        {/* General information */}
        <div className="w-full bg-gray h-12 text-center font-bold mb-3 pt-3">
          General information
        </div>
        <div className="flex flex-wrap mx-3 mb-6">
          <div className="w-full md:w-1/2 px-3 mb-6 md:mb-0 text-center font-bold">
            <label htmlFor="orderId">Order ID</label>
            <input
              type="text"
              id="orderId"
              {...register("orderId", { required: true })}
              className="appearance-none block w-full border rounded py-3 px-4 mb-2 leading-tight focus:bg-white text-center"
            />
            {errors.orderId && (
              <p className="text-sm text-red">Order ID is required</p>
            )}
          </div>
          <div className="w-full md:w-1/2 px-3 text-center font-bold">
            <label htmlFor="handlerid">Handler ID</label>
            <input
              type="text"
              id="handlerid"
              {...register("handlerid", { required: true })}
              className="appearance-none block w-full border rounded py-3 px-4 mb-2 leading-tight focus:bg-white text-center"
            />
            {errors.handlerid && (
              <p className="text-sm text-red">Handler ID is required</p>
            )}
          </div>
        </div>

        <div className="flex flex-wrap mx-3 mb-6">
          <div className="w-full md:w-1/2 px-3 text-center">
            <label
              htmlFor="palletexchange"
              className="block tracking-wide text-gray-700 text-center font-bold mb-2"
            >
              Pallet Exchange
              <div className="appearance-none block w-full border rounded py-3 px-4 mb-2 leading-tight">
                <input
                  className="accent-medium-green mr-2"
                  type="checkbox"
                  id="palletexchange"
                  {...register("palletexchange")}
                />
                <span className="text-sm">Pallet exchange is requested</span>
              </div>
            </label>
          </div>
          <div className="w-full md:w-1/2 px-3 text-center font-bold">
            <label htmlFor="origin">Origin</label>
            <select
              id="origin"
              {...register("origin")}
              className="appearance-none block w-full border rounded py-3 px-4 mb-2 leading-tight focus:bg-white text-center"
            >
              <option>Portal</option>
            </select>
          </div>
        </div>

        <div className="flex flex-wrap mx-3 mb-6">
          <div className="w-full md:w-1/2 px-3 text-center font-bold">
            <label htmlFor="collectiondate">Collection Date</label>
            <input
              type="date"
              id="collectiondate"
              {...register("collectionDate")}
              value={collectionDate}
              onChange={handleDateChange}
              className="appearance-none block w-full border rounded py-3 px-4 mb-2 leading-tight focus:bg-white text-center"
            />
            {errors.collectiondate && (
              <p className="text-sm text-red">Collection Date is required</p>
            )}
          </div>
          <div className="w-full md:w-1/2 px-3 text-center font-bold">
            <label htmlFor="deliverydate">Delivery Date</label>
            <input
              type="date"
              id="deliverydate"
              {...register("deliveryDate")}
              className="appearance-none block w-full border rounded py-3 px-4 mb-2 leading-tight focus:bg-white text-center"
            />
            {errors.deliverydate && (
              <p className="text-sm text-red">Delivery Date is required</p>
            )}
          </div>
        </div>

        <div className="flex flex-wrap mx-3 mb-6">
          <div className="w-full md:w-1/2 px-3 text-center font-bold">
            <label htmlFor="supplier">Supplier</label>
            <select
              id="supplier"
              {...register("supplier", { required: true })}
              className="appearance-none block w-full border rounded py-3 px-4 mb-2 leading-tight focus:bg-white text-center"
            >
              <option value="">Select supplier</option>
              {suppliers.map((supplier) => (
                <option key={supplier.supplierPK} value={supplier.supplierPK}>
                  {supplier.businessUnit.name}
                </option>
              ))}
            </select>
            {errors.supplier && (
              <p className="text-sm text-red">Supplier is required</p>
            )}
          </div>
          <div className="w-full md:w-1/2 px-3 text-center font-bold">
            <label htmlFor="customer">Customer</label>
            <select
              id="customer"
              {...register("customer", { required: true })}
              className="appearance-none block w-full border rounded py-3 px-4 mb-2 leading-tight focus:bg-white text-center"
            >
              <option value="">Select customer</option>
              {customer.map((customer) => (
                <option key={customer.customerPK} value={customer.customerPK}>
                  {customer.businessUnit.name}
                </option>
              ))}
            </select>
            {errors.customer && (
              <p className="text-sm text-red">Customer is required</p>
            )}
          </div>
        </div>

        <div className="m-6 text-center font-bold">
          <label htmlFor="internalcomment">Internal Comment</label>
          <textarea
            id="internalcomment"
            type="text"
            {...register("internalcomment")}
            className="w-full border rounded pl-2 pt-2"
          ></textarea>
        </div>
        <div className="m-6 text-center font-bold">
          <label htmlFor="commenttoagent">Comment to Agent</label>
          <textarea
            id="commenttoagent"
            type="text"
            {...register("commenttoagent")}
            className="w-full border rounded pl-2"
          ></textarea>
        </div>
        {/* End of General Information Section */}

        {/* Lines Section */}
        <div className="w-full bg-gray h-12 text-center mb-5 pt-3">Lines</div>
        <table className="w-full">
          <thead>
            <tr>
              <th>Item qty.</th>
              <th>Item no.</th>
              <th>Description</th>
              <th>Pallet qty.</th>
              <th>Pallet type</th>
            </tr>
          </thead>
          <tbody>
            {formData.lines.map((line, index) => (
              <tr key={index}>
                <td>
                  <input
                    type="number"
                    {...register(`lines[${index}].itemqty`)}
                    value={line.itemqty}
                    onChange={(e) => handleInputChange(index, e)}
                    name="itemqty"
                    className="w-full h-8 border rounded pl-2 text-center"
                  />
                </td>
                <td>
                  <input
                    type="number"
                    {...register(`lines[${index}].itemno`)}
                    value={line.itemno}
                    onChange={(e) => handleInputChange(index, e)}
                    name="itemno"
                    className="w-full h-8 border rounded pl-2 text-center"
                  />
                </td>
                <td>
                  <input
                    type="text"
                    {...register(`lines[${index}].description`)}
                    value={line.description}
                    onChange={(e) => handleInputChange(index, e)}
                    name="description"
                    className="w-full h-8 border rounded pl-2 text-center"
                  />
                </td>
                <td>
                  <input
                    type="number"
                    {...register(`lines[${index}].palletqty`)}
                    value={line.palletqty}
                    onChange={(e) => handleInputChange(index, e)}
                    name="palletqty"
                    className="w-full h-8 border rounded pl-2 text-center"
                  />
                </td>
                <td>
                  <select
                    value={line.pallettype}
                    {...register(`lines[${index}].pallettype`)}
                    onChange={(e) => handleInputChange(index, e)}
                    name="pallettype"
                    className="w-full h-8 border rounded pl-2 text-center"
                  >
                    <option></option>
                    <option value="EU B Pallet">EU B Pallet</option>
                    <option value="Red Pallets">Red Pallet</option>
                    <option value="Blue Pallet">Blue Pallet</option>
                    <option value="Green Pallet">Green Pallet</option>
                  </select>
                </td>
                <td>
                  <div className="flex gap-5 justify-center m-6">
                    <button
                      className="p-2 border rounded border-red text-red text-sm hover:bg-red hover:text-white"
                      onClick={(e) => {
                        e.preventDefault();
                        handleRemoveLine(index);
                      }}
                    >
                      Remove
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        <div className="flex gap-5 justify-center m-6 ">
          <button
            className="hover:bg-medium-green hover:text-white bg-gray border mt-3 text-dark-green py-1 px-2 rounded"
            onClick={handleAddLine}
          >
            Add Line
          </button>
        </div>
        {/* End of Lines Section */}

        {/* Buttons */}
        <div className="flex gap-5 justify-center m-6 ">
          <button
            className="bg-gray border-2 mt-3 text-dark-green py-1 px-2 rounded w-full"
            type="button"
            onClick={() => navigate(-1)}
          >
            Cancel
          </button>
          <button
            className="hover:bg-medium-green hover:text-white border-2 mt-3 text-dark-green py-1 px-2 rounded w-full"
            type="submit"
          >
            Save
          </button>
        </div>
      </form>
    </div>
  );
};

export default CreateForm;
