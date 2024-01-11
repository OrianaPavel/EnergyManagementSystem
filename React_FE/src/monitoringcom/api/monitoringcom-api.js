import { MONITORINGCOMSERVICE } from '../../commons/hosts';
import RestApiClient from "../../commons/api/rest-client";

const endpoint = {
    measurement: '/measurement'
};

function getHeaders(isJson = true) {
    const headers = {
        'Authorization': 'Bearer ' + localStorage.getItem('token')
    };
    if (isJson) {
        headers['Accept'] = 'application/json';
        headers['Content-Type'] = 'application/json';
    }
    return headers;
}

function getHourlyConsumption(deviceId, date, callback) {
    let request = new Request(MONITORINGCOMSERVICE.backend_api + endpoint.measurement + '/' + deviceId + '/' + date, {
        method: 'GET',
        headers: getHeaders()
    });
    RestApiClient.performRequest(request, callback);
}

export {
    getHourlyConsumption
};
