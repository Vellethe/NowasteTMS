import React, { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import createPrice from "./APICalls/Prices/CreatePrice";

const CreatePriceForm = () => {
    const navigate = useNavigate();

useEffect(() => {
    const fetchData = async () => {
        try {

        } catch (error) {
            console.error("Error fetching data:", error);
        }
    };

    fetchData();
}, []);

const onSubmit = async (data) => {
    try {
        const pk = await createPrice(data);
        window.alert("Price created");
        navigate("/Transport/TransportZonePrices");
    } catch (error) {
        console.error("Failed to create new price:", error);
    }
};




return (
    <div>
        <form>
            
        </form>
    </div>

)














}