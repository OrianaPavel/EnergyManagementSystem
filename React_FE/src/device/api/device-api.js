import {HOSTDEVICESERVICE} from '../../commons/hosts';
import RestApiClient from "../../commons/api/rest-client";

const endpoint = {
    device: '/device'
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

function getAllDevices(callback) {
    let request = new Request(HOSTDEVICESERVICE.backend_api + endpoint.device, {
        method: 'GET',
        headers: getHeaders()
    });
    RestApiClient.performRequest(request, callback);
}

function getDeviceById(userId, deviceId, callback) {
    let request = new Request(HOSTDEVICESERVICE.backend_api + endpoint.device + '/' + userId + '/' + deviceId, {
        method: 'GET',
        headers: getHeaders()
    });
    RestApiClient.performRequest(request, callback);
}

function getDevicesByUserId(userId, callback) {
    console.log(HOSTDEVICESERVICE.backend_api + endpoint.device + '/' + userId);
    let request = new Request(HOSTDEVICESERVICE.backend_api + endpoint.device + '/' + userId, {
        method: 'GET',
        headers: getHeaders()
    });
    RestApiClient.performRequest(request, callback);
}

function createDevice(userId,device, callback) {
    let request = new Request(HOSTDEVICESERVICE.backend_api + endpoint.device + '/' + userId , {
        method: 'POST',
        headers: getHeaders(),
        body: JSON.stringify(device)
    });
    RestApiClient.performRequest(request, callback);
}

function updateDevice(userId, deviceId, device, callback) {
    let request = new Request(HOSTDEVICESERVICE.backend_api + endpoint.device + '/' + userId + '/' + deviceId, {
        method: 'PUT',
        headers: getHeaders(),
        body: JSON.stringify(device)
    });
    RestApiClient.performRequest(request, callback);
}

function deleteDevice(userId, deviceId, callback) {
    let request = new Request(HOSTDEVICESERVICE.backend_api + endpoint.device + '/' + userId + '/' + deviceId, {
        method: 'DELETE',
        headers: getHeaders()
    });
    RestApiClient.performRequest(request, callback);
}

export {
    getAllDevices,
    getDeviceById,
    getDevicesByUserId,
    createDevice,
    updateDevice,
    deleteDevice
};
