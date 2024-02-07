let groupContainers = [...document.getElementsByClassName("group-toggle-container")]
let groupBtns = [...document.getElementsByClassName("group-toggle")]

document.querySelectorAll('.group-toggle').forEach(btn => {
    btn.addEventListener("click", e => {
        groupBtns.forEach(g=>g.classList.remove("active"));
        groupContainers.forEach(g => g.style.display = "none");

        btn.classList.add("active");
        document.querySelector(`.group-toggle-container[data-group='${btn.dataset["group"]}']`).style.display = "block";
    })
})