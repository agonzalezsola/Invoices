import React, { Component } from 'react';

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <div>
        <h1>Frontend for InvoicesAPI</h1>
        <p>This is a simple app for the InvoiceAPI. What can you do with this app?</p>
        <ul>
          <li>View the list of all Invoices</li>
          <li>Add, edit and delete Invoices</li>
          <li>Make currency exchanges to Invoices</li>
        </ul>
        <p>This is my first React app ;)</p>
      </div>
    );
  }
}
