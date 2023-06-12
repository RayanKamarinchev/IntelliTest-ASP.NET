let submitted = false;
let windowsChangedStartTime = null;

[...document.querySelectorAll("form > div > div.questTextBox.questAnswer > div > textarea")].forEach(e => {
    e.value = localStorage.getItem(e.getAttribute("name"))
});
[...document.querySelectorAll("form > div > div.choice > div > div > input[type=checkbox]")].forEach(e => {
    let name = e.getAttribute("name");
    e.checked = JSON.parse(localStorage.getItem(name));
    e.addEventListener("change", () => {
        localStorage.setItem(e.getAttribute("name"), e.checked);
    })
});
function textAreaAdjust(element) {
    localStorage.setItem(element.getAttribute("name"), element.value);
    element.style.height = "1px";
    element.style.height = element.scrollHeight + "px";
}

const form = document.querySelector("form");
function handleSubmit() {
    submitted = true;
    localStorage.clear();
}
form.addEventListener("submit", handleSubmit);


function onFocus(e) {
    e.parentNode.parentNode.parentNode.style.backgroundPosition = "0";
}
function onFocusOut(e) {
    e.parentNode.parentNode.parentNode.style.backgroundPosition = "-0.4em 0em";
}
var timer = document.getElementById("timer");
var interval = 1000;
var expected = Date.now() + interval;
setTimeout(step, interval);
function step() {
    var dt = Date.now() - expected;
    sec--;
    if (sec < 0) {
        sec += 60;
        min--;
    }
    if ((min < 0 || sec <= 0) && !submitted) {
        document.forms["test"].submit();
    }
    timer.innerHTML = ("0" + min).slice(-2) + ":" + ("0" + sec).slice(-2);
    expected += interval;
    setTimeout(step, Math.max(0, interval - dt));
}
document.addEventListener("visibilitychange", () => {
    debugger;
    if (document.hidden && !submitted) {
        if (windowsChangedStartTime == null) {
            windowsChangedStartTime = Date.now();
        }
        else if (Date.now() - windowsChangedStartTime > 3000) {
            document.forms["test"].submit();
        }
    }
});