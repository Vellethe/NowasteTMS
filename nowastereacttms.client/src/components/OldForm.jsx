// import { useForm, SubmitHandler } from "react-hook-form"
import { DevTool } from "@hookform/devtools";
import { useForm } from "react-hook-form";
import { Navigate } from "react";
import { useNavigate } from "react-router-dom";

const CreateForm = () => {
  //TODO Fix form, styling, functionality, the lines section needs to be responsive and fix showing errormessage on lines

  const form = OrderForm({
    defaultValues: {
      palletexchange: true,
    },
    line: {
      itemqty: "",
      itemno: "",
      description: "",
      palletqty: "",
      pallettype: "",
    },
  });
  const { register, handleSubmit, formState: { errors } } = useForm();
  const navigate = useNavigate();

  const onSubmit = (data) => {
    console.log("Form submitted", data);
  };

  return (
    <div>
      <form onSubmit={handleSubmit(onSubmit)}
        className="w-full mb-9 border"
        noValidate
      >
        <div className="w-full bg-gray h-12 text-center mb-5 pt-3">
          General information
        </div>
        <div className="flex flex-wrap mx-3 mb-6">
          <div className="w-full md:w-1/2 px-3 mb-6 md:mb-0">
            <label>orderid
              <input type="text" {...register("orderId", {required: true})}/>
              id="orderid"
              className="appearance-none block w-full border rounded py-3 px-4 mb-3 leading-tight focus:bg-white"
              {errors.orderid && <p>orderid is required</p>}
              <p className="text-sm text-red">{errors.orderid?.message}</p>
            </label>
          </div>
          <div className="w-full md:w-1/2 px-3">
            <label
              className="block tracking-wide text-gray-700 text-center text-xl font-bold mb-2"
              htmlFor="handlerid"
            >
              HandlerId
              <input
                className="appearance-none block w-full border rounded py-3 px-4 mb-3 leading-tight focus:bg-white"
                id="handlerid"
                type="text"
                {...register("handlerid", {
                  required: "HandlerId is required",
                })}
              />
              <p className="text-sm text-red">{errors.handlerid?.message}</p>
            </label>
          </div>
        </div>
        <div className="flex flex-wrap mx-3 mb-6">
          <div className="w-full md:w-1/2 px-3">
            <label
              htmlFor="palletexchange"
              className="block tracking-wide text-gray-700 text-center text-xl font-bold mb-2"
            >
              Pallet Exchange
              <div className="appearance-none block w-full border rounded py-3 px-4 mb-3 leading-tight">
                <input
                  className="border rounded mr-3 "
                  type="checkbox"
                  id="palletexchange"
                  {...register("palletexchange")}
                />
                <span className="text-sm">Pallet exchange is requested</span>
              </div>
            </label>
          </div>
          <div className="w-full md:w-1/2 px-3 relative">
            <label
              htmlFor="origin"
              className="block tracking-wide text-gray-700 text-center text-xl font-bold"
            >
              Origin
              <select
                className=" relative block appearance-none w-full border py-3 px-4 rounded leading-tight focus:outline-none focus:bg-white focus:border-gray-500"
                id="origin"
                placeholder="Select Origin"
                {...register("origin")}
              >
                <option>Portal</option>
              </select>
              <div className="pointer-events-none absolute inset-y-0 right-0 flex items-center px-2 mt-3 text-gray-700">
                <svg
                  className="fill-current h-4 w-4 mr-4"
                  xmlns="http://www.w3.org/2000/svg"
                  viewBox="0 0 20 20"
                >
                  <path d="M9.293 12.95l.707.707L15.657 8l-1.414-1.414L10 10.828 5.757 6.586 4.343 8z" />
                </svg>
              </div>
            </label>
          </div>
        </div>
        <div className="flex flex-wrap mx-3 mb-6">
          <div className="w-full md:w-1/2 px-3 mb-6 md:mb-0">
            <label
              htmlFor="collectiondate"
              className="block tracking-wide text-gray-700 text-center text-xl font-bold"
            >
              CollectionDate
              <input
                type="date"
                className="w-full rounded border bg-transparent px-3 transition-all"
                id="collectiondate"
                {...register("collectiondate", {
                  required: "Collectiondate is required",
                })}
              />
              <p className="text-sm text-red mt-2">
                {errors.collectiondate?.message}
              </p>
            </label>
          </div>
          <div className="w-full md:w-1/2 px-3 mb-6 md:mb-0">
            <label
              htmlFor="deliverydate"
              className="block tracking-wide text-gray-700 text-center text-xl font-bold"
            >
              DeliveryDate
              <input
                type="date"
                className="w-full rounded border bg-transparent px-3 transition-all"
                id="deliverydate"
                {...register("deliverydate", {
                  required: "Deliverydate is required",
                })}
              />
              <p className="text-sm text-red mt-2">
                {errors.deliverydate?.message}
              </p>
            </label>
          </div>
        </div>

        <div className="flex flex-wrap mx-3 mb-6 justify-center">
          <div className="w-full md:w-1/2 px-3 relative text-center text-xl">
            <label
              className="block tracking-wide text-gray-700 font-bold mb-2"
              htmlFor="customer"
            >
              Customer
              <div className="relative">
                <select
                  className="block appearance-none w-full bg-gray-200 border border-gray-200 text-gray-700 py-3 px-4 pr-8 rounded leading-tight focus:outline-none focus:bg-white focus:border-gray-500"
                  id="customer"
                  type="select"
                  {...register("customer", {
                    required: "Choosing customer is required",
                  })}
                >
                  <option></option>
                  <option>Coop</option>
                  <option>Ica</option>
                  <option>Netto</option>
                </select>
                <div className="pointer-events-none absolute inset-y-0 right-0 flex items-center px-2 text-gray-700">
                  <svg
                    className="fill-current h-4 w-4"
                    xmlns="http://www.w3.org/2000/svg"
                    viewBox="0 0 20 20"
                  >
                    <path d="M9.293 12.95l.707.707L15.657 8l-1.414-1.414L10 10.828 5.757 6.586 4.343 8z" />
                  </svg>
                </div>
              </div>
              <p className="text-sm text-red mt-2">
                {errors.customer?.message}
              </p>
            </label>
          </div>
          <div className="w-full md:w-1/2 px-3 relative">
            <label
              className="block text-center text-xl tracking-wide text-gray-700 font-bold mb-2 "
              htmlFor="supplier"
            >
              Supplier
              <div className="relative">
                <select
                  className="appearance-none w-full bg-gray-200 border border-gray-200 text-gray-700 py-3 px-4 pr-8 rounded leading-tight focus:outline-none focus:bg-white focus:border-gray-500"
                  id="supplier"
                  type="select"
                  {...register("supplier", {
                    required: "Choosing supplier is required",
                  })}
                >
                  <option></option>
                  <option>ICA</option>
                  <option>Coop</option>
                  <option>Netto</option>
                </select>
                <div className="pointer-events-none absolute inset-y-0 right-0 flex items-center px-2 text-gray-700">
                  <svg
                    className="fill-current h-4 w-4"
                    xmlns="http://www.w3.org/2000/svg"
                    viewBox="0 0 20 20"
                  >
                    <path d="M9.293 12.95l.707.707L15.657 8l-1.414-1.414L10 10.828 5.757 6.586 4.343 8z" />
                  </svg>
                </div>
              </div>
              <p className="text-sm text-red mt-2">
                {errors.supplier?.message}
              </p>
            </label>
          </div>
        </div>
        <div className="m-6">
          <label
            htmlFor="internalcomment"
            className="block tracking-wide text-gray-700 text-center "
          >
            <span className="text-gray-700 text-xl font-bold">
              Internal comment
            </span>
            <textarea
              id="internalcomment"
              type="text"
              className=" w-full mb-2 border rounded pl-2 pt-2"
              {...register("internalcomment")}
            ></textarea>
          </label>
        </div>
        <div className="m-6">
          <label
            htmlFor="commenttoagent"
            className="block tracking-wide text-gray-700 text-center"
          >
            <span className="text-gray-700 text-xl font-bold">
              Comment to agent
            </span>
            <textarea
              id="commenttoagent"
              type="text"
              className=" w-full mb-2 border rounded pl-2"
              {...register("commenttoagent")}
            ></textarea>
          </label>
        </div>

        {/* <div className="w-full bg-gray h-12 text-center mb-5 pt-3">Lines</div>
        <hr className="m-6"></hr> */}

        
          <div className="flex flex-row px-3 mb-6 ">
            <div className="tracking-wide text-gray-700 text-center text-xl mb-2">
            <label htmlFor="itemqty" className="text-center text-xl mb-2">

              Item qty.
              <input
                id="itemqty"
                type="number"
                className="rounded border px-3 text-center"
                {...register("line.itemqty", {
                  required: "Item Qty is required",
                })}
              />
            </label>
            </div>
            <p className="text-sm text-red">{errors.itemqty?.message}</p>
          
            <label
              htmlFor="itemno"
              className="tracking-wide text-gray-700 text-center text-xl mb-2"
            >
              Item no.
              <input
                id="itemno"
                type="number"
                className="rounded border px-3 text-center"
                {...register("line.itemno", {
                  required: "Item No is required",
                })}
              />
            </label>
            <label
              htmlFor="description"
              className="tracking-wide text-gray-700 text-center text-xl mb-2"
            >
              Description
              <input
                id="description"
                type="text"
                className=" w-80 rounded border bg-transparent px-3"
                {...register("line.description", {
                  required: "A description is required",
                })}
              />
            </label>

            <label
              htmlFor="palletqty"
              className="tracking-wide text-gray-700 text-center text-xl mb-2"
            >
              Pallet qty.
              <input
                id="palletqty"
                type="text"
                className="rounded border px-3 text-center"
                {...register("line.palletqty", {
                  required: "Pallet Qty required",
                })}
              />
               
            </label>
            <label
              htmlFor="pallettype"
              className="tracking-wide text-gray-700 text-center text-xl mb-2"
            >
              Pallet type
              <select
                id="pallettype"
                type="select"
                className="rounded border px-3"
                {...register("line.pallettype", {
                  required: "A pallet type is required",
                })}
              >
                <option></option>
                <option value="EU B Pallet">EU B Pallet</option>
                <option value="Red Pallets">Sea Pallet</option>
                <option value="Blue Pallet">Blue Pallet</option>
                <option value="Green Pallet">Green Pallet</option>
              </select>
            </label>
                
            <div className="flex justify-center items-center mr-4 pt-3">
              <button className="p-2 border rounded border-red text-red text-sm hover:bg-red hover:text-white "              >
                Remove
              </button>
            </div>
          </div>
        
        <div className="text-center">
          <button className="hover:bg-medium-green hover:text-white bg-gray border mt-3 text-dark-green py-1 px-2 rounded">
            Add Line
          </button>
        </div>

        <div className="flex gap-5 justify-center m-6 ">
          <button
            className=" bg-gray border-2 mt-3 text-dark-green py-1 px-2 rounded w-full"
            type="button"
            onClick={() => navigate(-1)}
            >
            Cancel
          </button>
          <button
            className=" hover:bg-medium-green hover:text-white border-2 mt-3 text-dark-green py-1 px-2 rounded w-full"
            type="submit"
          >
            Save
          </button>
        </div>
      </form>
      <DevTool control={control} />
    </div>
  );
};

export default CreateForm;
