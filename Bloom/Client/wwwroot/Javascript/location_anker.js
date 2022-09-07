function Org() {
    var result = "commite"
    if (location.href.indexOf("club") != -1) {
        result = "club"
    } else if (location.href.indexOf("org") != -1) {
        result = "org"
    } else if (location.href.indexOf("shool") != -1) {
        result = "shool"
    }

    return result;
}