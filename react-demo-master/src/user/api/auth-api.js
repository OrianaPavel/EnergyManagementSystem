import {HOST} from '../../commons/hosts';
import RestApiClient from "../../commons/api/rest-client";

const endpoint = {
    register: '/auth/register',
    login: '/auth/login'
};

function getHeaders(isJson = true) {
    const headers = {};
    if (isJson) {
        headers['Accept'] = 'application/json';
        headers['Content-Type'] = 'application/json';
    }
    return headers;
}

function register(user, callback){
    let request = new Request(HOST.backend_api + endpoint.register, {
        method: 'POST',
        headers: getHeaders(),
        body: JSON.stringify(user)
    });
    console.log("URL: " + request.url);
    RestApiClient.performRequest(request, (response) => {
        if (response.ok && response.token && response.userId) {
            localStorage.setItem('token', response.token);
            localStorage.setItem('userId', response.userId);
        }
        callback(response);
    });
}

function login(user, callback){
    let request = new Request(HOST.backend_api + endpoint.login, {
        method: 'POST',
        headers: getHeaders(),
        body: JSON.stringify(user)
    });
    console.log("URL: " + request.url);
    RestApiClient.performRequest(request, (response) => {
        if (response.ok && response.token && response.user) {
            localStorage.setItem('token', response.token);
            localStorage.setItem('userId', response.user.Id);
            localStorage.setItem('username', response.user.Username);
            localStorage.setItem('role', response.user.UserRole);
        }
        callback(response);
    });
}

export {
    register,
    login
};
