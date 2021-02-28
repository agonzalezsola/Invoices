import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { invoiceServices } from '../services/invoiceServices';

export class List extends Component {
    static displayName = List.name;

    constructor(props) {
        super(props);
        this.state = { invoices: [], loading: true };
        this.deleteRow = this.deleteRow.bind(this);
    }

    deleteRow(invoiceId, event) {
        this.delete(invoiceId);
    }

    componentDidMount() {
        this.populateInvoicesData();
    }

    renderInvoicesTable() {
        const invoices = this.state.invoices;
        return (
            <div>
                <Link to={location => `${location.pathname}/add`} className="btn btn-sm btn-success mb-2">Add</Link>
                <table className="table table-striped" aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>Description</th>
                            <th>Date</th>
                            <th>Supplier</th>
                            <th>Amount</th>
                            <th>Currency</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        {invoices.map(invoices =>
                            <tr key={invoices.invoiceId}>
                                <td>{invoices.description}</td>
                                <td>{new Date(invoices.dateIssued).toLocaleString()}</td>
                                <td>{invoices.supplier}</td>
                                <td>{invoices.amount}</td>
                                <td>{invoices.currency}</td>
                                <td>
                                    <Link to={location => `${location.pathname}/exchange/${invoices.invoiceId}`} className="btn btn-sm btn-secondary mr-1">Exchange</Link>
                                    <Link to={location => `${location.pathname}/edit/${invoices.invoiceId}`} className="btn btn-sm btn-primary mr-1">Edit</Link>
                                    <button onClick={(e) => this.deleteRow(invoices.invoiceId, e)} className="btn btn-sm btn-danger mr-1">Delete</button>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderInvoicesTable();

        return (
            <div>
                <h1 id="tabelLabel">Invoices</h1>
                {contents}
            </div>
        );
    }

    async delete(invoiceId) {
        await invoiceServices.deleteInvoice(invoiceId);
        this.populateInvoicesData();
    }

    async populateInvoicesData() {
        const data = await invoiceServices.getInvoices();
        this.setState({ invoices: data, loading: false });
    }
}