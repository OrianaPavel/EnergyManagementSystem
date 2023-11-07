import {HOSTUSERSERVICE} from '../../commons/hosts';
import RestApiClient from "../../commons/api/rest-client";

const endpoint = {
    user: '/user'
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

function getAllUsers(callback) {
    let request = new Request(HOSTUSERSERVICE.backend_api + endpoint.user, {
        method: 'GET',
        headers: getHeaders()
    });
    RestApiClient.performRequest(request, callback);
}

function getUserById(id, callback){
    let request = new Request(HOSTUSERSERVICE.backend_api + endpoint.user + '/' + id, {
       method: 'GET',
       headers: getHeaders()
    });
    RestApiClient.performRequest(request, callback);
}

function createUser(user, callback){
    let request = new Request(HOSTUSERSERVICE.backend_api + endpoint.user, {
        method: 'POST',
        headers: getHeaders(),
        body: JSON.stringify(user)
    });
    RestApiClient.performRequest(request, callback);
}

function updateUser(id, user, callback){
    let request = new Request(HOSTUSERSERVICE.backend_api + endpoint.user + '/' + id, {
        method: 'PUT',
        headers: getHeaders(),
        body: JSON.stringify(user)
    });
    RestApiClient.performRequest(request, callback);
}

function deleteUser(id, callback){
    let request = new Request(HOSTUSERSERVICE.backend_api + endpoint.user + '/' + id, {
        method: 'DELETE',
        headers: getHeaders()
    });
    RestApiClient.performRequest(request, callback);
}

export {
    getAllUsers,
    getUserById,
    createUser,
    updateUser,
    deleteUser
};
