window.onload += function() {
    if (!('serviceWorker' in navigator))
        return;

    navigator.serviceWorker.getRegistration()
        .then(registration => {
            if (registration.waiting != null) {
                // registration.unregister();  // 効果が疑わしいので保留
                console.log('インストール済みの更新があります。アプリを再起動してください。');
                disableUpdateButton();
            }
            else {
                registration.update()
                    .then(registration => {
                        const installingWorker = registration.installing;
                        if (installingWorker != null) {
                            installingWorker.onstatechange = e => {
                                if (e.target.state == 'installed') {
                                    // registration.unregister();  // 効果が疑わしいので保留
                                    console.log('更新がインストールされました。アプリを再起動してください。');
                                    disableUpdateButton();
                                }
                            }
                        }
                        else {
                            console.log('更新はありませんでした。');
                        }
                    });
            }
        });
}