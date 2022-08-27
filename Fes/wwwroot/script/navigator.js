window.scrollToElementId = (elementId) => {
    console.info('scrolling to element', elementId);
    var element = document.getElementById(elementId);
    if (!element) {
        console.warn('element was not found', elementId);
        return false;
    }
    var new_element = document.createElement('div');
    new_element.style.position = "relative";
    new_element.style.top = "-5vh";
    new_element.style.height = "1px";
    element.before(new_element);
    new_element.scrollIntoView();
    new_element.remove();
    return true;
}