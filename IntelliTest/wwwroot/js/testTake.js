[...document.querySelectorAll("form > div > div.questTextBox.questAnswer > div > textarea")].forEach(e => {
    e.value = localStorage.getItem(e.getAttribute("name"))
});
[...document.querySelectorAll("form > div > div.choice > div > div > input[type=checkbox]")].forEach(e => {
    e.checked = JSON.parse(localStorage.getItem(e.getAttribute("name")))
    e.addEventListener("change", () => {
        localStorage.setItem(e.getAttribute("name"), e.checked);
    })
});
function textAreaAdjust(element) {
    localStorage.setItem(element.getAttribute("name"), element.value);
    element.style.height = "1px";
    element.style.height = element.scrollHeight + "px";
}