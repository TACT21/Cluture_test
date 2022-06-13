export function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}
var isComp = false;
var body = "";
export async function People(url,out = 10000) {
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
        } else if (xhr.readyState == 4) {]
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

export function Login() {
    return People("https://script.google.com/a/macros/sit-kashiwa.com/s/AKfycbyLqSuV8-BqSjYvl1Zkgb888UtrC4EMyppn5i2eHHVxCiu_QEMudjMS7jrRX3jpOKZk/exec?name=true");
}