﻿console.log("Loading..");

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

window.ifream = (id) => {
    console.log("called")
    const parent = document.getElementsByTagName("article")[0];
    let img = document.createElement('ifream');
    img.id = id;
    img.src = "https://tact21.github.io/Cluture_test/Shareing/";
    console.log("Add!")
    parent.appendChild(img);
}

async function GetValue(valueTag, settimeout) {
    if (isUsed) {
        throw  'another task use memory helper'
    }
    isUsed = true;
    var iframe = document.getElementsByName('iframe')[0];
    iframe.postMessage({
        action: 'CallValue',
        message: valueTag
    }, '*');
    for (var i = 0; i < (settimeout / 500); i++) {
        await sleep(500);
        if (isComp) {
            break;
        }
    }
    isComp = false;
    isUsed = false;
    return value;
}

function ToLocal(tag, value, shareOrigin, shareAddress) {
    var iframe = document.getElementsByName('iframe')[0];
    iframe.postMessage({
        action: 'SetValue',
        tag: tag,
        message: value,
        Origin: shareOrigin,
        adress: shareAddress
    }, '*');
}

function ToSession(value, shareOrigin, shareAddress) {
    var iframe = document.getElementsByName('iframe')[0];
    iframe.postMessage({
        action: 'SendValue',
        tag: tag,
        message: value,
        Origin: shareOrigin,
        adress: shareAddress
    }, '*');
}

async function TrySet(type, value, shareOrigin, shareAddress, timeout = 5000) {
    var respons = "";
    if (isUsed) {
        throw  'another task use memory helper'
    }
    isUsed = true;
    if (type == "Local") {
        ToLocal(value, shareOrigin, shareAddress)
    } else if (type == "Session") {
        ToSession(value, shareOrigin, shareAddress)
    }
    for (var i = 0; i < (out / 500); i++) {
        await sleep(500);
        if (isComp) {
            respons = body;
            break;
        }
    }
    isUsed = false;
    isComp = false;
    return respons;
}

window.addEventListener('message', function (e) {
    switch (e.data.action) {
        case 'ReturnValue':
            isComp = true;
            value = e.data.message;
            break;
    }
});

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