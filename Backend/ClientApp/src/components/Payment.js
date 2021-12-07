import React, { useState, useEffect } from "react";
import { useAuth0 } from "../react-auth0-spa";
import * as weatherForecastApi from "../api/weatherForecastApi";

export const Payment = () => {
    const { getTokenSilently } = useAuth0();

    const getPayment = async () => {
        const token = await getTokenSilently();
        console.log('token', token);
        const data = await weatherForecastApi.getPayment(token);
        location.href = data.url;
    };

    useEffect(() => {
        getPayment();
    }, []);

    return (
        <div>
            <h1 id="tabelLabel">Payment</h1>
            <p>This component demonstrates payment from the server.</p>
        </div>
    );
};
