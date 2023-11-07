import {HOSTUSERSERVICE} from '../../commons/hosts';
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

function register(user, callback) {
    let request = new Request(HOSTUSERSERVICE.backend_api + endpoint.register, {
        method: 'POST',
        headers: getHeaders(),
        body: JSON.stringify(user)
    });
    
    console.log("test ++ " + HOSTUSERSERVICE.backend_api);
    console.log("URL: " + request.url);
    
    RestApiClient.performRequest(request, (data, statusCode, error) => {
        if (data && data.token && data.userId) {
            localStorage.setItem('token', data.token);
            localStorage.setItem('userId', data.userId);

            callback(statusCode);
        } else {
            console.error("Error during registration", error);
            callback(null);
        }
    });
}

function login(user, callback) {
    let request = new Request(HOSTUSERSERVICE.backend_api + endpoint.login, {
        method: 'POST',
        headers: getHeaders(),
        body: JSON.stringify(user)
    });
    
    console.log("URL: " + request.url);
    
    RestApiClient.performRequest(request, (data, statusCode, error) => {
        if (data && data.token && data.user) {
            localStorage.setItem('token', data.token);
            localStorage.setItem('userId', data.user.hashid);
            localStorage.setItem('username', data.user.username);
            localStorage.setItem('role', data.user.userRole); 

            callback(statusCode);
        } else {
            console.error("Error during login", error);
            callback(null);
        }
    });
}

export {
    register,
    login
};
