import React, { useState, useEffect } from "react";
import { useForm, useFieldArray } from "react-hook-form";
import { useNavigate, useParams } from "react-router-dom";
import updateOrder from "../APICalls/Orders/UpdateOrder";
import getOrderById from "../APICalls/Orders/GetOrderById";
import getAllSupplier from "../APICalls/Suppliers/GetAllSuppliers";
import getAllCustomers from "../APICalls/Customers/GetAllCustomers";
import getItems from "../APICalls/Orders/GetItems";
import getPalletTypes from "../APICalls/Orders/GetPalletTypes";

const EditOrderForm = () => {
  const { id } = useParams();
  const [suppliers, setSuppliers] = useState([]);
  const [customer, setCustomers] = useState([]);
  const [collectionDate, setCollectionDate] = useState(getTodayDate());
  const [deliveryDate, setDeliveryDate] = useState(getTodayDate());
  const [items, setItems] = useState([]);
  const [palletTypes, setPalletTypes] = useState([]);
  const [order, setOrder] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const orderData = await getOrderById(id);
        setOrder(orderData);

        const suppliersData = await getAllSupplier();
        setSuppliers(suppliersData);

        const palletTypesData = await getPalletTypes();
        setPalletTypes(palletTypesData);

        const customersData = await getAllCustomers();
        setCustomers(customersData);

        const itemsData = await getItems();
        setItems(itemsData);
      } catch (error) {
        console.error("Error fetching data:", error);
      }
    };

    fetchData();
  }, [id]);

  const {
    register,
    setValue,
    handleSubmit,
    formState: { errors },
    control,
  } = useForm({
    defaultValues: order,
  });

  const { fields, append, remove } = useFieldArray({
    control,
    name: "lines",
  });

  const navigate = useNavigate();

  const onSubmit = async (data) => {
    try {
      const updatedOrder = {
        ...order,
        ...data,
      };
      await updateOrder(id, updatedOrder);
  
      window.alert("Order updated");
      navigate("/Transport/Order/AllOrders");
    } catch (error) {
      console.error("Failed to update order:", error);
    }
  };
  
  function getTodayDate() {
    const today = new Date();
    const year = today.getFullYear();
    let month = today.getMonth() + 1;
    let day = today.getDate();

    month = month < 10 ? `0${month}` : month;
    day = day < 10 ? `0${day}` : day;

    return `${year}-${month}-${day}`;
  }

  function handleDateChange(event) {
    setCollectionDate(event.target.value);
  }

  function handleDeliveryDateChange(event) {
    setDeliveryDate(event.target.value);
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
                  className="accent-medium-green mr-2 "
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
              value={deliveryDate}
              onChange={handleDeliveryDateChange}
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
              {...register("supplierPK", { required: true })}
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
              {...register("customerPK", { required: true })}
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
        <table className="w-full ml-4">
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
            {fields.map((line, index) => (
              <tr key={line.id}>
                <td>
                  <input
                    type="text"
                    {...register(`lines[${index}].itemqty`)}
                    // defaultValue={line.itemqty}
                    className="w-full h-8 border rounded pl-2 text-center"
                  />
                </td>
                <td>
                  <input
                    type="text"
                    {...register(`lines[${index}].itemno`)}
                    // defaultValue={line.itemID}
                    className="w-full h-8 border rounded pl-2 text-center"
                    list="itemList"
                    onChange={(e) => {
                      const selectedItemID = e.target.value;
                      const selectedItem = items.find(
                        (item) => item.itemID === selectedItemID
                      );
                      if (selectedItem) {
                        setValue(`lines[${index}].ItemName`, selectedItem.name);
                      }
                    }}
                  />
                  <datalist id="itemList">
                    {items.map((item) => (
                      <option key={item.itemPK} value={item.itemID} />
                    ))}
                  </datalist>
                </td>
                <td>
                  <input
                    type="text"
                    // defaultValue={line.name}
                    {...register(`lines[${index}].ItemName`)}
                    className="w-full h-8 border rounded pl-2 text-center text-sm"
                    list="itemName"
                    onChange={(e) => {
                      const selectedName = e.target.value;
                      const selectedItem = items.find(
                        (item) => item.name === selectedName
                      );
                      if (selectedItem) {
                        setValue(`lines[${index}].ItemName`, selectedItem.name);
                      }
                    }}
                  
                  />
                  <datalist id="itemName">
                    {items.map((item) => (
                      <option key={item.itemPK} value={item.name} />
                    ))}
                  </datalist>
                </td>
                <td>
                  <input
                    type="text"
                    {...register(`lines[${index}].palletqty`)}
                    // defaultValue={line.palletqty}
                    className="w-full h-8 border rounded pl-2 text-center"
                  />
                </td>
                <td>
                  <select
                    {...register(`lines[${index}].PalletTypeId`)}
                    // defaultValue={line.palletType}
                    className="w-full h-8 border rounded pl-2 text-center"
                  >
                    
                    {palletTypes.map((palletType) => (
                      <option key={palletType.id} value={palletType.id}>
                        {palletType.description}
                      </option>
                    ))}
                  </select>
                </td>
                <td>
                  <button
                    className="p-2 border rounded border-red text-red text-sm hover:bg-red hover:text-white"
                    onClick={() => remove(index)}
                    type="button"
                  >
                    Remove
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        <div className="flex gap-5 justify-center m-6 ">
          <button
            className="hover:bg-medium-green hover:text-white bg-gray border mt-3 text-dark-green py-1 px-2 rounded"
            type="button"
            onClick={() =>
              append({
                itemqty: "",
                itemno: "",
                ItemName: "",
                palletqty: "",
                PalletTypeId: ""
                
              })
            }
          >
            Add Line
          </button>
        </div>
        {/* Buttons */}
        <div className="flex gap-5 justify-center m-6 ">
          <button
            className="hover:bg-medium-green hover:text-white border mt-3 text-dark-green py-1 px-2 rounded"
            type="submit"
          >
            Update
          </button>
          <button
            className="hover:hover:bg-red hover:text-white bg-gray border mt-3 text-dark-green py-1 px-2 rounded"
            type="button"
            onClick={() => navigate("/Transport/Order/AllOrders")}
          >
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
};

export default EditOrderForm;