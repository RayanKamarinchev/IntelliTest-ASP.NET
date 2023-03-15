const trigger = document.getElementById("trigger")
const filter = document.getElementsByClassName("cd-filter")[0]
const gallery = document.getElementsByClassName("cd-gallery")[0]

function toggleClass(ref, className) {
    if (ref.classList.contains(className))
        ref.classList.remove(className);
    else
        ref.classList.add(className);
}

function OnTrigger() {
    toggleClass(trigger, "filter-is-visible");
    toggleClass(filter, "filter-is-visible");
    toggleClass(gallery, "filter-is-visible");
}

function OnCloseOpen(e) {
    toggleClass(e, "closed")
    toggleClass(e.parentNode.children[1], "hide")
}
function onHover(e) {
    timeout = setTimeout(function () {
        console.log(e.children[0].children[0].children[2])
        e.children[0].children[1].style.display = "block";
        e.children[0].children[0].children[2].style.display = "block";
    }, 290);
}
function onLeave(e) {
    window.clearTimeout(timeout)
    e.children[0].children[1].style.display = "none";
    e.children[0].children[0].children[2].style.display = "none";
}

function onLike(e) {
    if (e.target.style.backgroundPositionX === "-2800px") {
        e.target.style.transitionDuration = "0s";
        e.target.style.backgroundPosition = '0 0';
    }
    else {
        e.target.style.transitionDuration = "1s";
        e.target.style.backgroundPosition = '-2800px 0';
    }
}
function warn(testId, time, title) {
    Swal.fire({
        title: 'Сигурен ли си че искаш да направиш тестът сега?',

        html: `${title} <br/> Време за работа: ${time} минути`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Да започваме!',
        cancelButtonText: 'Откажи'
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = `Take/${testId}`;
        }
    })
}