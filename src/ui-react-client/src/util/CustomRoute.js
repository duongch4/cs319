import React from "react";
import { Route, Redirect } from "react-router-dom";

const renderMergedProps = (component, ...rest) => {
    const finalProps = Object.assign({}, ...rest);
    return (
        React.createElement(component, finalProps)
    );
};

export const PropsRoute = ({ component, ...rest }) => {
    return (
        <Route
            {...rest}
            render={
                (routeProps) => {
                    return renderMergedProps(component, routeProps, rest);
                }
            }
        />
    );
};

export const PrivateRoute = ({ isLoggedIn, component, redirectTo, ...rest }) => {
    return (
        <Route
            {...rest}
            render={
                (routeProps) => {
                    if (isLoggedIn) {
                        return renderMergedProps(component, routeProps, rest);
                    }
                    else {
                        return (
                            <Redirect to={
                                {
                                    pathname: redirectTo,
                                    state: { from: routeProps.location }
                                }
                            } />
                        );
                    }
                }
            }
        />
    );
};
