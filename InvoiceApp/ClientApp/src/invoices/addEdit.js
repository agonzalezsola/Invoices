import React, { Component } from 'react';
import { TextField, MenuItem, InputAdornment } from '@material-ui/core';
import { invoiceServices } from '../services/invoiceServices';

export class AddEdit extends Component {
    static displayName = AddEdit.name;
    static currencyOptions = ["AED", "AFN", "ALL", "AMD", "ANG", "AOA", "ARS", "AUD", "AWG", "AZN", "BAM", "BBD", "BDT", "BGN", "BHD", "BIF", "BMD", "BND", "BOB", "BRL", "BSD", "BTN", "BWP", "BYN", "BZD", "CAD", "CDF", "CHF", "CLP", "CNY", "COP", "CRC", "CUC", "CUP", "CVE", "CZK", "DJF", "DKK", "DOP", "DZD", "EGP", "ERN", "ETB", "EUR", "FJD", "FKP", "FOK", "GBP", "GEL", "GGP", "GHS", "GIP", "GMD", "GNF", "GTQ", "GYD", "HKD", "HNL", "HRK", "HTG", "HUF", "IDR", "ILS", "IMP", "INR", "IQD", "IRR", "ISK", "JMD", "JOD", "JPY", "KES", "KGS", "KHR", "KID", "KMF", "KRW", "KWD", "KYD", "KZT", "LAK", "LBP", "LKR", "LRD", "LSL", "LYD", "MAD", "MDL", "MGA", "MKD", "MMK", "MNT", "MOP", "MRU", "MUR", "MVR", "MWK", "MXN", "MYR", "MZN", "NAD", "NGN", "NIO", "NOK", "NPR", "NZD", "OMR", "PAB", "PEN", "PGK", "PHP", "PKR", "PLN", "PYG", "QAR", "RON", "RSD", "RUB", "RWF", "SAR", "SBD", "SCR", "SDG", "SEK", "SGD", "SHP", "SLL", "SOS", "SRD", "SSP", "STN", "SYP", "SZL", "THB", "TJS", "TMT", "TND", "TOP", "TRY", "TTD", "TVD", "TWD", "TZS", "UAH", "UGX", "USD", "UYU", "UZS", "VES", "VND", "VUV", "WST", "XAF", "XCD", "XDR", "XOF", "XPF", "YER", "ZAR", "ZMW"];

    constructor(props) {
        super(props);
        this.id = props.match.params.id;
        this.isAddMode = !this.id;
        this.state = {
            loading: true,
            loadingCurrency: false,
            invoice: {
                invoiceId: "00000000-0000-0000-0000-000000000000",
                description: "",
                supplier: "",
                dateIssued: "",
                amount: "",
                currency: "",
            }
        };

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleBack = this.handleBack.bind(this);
    }

    handleChange(event) {
        this.setState({
            invoice: {
                ...this.state.invoice,
                [event.target.name]: event.target.value,
            },
        });
    }

    handleSubmit(event) {
        event.preventDefault();

        if (!AddEdit.validateInvoice(this.state.invoice)) {
            alert('Fields with (*) are mandatory and amount must be more than 0');
            return;
        }
        
        if (this.isAddMode)
            this.createInvoice();
        else
            this.updateInvoice();
        
    }

    handleBack() {
        window.history.back();
    }

    componentDidMount() {
        if (!this.isAddMode) {
            this.populateInvoiceData();
        }
        else {
            this.setState({ loading: false });
        }
    }

    renderForm() {
        let invoice = this.state.invoice;
        let amount = this.state.loadingCurrency 
            ? <div>Loading...</div>
            : <TextField name="amount"
                required
                type="number"
                label="Amount"
                InputLabelProps={{ shrink: true }}
                placeholder="100.00"
                value={invoice.amount}
                onChange={this.handleChange}
                fullWidth
                variant="outlined"
                InputProps={{
                    endAdornment: <InputAdornment position="end">{invoice.currency}</InputAdornment>,
                }}
            />


        return (
            <form onSubmit={this.handleSubmit}>
                <div className="form-row">
                    <div className="form-group col">
                        <TextField name="description"
                            required 
                            label="Description"
                            value={invoice.description}
                            onChange={this.handleChange}
                            fullWidth
                            variant="outlined"
                        />
                    </div>
                </div>
                <div className="form-row">
                    <div className="form-group col">
                        <TextField name="supplier"
                            label="Supplier"
                            value={invoice.supplier}
                            onChange={this.handleChange}
                            fullWidth
                            variant="outlined"
                        />
                    </div>
                    
                </div>
                <div className="form-row">
                    <div className="form-group col-md-5 col-lg-4 col-xl-3">
                        <TextField name="dateIssued"
                            required
                            type="datetime-local"
                            label="Date"
                            InputLabelProps={{ shrink: true }}
                            value={invoice.dateIssued}
                            onChange={this.handleChange}
                            fullWidth
                            variant="outlined"
                        />
                    </div>
                    <div className="form-group col-sm-6 col-md-2">
                        <TextField name="currency"
                            required 
                            select
                            label="Currency"
                            InputLabelProps={{ shrink: true }}
                            value={invoice.currency}
                            onChange={this.handleChange}
                            fullWidth
                            variant="outlined"
                        >
                            {AddEdit.currencyOptions.map(currencyOption =>
                                <MenuItem key={currencyOption} value={currencyOption}>{currencyOption}</MenuItem>
                            )}
                        </TextField>
                    </div>
                    <div className="form-group col-md-3 col-sm-6">
                        {amount}
                    </div>
                </div>
                <button type="submit" className="btn btn-primary">Save</button>
                <button type="button" onClick={this.handleBack} className="btn btn-link">Cancel</button>
            </form>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderForm();
        let title = this.isAddMode
            ? <h1>New Invoice</h1>
            : <h1>Edit Inovice</h1>
        return (
            <div>
                {title}
                {contents}
            </div>
        );
    }

    async populateInvoiceData() {
        const data = await invoiceServices.getInvoiceById(this.id);
        this.setState({ invoice: data, loading: false });
    }

    async createInvoice() {
        await invoiceServices.createInvoice(this.state.invoice)
        window.history.back();
    }

    async updateInvoice() {
        await invoiceServices.updateInvoice(this.id, this.state.invoice)
        window.history.back();
    }

    static validateInvoice(invoice) {
        return true
            && invoice.description
            && invoice.amount
            && invoice.amount != 0
            && invoice.currency
            && invoice.dateIssued;
    }
}