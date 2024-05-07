
// TODO: funciton to enable/diable logging (could cause preformance issue/could be env based)

export async function log(userHash, level, category, message, jwtToken) {
    let logWebServiceUrl = await fetchLogServiceUrl()

    let log = {
        userHash: userHash,
        level: level,
        category: category,
        message: message
    }

    let request = ajaxClient.post(logWebServiceUrl, log, jwtToken)

    return new Promise(function (resolve, reject) {
        request.then(function (response) {
            return response.json()
        }).then(function (response) {
            resolve(response)
        }).catch(function (error) {
            reject(error)
        })
    })
}

export async function logLogin(userHash, status="failure")
{
    let authWebServiceUrl = await fetchLogServiceUrl("auth")

    let message = ""
    let level = ""
    if (status == "Success") {
        level = "Info"
        message = `${userHash} successfully log in`

    }
    else {
        level = "ERROR"
        message = `${userHash} failed to log in`
    }

    let log = {
        userHash: userHash,
        level: level,
        category: "Business",
        message: message
    }

    let request = ajaxClient.post(authWebServiceUrl, log)

    return new Promise(function (resolve, reject) {
        request.then(function (response) {
            return response.json()
        }).then(function (response) {
            resolve(response)
        }).catch(function (error) {
            reject(error)
        })
    })
}

export async function logReg(userHash)
{
    let authWebServiceUrl = await fetchLogServiceUrl("auth")

    let user
    let status
    if (userHash != null) {
        user = userHash
        status = "Success"
    } else {
        user = "System"
        status = "Failure"
    }

    let message = ""
    let level = ""
    if (status == "Success") {
        level = "Info"
        message = `${user} successfully registered an account`

    }
    else {
        level = "ERROR"
        message = `User Account registration failed`
    }

    let log = {
        userHash: user,
        level: level,
        category: "Business",
        message: message
    }

    let request = ajaxClient.post(authWebServiceUrl, log)

    return new Promise(function (resolve, reject) {
        request.then(function (response) {
            return response.json()
        }).then(function (response) {
            resolve(response)
        }).catch(function (error) {
            reject(error)
        })
    })

}

export async function logPageAccess(userHash, page, jwtToken)
{
    let logWebServiceUrl = await fetchLogServiceUrl()

    let accessLog = {
        userHash: userHash,
        level: "Info",
        category: "View",
        message: `${userHash} accessed ${page}`
    }

    let request = ajaxClient.post(logWebServiceUrl, accessLog, jwtToken)

    return new Promise(function (resolve, reject) {
        request.then(function (response) {
            return response.json()
        }).then(function (response) {
            resolve(response)
        }).catch(function (error) {
            reject(error)
        })
    })
}

async function fetchLogServiceUrl(type=null) {
    const response = await fetch('../lifelog-config.url.json');
    const data = await response.json();
    
    if (type == "auth") {
        return data.LifelogUrlConfig.Log.LogWebService + data.LifelogUrlConfig.Log.Auth;
    }

    return data.LifelogUrlConfig.Log.LogWebService;
}