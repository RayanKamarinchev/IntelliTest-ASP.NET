﻿function textAreaAdjust(element) {
    element.style.height = "1px";
    element.style.height = element.scrollHeight + "px";
}

[...document.querySelectorAll("div > div.questTextBox.questAnswer > div > textarea")].forEach(e => {
    textAreaAdjust(e)
});
[...document.querySelectorAll("#questions > div > div > div.customRow > div.questTextBox > textarea")].forEach(e => {
    textAreaAdjust(e)
});