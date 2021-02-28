import { config } from '../Constants'

export const invoiceServices = {
    getInvoices,
    getInvoiceById,
    createInvoice,
    updateInvoice,
    deleteInvoice
};

async function getInvoices() {
    const response = await fetch(config.API_URL);
    const data = await response.json();
    return data;
}

async function getInvoiceById(id, currency = null) {
    const response = currency
        ? await fetch(`${config.API_URL}/${id}?currency=${currency}`)
        : await fetch(`${config.API_URL}/${id}`);
    const data = await response.json();
    return data;
}

async function createInvoice(invoice) {
    await fetch(config.API_URL, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(invoice)
    });
}

async function updateInvoice(id, invoice) {
    await fetch(`${config.API_URL}/${id}`, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(invoice)
    });
}

async function deleteInvoice(id) {
    await fetch(`${config.API_URL}/${id}`, {
        method: 'DELETE',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        }
    });
}