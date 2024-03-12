import axios from 'axios';
const baseUrl = "https://localhost:7253/api";

const fetchData = async (page, pageSize, filter, column, historical) => {
    try {
        const response = await axios.post(`${baseUrl}/SearchOrders`, {
            Page: page,
            Size: pageSize,
            Filter: filter,
            Column: column,
            Historical: historical
        });
        return response.data;
    } catch (error) {
        console.error('Error fetching data:', error);
        return { data: [], pageCount: 0 };
    }
};


export default fetchData;
