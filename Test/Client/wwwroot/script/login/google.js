 function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}
var isComp = false;
var body = "";
async function People(url,out = 10000) {
    let xhr = new XMLHttpRequest();
    console.log("Get 2 " + url)
    xhr.open("GET", url,false);
    xhr.timeout = out;
    xhr.send();
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 3) {
            // loading
        }
        if (xhr.readyState == 4 && xhr.status === 200) {
            isComp = true;
            body = xhr.responseText;
        } else if (xhr.readyState == 4) {
            // error
        }
    };
    for (var i = 0; i < (out/500); i++) {
        await sleep(500);
        if (isComp) {
            break;
        }       
    }
    return body;
}

function Login(to, token) {
    return People("https://script.google.com/a/macros/sit-kashiwa.com/s/AKfycbwdRt7dzGKJpbr1fvxYR7yKd5c6y_Ig6NaRQ_F8jDzjKPoizRp_vkdZBix3OodaMC0w/exec?name=true&to=" + String(to) + "&token=" + String(token));
}