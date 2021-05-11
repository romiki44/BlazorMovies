function my_function(message) {
    console.log("Logging from utilities: " + message);
}

function dotnetStaticInvocation() {
    DotNet.invokeMethodAsync("BlazorMovies.Client", "GetCurrentCount")
        .then(result => {
            console.log("Count from javascript: " + result);
        });
}

function dotnetInstanceInvocation(dotnetHelper) {
    dotnetHelper.invokeMethodAsync("IncrementCount");
}

function initializeInactivityTimer(dotnetHelper) {
    var timer;
    document.onmousemove = resetTimer;
    document.onkeypress = resetTimer;

    function resetTimer() {
        clearTimeout(timer);
        timer = setTimeout(logout, 10000);
    }

    function logout() {
        console.log("logout after timeout");
        dotnetHelper.invokeMethodAsync("Logout");
    }
}

function setInLocalStorage(key, value) {
    localStorage[key] = value;
}

function getFromLocalStorage(key) {
    return localStorage[key];
}

