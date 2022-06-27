console.log("Loading..");

var value = "";
var isComp = false;
var isUsed = false;
var body = "NULL";

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

window.convertArray = (win1251Array) => {
    var win1251decoder = new TextDecoder('windows-1251');
    var bytes = new Uint8Array(win1251Array);
    var decodedArray = win1251decoder.decode(bytes);
    console.log(decodedArray);
    return decodedArray;
};

window.addEventListener('message', function (event) {
    body = event.data;
    alert(event.data);
});

async function Get(tag, id) {
    var unit = 500
        document.querySelector('#hippocampus').contentWindow.postMessage('get' + ',' + tag);
    for (var i = 0; i < (5000 / unit); i++) {
        sleep(unit);
        if (body != "NULL") {
            break;
        }
    }
    var result = body;
    body = "NULL";
    return result
}

function Del(tag, id) {
    if (isLoad) {
        document.querySelector('#hippocampus').contentWindow.postMessage('del' + ',' + tag);
        return "Del comannd is completed";
    }
    return "Ifream is not opened";
}

function Set(tag, content, id) {
    if (isLoad) {
        document.querySelector('#hippocampus').contentWindow.postMessage('get' + ',' + tag + ',' + content);
        return "Set comannd is completed";
    }
    return "Ifream is not opened";
}

async function People(url, out = 10000) {
    window.open(url, "child_window", "width=500,height=750,scrollbars=yes");
    for (var i = 0; i < (out / 500); i++) {
        await sleep(500);
        if (isComp) {
            break;
        }
    }
    isComp = false;
    console.log(body);
    return body;
}

window.Login= () => {
    return People("https://script.google.com/a/macros/sit-kashiwa.com/s/AKfycbwdRt7dzGKJpbr1fvxYR7yKd5c6y_Ig6NaRQ_F8jDzjKPoizRp_vkdZBix3OodaMC0w/exec");
}

function parentFunc(content) {
    console.log(content);
    isComp = true;
    body = content;
}