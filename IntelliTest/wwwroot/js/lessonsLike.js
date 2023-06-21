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