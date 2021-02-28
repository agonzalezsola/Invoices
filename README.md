# Invoices

_There is a little app to manage Invoices_

## Getting started

_You can view a live demo over at (https://alvaroinvoiceapp.azurewebsites.net)_

_API server running at (https://alvaroinvoiceapi.azurewebsites.net/api)_

_You can download and try it in your local machine if you want_

## Backend

_Coded in C# with .Net Core. There offer an REST API to allow basic CRUD operations. Thanks to [ExchangeRate-Api v6](https://www.exchangerate-api.com/) the API allow Invoice exchange to the desire currency_

_There is an example of the Invoice structure_

```
{
    "invoiceId": "5e3e0b21-e98a-4480-bfb7-49e8dc61f551",
    "suplier": "",
    "dateIssued": "2019-10-10T13:30:01",
    "currency": "EUR",
    "amount": 1000.00,
    "description": "New projector for confenrece room"
}
```

_Some clarifications_
* Not authentication required.
* The API allow CORS. 
* Data is not persistent, there is store in memory for testing purposes only.

## Frontend

_Developed with [React](https://reactjs.org/) and [Material-UI](https://material-ui.com/) this is a basic app to interact with the Backend_
