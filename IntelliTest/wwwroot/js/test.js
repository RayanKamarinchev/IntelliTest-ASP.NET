btns = document.getElementsByTagName("button");
input = Array.prototype.slice.call(btns);
input.forEach(b => b.addEventListener("click", save));
function onFocus(e) {
    e.parentNode.parentNode.parentNode.style.backgroundPosition = "0";
}
function onFocusOut(e) {
    e.parentNode.parentNode.parentNode.style.backgroundPosition = "-0.4em 0em";
}