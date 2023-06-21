const panels = document.getElementById("panels");
const tabs = document.getElementById("tabs");
function Info() {
    $.ajax({
        type: 'get',
        url: 'GetPanel',
        data: { type: "info" },
        success: function (res) {
            [...tabs.children].forEach(c => c.children[0].classList.remove("active"))
            panels.innerHTML = res;
            tabs.children[0].children[0].classList.add("active");
        }
    });
}
function Results() {
    $.ajax({
        type: 'get',
        url: 'GetPanel',
        data: { type: "results" },
        success: function (res) {
            [...tabs.children].forEach(c => c.children[0].classList.remove("active"))
            panels.innerHTML = res;
            tabs.children[1].children[0].classList.add("active");
        }
    });
}

function Read() {
    $.ajax({
        type: 'get',
        url: 'GetPanel',
        data: { type: "read" },
        success: function (res) {
            [...tabs.children].forEach(c => c.children[0].classList.remove("active"));
            panels.innerHTML = res;
            tabs.children[2].children[0].classList.add("active");
            let e = document.getElementById("lessonHeading");
            e.parentNode.removeChild(e);

        }
    });
}

function Like() {
    $.ajax({
        type: 'get',
        url: 'GetPanel',
        data: { type: "like" },
        success: function (res) {
            [...tabs.children].forEach(c => c.children[0].classList.remove("active"));
            panels.innerHTML = res;
            tabs.children[3].children[0].classList.add("active");
            let e = document.getElementById("lessonHeading");
            e.parentNode.removeChild(e);

        }
    });
}

function MyTests() {
    $.ajax({
        type: 'get',
        url: 'GetPanel',
        data: { type: "myTests" },
        success: function (res) {
            [...tabs.children].forEach(c => c.children[0].classList.remove("active"));
            panels.innerHTML = res;
            tabs.children[1].children[0].classList.add("active");
        }
    });
}

var root = document.querySelector(":root");

root.style.fontSize = "16px";
function onLike(e) {
    e = e.parentNode.children[1]
    if (e.style.backgroundPositionX === "-2800px") {
        debugger;
        $.ajax({
            type: 'get',
            url: `/Lessons/Unlike?lessonId=${e.id}&userId=${userId}`
        });
        e.parentNode.children[2].textContent = parseInt(e.parentNode.children[2].textContent.replace(" Likes", "")) - 1 + " Likes";
        e.style.transitionDuration = "0s";
        e.style.backgroundPosition = '0 0';
    }
    else {
        $.ajax({
            type: 'get',
            url: `/Lessons/Like?lessonId=${e.id}&userId=${userId}`
        });
        e.parentNode.children[2].textContent = parseInt(e.parentNode.children[2].textContent.replace(" Likes", "")) + 1 + " Likes";
        e.style.transitionDuration = "1s";
        e.style.backgroundPosition = '-2800px 0';
    }
}
$("#main").css("margin-bottom", "0px");