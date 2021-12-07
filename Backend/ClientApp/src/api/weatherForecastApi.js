import { handleJsonResponse, handleError } from "./apiUtils";

const baseUrl = "api/v1/";

export async function get(token) {
    try {
        const response = await fetch(baseUrl + "weatherForecast", {
            headers: { Accept: "application/json", Authorization: `Bearer ${token}` },
        });
        return handleJsonResponse(response);
    } catch (error) {
        handleError(error);
    }
}

export async function getPayment(token) {
    try {
        const response = await fetch(baseUrl + "payment", {
            headers: { Accept: "application/json", Authorization: `Bearer ${token}` },
        });
        return handleJsonResponse(response);
    } catch (error) {
        handleError(error);
    }
}

