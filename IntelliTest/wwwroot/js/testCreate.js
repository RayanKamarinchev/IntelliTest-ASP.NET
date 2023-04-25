const selectBtn = document.querySelector(".select-btn"),
    items = document.querySelectorAll(".item");
let publicSelect = document.querySelector("#publicSelect");
let classSelect = document.querySelector("#classSelect");

selectBtn.addEventListener("click", () => {
    selectBtn.classList.toggle("open");
});

items.forEach(item => {
    item.addEventListener("click", () => {
        item.classList.toggle("checked");
        console.log(item)
        item.children[0].children[1].checked = !item.children[0].children[1].checked
        let checked = document.querySelectorAll(".checked"),
            btnText = document.querySelector(".btn-text");

        if (checked && checked.length > 0) {
            btnText.innerText = `${checked.length} Selected`;
        } else {
            btnText.innerText = "Select Language";
        }
    });
})

publicSelect.addEventListener("change", (e) => {
    console.log("here")
    if (e.target.value == 3) {
        classSelect.style.display = "block";
    } else {
        classSelect.style.display = "none";
    }
})