function post(url, data, jwtToken) {
    const options = {
        method: 'POST',
        mode: 'cors',
        cache: 'default',
        credentials: 'same-origin',
        headers: {
            'Content-Type': 'application/json',
            'Token': jwtToken
        },
        redirect: 'follow',
        referrerPolicy: 'no-referrer-when-downgrade',
        body: JSON.stringify(data)
    };

    return fetch(url, options);
}

function get(url, jwtToken) {

    const options = {
        method: 'GET',
        mode: 'cors',
        cache: 'default',
        credentials: 'same-origin',
        headers: {
            'Content-Type': 'application/json',
            'Token': jwtToken
        },
        redirect: 'follow',
        referrerPolicy: 'no-referrer-when-downgrade',
    };

    return fetch(url, options);
}

function put(url, data, jwtToken) {
    const options = {
        method: 'PUT',
        mode: 'cors',
        cache: 'default',
        credentials: 'same-origin',
        headers: {
            'Content-Type': 'application/json',
            'Token': jwtToken
        },
        redirect: 'follow',
        referrerPolicy: 'no-referrer-when-downgrade',
        body: JSON.stringify(data)
    };

    return fetch(url, options);
}

function del(url, jwtToken) {
    const options = {
        method: 'DELETE',
        mode: 'cors',
        cache: 'default',
        credentials: 'same-origin',
        headers: {
            'Content-Type': 'application/json',
            'Token': jwtToken
        },
        redirect: 'follow',
        referrerPolicy: 'no-referrer-when-downgrade',
    }

    return fetch(url, options)
}

window.ajaxClient = {
    post: post,
    get: get,
    post: post,
    put: put,
    del: del
}