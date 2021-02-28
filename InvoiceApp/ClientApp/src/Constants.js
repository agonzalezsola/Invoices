const prod = {
    API_URL: 'https://alvaroinvoiceapi.azurewebsites.net/api/invoices'
};

const dev = {
    API_URL: 'https://localhost:44367/api/invoices'
};

export const config = process.env.NODE_ENV === 'development' ? dev : prod;