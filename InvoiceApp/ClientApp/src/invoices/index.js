import React from 'react';
import { Route, Switch } from 'react-router-dom';

import { List } from './list';
import { AddEdit } from './addEdit';
import { Exchange } from './exchange';

export function Invoices({ match }) {
    const { path } = match;

    return (
        <Switch>
            <Route exact path={path} component={List} />
            <Route path={`${path}/add`} component={AddEdit} />
            <Route path={`${path}/edit/:id`} component={AddEdit} />
            <Route path={`${path}/exchange/:id`} component={Exchange} />
        </Switch>
    );
}