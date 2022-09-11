function LocalStorageGet(key) {
    return localStorage.getItem(String(key))
}

function LocalStorageSet(cmd) {
    localStorage.setItem(String(cmd).split(',')[0], String(cmd).split(',')[1]);
}
