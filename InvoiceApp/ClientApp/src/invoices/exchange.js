import React, { Component } from 'react';
import { invoiceServices } from '../services/invoiceServices';

export class Exchange extends Component {
    static displayName = Exchange.name;
    static currencyOptions = ["AED", "AFN", "ALL", "AMD", "ANG", "AOA", "ARS", "AUD", "AWG", "AZN", "BAM", "BBD", "BDT", "BGN", "BHD", "BIF", "BMD", "BND", "BOB", "BRL", "BSD", "BTN", "BWP", "BYN", "BZD", "CAD", "CDF", "CHF", "CLP", "CNY", "COP", "CRC", "CUC", "CUP", "CVE", "CZK", "DJF", "DKK", "DOP", "DZD", "EGP", "ERN", "ETB", "EUR", "FJD", "FKP", "FOK", "GBP", "GEL", "GGP", "GHS", "GIP", "GMD", "GNF", "GTQ", "GYD", "HKD", "HNL", "HRK", "HTG", "HUF", "IDR", "ILS", "IMP", "INR", "IQD", "IRR", "ISK", "JMD", "JOD", "JPY", "KES", "KGS", "KHR", "KID", "KMF", "KRW", "KWD", "KYD", "KZT", "LAK", "LBP", "LKR", "LRD", "LSL", "LYD", "MAD", "MDL", "MGA", "MKD", "MMK", "MNT", "MOP", "MRU", "MUR", "MVR", "MWK", "MXN", "MYR", "MZN", "NAD", "NGN", "NIO", "NOK", "NPR", "NZD", "OMR", "PAB", "PEN", "PGK", "PHP", "PKR", "PLN", "PYG", "QAR", "RON", "RSD", "RUB", "RWF", "SAR", "SBD", "SCR", "SDG", "SEK", "SGD", "SHP", "SLL", "SOS", "SRD", "SSP", "STN", "SYP", "SZL", "THB", "TJS", "TMT", "TND", "TOP", "TRY", "TTD", "TVD", "TWD", "TZS", "UAH", "UGX", "USD", "UYU", "UZS", "VES", "VND", "VUV", "WST", "XAF", "XCD", "XDR", "XOF", "XPF", "YER", "ZAR", "ZMW"];

    constructor(props) {
        super(props);
        this.id = props.match.params.id;
        this.state = {
            loading: true,
            exchange: false,
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
        this.handleBack = this.handleBack.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleChange(event) {
        this.setState({ exchange: true });
        this.populateInvoiceData(event.target.value);
    }

    handleSubmit(event) {
        event.preventDefault();
        this.updateInvoice();
    }

    handleBack() {
        window.history.back();
    }

    componentDidMount() {
        this.populateInvoiceData(null);
    }

    renderContent() {
        let invoice = this.state.invoice;

        return (
            <form onSubmit={this.handleSubmit}>
                <div className="form-row">
                    <div className="form-group col">
                        <label>Description</label>
                        <div className="font-weight-bold">
                            {invoice.description}
                        </div>
                    </div>
                </div>
                <div className="form-row">
                    <div className="form-group col">
                        <label>Supplier</label>
                        <div className="font-weight-bold">
                            {invoice.supplier}
                        </div>
                    </div>
                </div>
                <div className="form-row">
                    <div className="form-group col">
                        <label>Date</label>
                        <div className="font-weight-bold">
                            {new Date(invoice.dateIssued).toLocaleString()}
                        </div>
                    </div>
                </div>
                <div className="form-row">
                    <div className="form-group col-3">
                        <label>Amount</label>
                        <div className="font-weight-bold">
                            {invoice.amount}
                        </div>
                    </div>
                    <div className="form-group col-2">
                        <label>Currency</label>
                        <div className="font-weight-bold">
                            {invoice.currency}
                        </div>
                    </div>
                    <div className="form-group col-2">
                        <label>Exchange to:</label>
                        <div>
                            <select value={invoice.currency} onChange={this.handleChange}>
                                {Exchange.currencyOptions.map(currencyOption =>
                                    <option key={currencyOption} value={currencyOption}>{currencyOption}</option>
                                )}
                            </select>
                        </div>
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
            : this.renderContent();

        return (
            <div>
                <h1>Invoice Exchange</h1>
                <div>
                    {contents}
                </div>
            </div>
        );
    }

    async populateInvoiceData(currency) {
        const data = currency
            ? await invoiceServices.getInvoiceById(this.id, currency)
            : await invoiceServices.getInvoiceById(this.id);
        this.setState({ invoice: data, loading: false });
    }

    async updateInvoice() {
        await invoiceServices.updateInvoice(this.id, this.state.invoice)
        window.history.back();
    }
}