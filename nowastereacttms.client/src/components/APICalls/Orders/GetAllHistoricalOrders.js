import { baseUrl } from "../API";

const getAllOrders = async () => {
    try {
        const response = await fetch(`${baseUrl}/Order`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ Size: 999999, Page: 0, Filter: {}, Column: {}, "historical": true }),
        });

        if (!response.ok) {
            throw new Error('Failed to fetch orders');
        }

        const data = await response.json();

        data.orders = data.orders.map(o => {
            var euLines = o.lines.map(l => {
                if (l.palletType.id === 2) {
                    return l.palletQty
                } else return 0;
            })
            o.euQty = euLines.reduce((a, b) => a + b, 0);

            var seaLines = o.lines.map(l => {
                if (l.palletType.id === 8) {
                    return l.palletQty
                } else return 0;
            })
            o.seaQty = seaLines.reduce((a, b) => a + b, 0);

            // Format datetime fields and calculate weekdays
            o.collectionDate = formatDatetime(o.collectionDate);
            o.deliveryDate = formatDatetime(o.deliveryDate);
            o.arrivalDate = formatDatetime(o.transportOrder.arrivalDate);
            o.created = formatDatetime(o.created);
            o.updated = formatDatetime(o.updated);
            o.collectionDateWD = getWeekdayFromDate(o.collectionDate);
            o.deliveryDateWD = getWeekdayFromDate(o.deliveryDate);
            o.updatedWD = getWeekdayFromDate(o.updated);
            // Add more fields if needed
            return o;
        });

        return data;
    } catch (error) {
        throw new Error('Error fetching orders: ' + error.message);
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

function getWeekdayFromDate(dateString) {
    const daysOfWeek = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
    const date = new Date(dateString);
    const weekdayIndex = date.getDay();
    return daysOfWeek[weekdayIndex];
}

export default getAllOrders;
