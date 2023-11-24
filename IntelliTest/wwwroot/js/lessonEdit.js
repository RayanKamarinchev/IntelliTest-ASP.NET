BalloonBlockEditor
    .create(document.querySelector('.editor'), {
        licenseKey: '',
    })
    .then(editor => {
        window.editor = editor;
        editor.setData(title);
    })
let validationSpan = document.getElementById("validation");
function Submit() {
    let titleText = document.querySelector(".lessonEditor h1").textContent;
    let htmlContentText = editor.getData();
    let contentText = [...document.querySelectorAll(".lessonEditor > p")].map(e => e.textContent).join('. ');
    let schoolText = document.querySelector("#school").value;
    let subjectText = document.querySelector("#subject").value;
    let grade = document.querySelector("#grade").value;
    let url = `/Lessons/SubmitEdit/${id}`;
    let model = {
        title: titleText,
        htmlContent: htmlContentText,
        content: contentText,
        school: schoolText,
        subject: parseInt(subjectText),
        grade: parseInt(grade)
    }
    $.ajax({
        type: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                document.getElementsByName("__RequestVerificationToken")[0].value
        },
        url: url,
        data: JSON.stringify(model),
        success: function (res) {
            if (res.startsWith("/")) {
                window.location.href = res;
            }
            validationSpan.textContent = res;
        },
        error: function (er) {
            window.location.href = "/Error/" + er.status;
        }
    });
}