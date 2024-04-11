import { baseUrl } from "../API";

const getAllServices = async() => {
    try {
      const response = await fetch(`${baseUrl}/Service`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({Size: 400, Page: 0, Filter: {}, Column: {}}),
      });
      
      if (!response.ok) {
        throw new Error('Failed to fetch services');
      }
      const data = await response.json();

      data.forEach(item => {
        item.timestamp = formatDatetime(item.timestamp);
      });

      return data;
    } catch (error) {
      throw new Error('Error fetching services: ' + error.message);
    }
  };

  function formatDatetime(datetimeString) {
    const datetime = new Date(datetimeString);
    const day = datetime.getDate().toString().padStart(2, '0');
    const month = (datetime.getMonth() + 1).toString().padStart(2, '0');
    const year = datetime.getFullYear();
    const hours = datetime.getHours().toString().padStart(2, '0'); 
    const minutes = datetime.getMinutes().toString().padStart(2, '0');
    const seconds = datetime.getSeconds().toString().padStart(2, '0'); 
    return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
  }
  
  export default getAllServices;