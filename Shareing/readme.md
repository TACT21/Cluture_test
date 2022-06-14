# Deta Shareing system

## What is this

This is a system to share some data with different origin.

## How to use it
Included below code with where you want.

 ```HTML:hoge.html
    <iframe id="iframe" src="http://A.com/index.html" style="height:0"></iframe>
    <script>
        class Shareing{
            var value = "";
            var isComp = false;
            var isUsed = false;

            function sleep(ms) {
                return new Promise(resolve => setTimeout(resolve, ms));
            }

            async function GetValue(valueTag,settimeout){
                if(isUsed){
                   throw; 
                }
                isUsed = true;
                var iframe = document.getElementById('iframe');
                iframe.postMessage({
                action: 'CallValue',
                message: valueTag
                }, '*', );
                for (var i = 0; i < (out / 500); i++) {
                    await sleep(500);
                    if (isComp) {
                        break;
                    }
                }
                isComp = false;
                isUsed = false;
                return value;
            }

            function ToLocal(tag,value,shareOrigin,shareAddress){
                var iframe = document.getElementById('iframe');
                iframe.postMessage({
                action: 'SetValue',
                tag:tag,
                message: value,
                Origin: shareOrigin
                adress: shareAddress
                }, '*', );
            }

            function ToSession(value,shareOrigin,shareAddress){
                var iframe = document.getElementById('iframe');
                iframe.postMessage({
                action: 'SendValue',
                tag:tag,
                message: value,
                Origin: shareOrigin
                adress: shareAddress
                }, '*', );
            }

            async function TrySet(type,value,shareOrigin,shareAddress,timeout = 5000){
                if(isUsed){
                    throw; 
                }
                isUsed = true;
                if(type == "Local"){
                    ToLocal(value,shareOrigin,shareAddress)
                }else if (type == "Session"){
                    ToSession(value,shareOrigin,shareAddress)
                }
                for (var i = 0; i < (out / 500); i++) {
                    await sleep(500);
                    if (isComp) {
                        break;
                    }
                }
                isUsed = false;
                isComp = false;
                return value;
            }

            window.addEventListener('message', function (e) {
                switch (e.data.action) {
                    case 'ReturnValue':
                        isComp = true;
                        value = e.data.message;
                        break;
                }
            });
        } 
    </script>
```

And than you call each function where you want.

Good luck!