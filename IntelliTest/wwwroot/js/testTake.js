let submitted = false;
let windowsChangedStartTime = null;

// const form = document.querySelector("form");
// form.addEventListener("submit", handleSubmit);
window.onsubmit = handleSubmit;

checkboxes = []
i = 0
function RemoveHiddenCheckboxes() {
    document.querySelectorAll('input[type=hidden]').forEach(e => {
        let newI = e.name.match(/\d+/g)[0]
        if (newI !== i) {
            i = newI
            checkboxes.push([])
        }
        checkboxes[i].push(false)
        e.remove();
    });
}

RemoveHiddenCheckboxes();

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
        $("form").trigger('submit');
    }
    timer.innerHTML = ("0" + min).slice(-2) + ":" + ("0" + sec).slice(-2);
    expected += interval;
    setTimeout(step, Math.max(0, interval - dt));
}
document.addEventListener("visibilitychange", () => {
    if (!submitted) {
        if (windowsChangedStartTime == null) {
            windowsChangedStartTime = Date.now();
        }
        else if (Date.now() - windowsChangedStartTime > 3000) {
            $("form").trigger('submit');
        }
    }
});
mathFields = [];

[...document.getElementsByClassName("math-field")].forEach(mathFieldSpan=>{
    let MQ = MathQuill.getInterface(2); // for backcompat
    if (mathFieldSpan.classList[1] === "answer"){
        let mathField = MQ.MathField(mathFieldSpan, {
            spaceBehavesLikeTab: true,
        });
        mathFields.push(mathField)
    }
    else{
        let fillInTheBlank = MQ.StaticMath(mathFieldSpan);
    }
})
function GetByDictKey(allowed, value) {
    return Object.keys(value)
        .filter(key => key.includes(allowed))
        .reduce((obj, key) => {
                obj[key] = value[key];
                return obj;
            },
            {});
}

function isQuestionEmpty(question) {
    return Object.keys(question).length === 0;
}

function GetAnswerIndexes(questionParams, questionIndex) {
    let selectedAnswers = Object.keys(GetByDictKey(questionIndex + ".AnswerIndexes", questionParams));
    let indexes = checkboxes[questionIndex.match(/\d+/g)[0]]
    for (const selectedAnswer of selectedAnswers) {
        indexes[parseInt(selectedAnswer.match(/\d+/g)[1])] = true
    }
    return indexes;
}

function GetParameter(questionParams, questionIndex, parameter, isMultiple = false) {
    let foundValues = Object.values(GetByDictKey(questionIndex + "." + parameter, questionParams));
    if (isMultiple) {
        return foundValues;
    } else {
        return foundValues[0];
    }
}

function createOpenQuestion(questionParams, index) {
    let questionIndex = `OpenQuestions[${index}]`;
    return{
        answer: GetParameter(questionParams, questionIndex, "Answer"),
        maxScore: 0,
        text: "",
        isEquation: false,
        imagePath: ""
    }
}
function createClosedQuestion(questionParams, index) {
    let questionIndex = `ClosedQuestions[${index}]`;
    return {
        answerIndexes: GetAnswerIndexes(questionParams, questionIndex, "AnswerIndexes"),
        maxScore: 0,
        text: "",
        isEquation: false,
        imagePath: "",
        possibleAnswers: []
    }
}

function handleSubmit(event) {
    event.preventDefault();
    submitted = true;
    localStorage.clear();
    const data = new FormData(event.target);
    let value = Object.fromEntries(data.entries());
    let res = {
        OpenQuestions: [],
        ClosedQuestions: []
    };
    let i = 0;
    while (true) {
        let question = GetByDictKey(`OpenQuestions[${i}]`, value);
        if (isQuestionEmpty(question)) {
            break;
        } else {
            res["OpenQuestions"].push(createOpenQuestion(question, i));
            i++;
        }
    }
    for (const mathField of mathFields) {
        res["OpenQuestions"].push({
            answer: mathField.latex(),
            maxScore: 0,
            text: "",
            isEquation: false,
            imagePath: ""
        })
    }
    i = 0;
    while (true){
        let question = GetByDictKey(`ClosedQuestions[${i}]`, value);
        if (isQuestionEmpty(question)) {
            break;
        } else {
            res["ClosedQuestions"].push(createClosedQuestion(question, i));
            i++;
        }
    }
    res["Time"] = min*60 + sec;
    res["Title"] = "";
    res["QuestionOreder"] = []
    res["Id"] = id
    console.log(res);
    console.log(JSON.stringify(res))
    $.ajax({
        url: "/Tests/Take/" + id,
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(res),
        success: function (response) {
            console.log(response)
            if (response === "redirect") {
                window.location.href = `/Tests/Review/${id}/${studentId}`;
            }
            $(body).html(response);
        }
    });
}