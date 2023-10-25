import {HOST} from '../../commons/hosts';
import RestApiClient from "../../commons/api/rest-client";


const endpoint = {
    person: '/person'
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

function getPersons(callback) {
    let request = new Request(HOST.backend_api + endpoint.person, {
        method: 'GET',
        headers: getHeaders()
    });
    console.log(request.url);
    RestApiClient.performRequest(request, callback);
}

function getPersonById(params, callback){
    let request = new Request(HOST.backend_api + endpoint.person + params.id, {
       method: 'GET',
       headers: getHeaders()
    });

    console.log(request.url);
    RestApiClient.performRequest(request, callback);
}

function postPerson(user, callback){
    let request = new Request(HOST.backend_api + endpoint.person , {
        method: 'POST',
        headers: getHeaders(),
        body: JSON.stringify(user)
    });

    console.log("URL: " + request.url);

    RestApiClient.performRequest(request, callback);
}

export {
    getPersons,
    getPersonById,
    postPerson
};
