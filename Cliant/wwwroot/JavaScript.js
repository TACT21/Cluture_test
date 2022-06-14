console.log("Loading..");

window.addEventListener('message', function (e) {
    //if (event.origin !== "https://example.com") 
        //return;
    switch (e.data.action) {
        case 'googleLogin':
            console.log(e.data.message);
            break;
    }
});

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}
var isComp = false;
var body = "NULL";

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

class 