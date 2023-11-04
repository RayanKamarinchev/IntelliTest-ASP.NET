function isNumeric(value) {
    return /^-?\d+$/.test(value);
}

function getWordsCount(str) {
    return str.trim().split(/\s+/).length;
}

function RemoveHiddenCheckboxes() {
    document.querySelectorAll('input[type=hidden]').forEach(e => e.remove());
}

RemoveHiddenCheckboxes();

let connection = null;
function EstablishSignalRConnection() {
    //Connection
    setupConnection = () => {
        connection = new signalR.HubConnectionBuilder()
            .withUrl("/testEditHub")
            .build();
        connection.on("Add",
            (question, answer) => {
                let openQuestionHtml = openQuestionPartialView(question, answer, questionCount, 0)
                let element = createElementFromHTML(openQuestionHtml);

                questions.prepend(element);
                
                textAreaAdjust(GetQuestionTextarea(element));
                textAreaAdjust(GetAnswerTextarea(element));
                questionCount++;
            });
        connection.on("WrongLesson",
            () => {
                lessonError.textCotnent = "Урокът не е намерен";
            });
        connection.on("Finished",
            function() {
                connection.stop();
            });
        connection.start()
            .catch(err => console.error(err, toString()));
    }

    setupConnection();
}
function AddQuestionToDom(question) {
    makeDeleteAble(question.querySelector(".delete"));
    questions.appendChild(question);
    questionCount++;
}

function GetQuestionTextarea(element) {
    return element.children[1].children[0].children[0];
}

function GetAnswerTextarea(element) {
    return element.children[2].children[0].children[0];
}

EstablishSignalRConnection();

function createElementFromHTML(htmlString) {
    var div = document.createElement('div');
    div.innerHTML = htmlString.trim();

    return div.firstChild;
}

let emptyAnswers = [
    {
        answer: "",
        order: 0
    },
    {
        answer: "",
        order: 1
    },
    {
        answer: "",
        order: 2
    },
    {
        answer: "",
        order: 3
    }
];
let lessonError = document.getElementById("lessonError");
let openQuestionAddBtn = document.getElementById("openQuestionAdd");
let closedQuestionAddBtn = document.getElementById("closedQuestionAdd");
let generateBtn = document.getElementById("gen");
let genLesson = document.getElementById("genLesson");
let questions = document.getElementById("questions");
let lessonNameInput = document.getElementById("lessonName");
let promptInput = document.getElementById("prompt");
let questionsToGenerateCount = document.getElementById("questionCount");

function makeDeleteAble(item) {
    item.addEventListener('click',
        e => {
            let btnElement = e.target;
            while (!btnElement.classList.contains("delete")) {
                btnElement = btnElement.parentNode;
            }
            btnElement.parentNode.parentNode.remove();
            questionCount--;
        });
}

DeleteButtonsFunctionality();

AddButtonsFunctionality();

GenerateButtonsFunctionality();

[...document.getElementsByClassName("textTypeCheckbox")].forEach(x=>x.addEventListener('change', (event) => {
    if (event.currentTarget.checked) {
        event.currentTarget.parentElement.parentElement.children[1].style.display = "none";
        event.currentTarget.parentElement.parentElement.children[2].style.display = "block";
        [...event.currentTarget.parentElement.parentElement.parentElement.parentElement.children[2].children].forEach(x=>{
            console.log(x)
            x.children[1].style.display = "none"
            x.children[2].style.display = "block"
        });
    } else {
        event.currentTarget.parentElement.parentElement.children[2].style.display = "none"
        event.currentTarget.parentElement.parentElement.children[1].style.display = "block"
        textAreaAdjust(event.currentTarget.parentElement.parentElement.children[1]);
        [...event.currentTarget.parentElement.parentElement.parentElement.parentElement.children[2].children].forEach(x=>{
            console.log(x)
            x.children[1].style.display = "block"
            textAreaAdjust(x.children[1]);
            x.children[2].style.display = "none"
        });
    }
}));
let mathFields = {
    open: [],
    openAnswer: [],
    closed: [],
    closedAnswer: []
};
function GetMathTyperParentIndex(element){
    return element.parentElement.children[1].name.match(/\d+/g)[0]
}

[...document.getElementsByClassName("math-field")].forEach(mathFieldSpan=>{
    let MQ = MathQuill.getInterface(2); // for backcompat
    let mathField = MQ.MathField(mathFieldSpan, {
        spaceBehavesLikeTab: true,
    });
    if (mathFieldSpan.classList[1] === "closedAnswer"){
        let index = GetMathTyperParentIndex(mathFieldSpan)
        console.log(mathFields);
        while (mathFields["closedAnswer"].length <= index){
            mathFields["closedAnswer"].push([])
        }
        mathFields["closedAnswer"][index].push(mathField)
    }
    else
        mathFields[mathFieldSpan.classList[1]].push(mathField);
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

function getImagePath(index){
    return document.getElementsByClassName("questBox")[index].children[0].children[1].children[0].src;
}

function handleSubmit(event) {
    event.preventDefault();
    const data = new FormData(event.target);

    let value = Object.fromEntries(data.entries());
    console.log(value)
    value["QuestionsOrder"] = []
    let i = 0;
    let openQuestions = 0
    while (true) {
        let question = GetByDictKey(`OpenQuestions[${i}]`, value);
        if (isQuestionEmpty(question)) {
            question = GetByDictKey(`ClosedQuestions[${i}]`, value);
        } else {
            let isEquation = GetByDictKey(`OpenQuestions[${i}].IsEquation`, value) === "true";
            if (isEquation) {
                value[`OpenQuestions[${i}].Text`] = mathFields["open"][openQuestions].latex()
                value[`OpenQuestions[${i}].Answer`] = mathFields["openAnswer"][openQuestions].latex()
            }
            value[`OpenQuestions[${i}].ImagePath`] = getImagePath(i);
            value["QuestionsOrder"].push(0);
            openQuestions++;
            i++;
            continue;
        }
        if (isQuestionEmpty(question)) {
            break;
        } else {
            let isEquation = GetByDictKey(`ClosedQuestions[${i}].IsEquation`, value) === "true";
            if (isEquation) {
                value[`ClosedQuestions[${i}].Text`] = mathFields["closed"][i-openQuestions].latex()
                for (let answerIndex = 0; answerIndex < mathFields["closedAnswer"].length; answerIndex++) {
                    value[`ClosedQuestions[${i}].AnswerIndexes[${answerIndex}]`] = mathFields["closedAnswer"][answerIndex][i].latex()
                }
            }
            value[`ClosedQuestions[${i}].ImagePath`] = getImagePath(i);
            value["QuestionsOrder"].push(1);
        }
        i++;
    }
    value["id"] = id;
    value["title"] = document.getElementById("title").value;
    value["Description"] = document.getElementById("desc").value;
    value["time"] = parseInt(document.getElementById("time").value);
    value["grade"] = parseInt(document.getElementById("grade").value);
    let f = new FormData();
    for ( var key in value ) {
        f.append(key, value[key]);
    }
    fetch("/Tests/Edit/" + id, {method: "POST", body: f})
        .then(x=>x.text())
        .then((response) => {
            console.log(response)
            if (response === "redirect") {
                window.location.href = "/Tests";
            }
            $("body").html(response);
        })
}



function isQuestionEmpty(question) {
    return Object.keys(question).length === 0;
}
function GetAnswerIndexes(questionParams, questionIndex) {
    let foundKeys = Object.keys(GetByDictKey(questionIndex + ".AnswerIndexes", questionParams));
    let answersCount = Object.keys(GetByDictKey(questionIndex + ".Answers", questionParams)).length;
    let indexes = [];
    for (let i = 0; i < answersCount; i++) {
        let isContained = foundKeys
            .map(k => parseInt(k
                .replace(questionIndex + ".AnswerIndexes[", "")
                .replace("]", "")))
            .some(k => k === i);
        if (isContained) {
            indexes.push(true);
        } else {
            indexes.push(false);
        }
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

const form = document.querySelector("form");
form.addEventListener("submit", handleSubmit);

function DeleteButtonsFunctionality() {
    questions.querySelectorAll(".delete").forEach(item => {
        makeDeleteAble(item);
    });
}
function AddButtonsFunctionality() {
    openQuestionAddBtn.addEventListener("click",
        () => {
            let createdQuestion = createElementFromHTML(openQuestionPartialView("", "", questionCount, 0));
            AddQuestionToDom(createdQuestion);
        });
    closedQuestionAddBtn.addEventListener("click",
        () => {
            let createdQuestion = createElementFromHTML(closedQuestionPartialView("", emptyAnswers, questionCount, 0));
            AddQuestionToDom(createdQuestion);
        });
}
function GenerateButtonsFunctionality() {
    generateBtn.addEventListener("click",
        (e) => {
            e.preventDefault();
            if (!isNumeric(questionsToGenerateCount.value) || questionsToGenerateCount.value == 0) {
                questionsToGenerateCount.value = Math.round(getWordsCount(promptInput.value) / 40);
            }
            connection.invoke("AddQuestion", promptInput.value, Number(questionsToGenerateCount.value));
        });
    genLesson.addEventListener("click",
        (e) => {
            e.preventDefault();
            if (!isNumeric(questionsToGenerateCount.value)) {
                questionsToGenerateCount.value = 0;
            }
            connection.invoke("FromLesson", lessonNameInput.value, Number(questionsToGenerateCount.value));
        });
}



function openQuestionPartialView(question, answer, order, maxScore) {
    return `
                    <div class="questBox">
    <div class="questionScore questTextBox" style="width: 200px; height: 45px">
        <span>Точки:</span>
        <input type="text" value="${maxScore}" name="OpenQuestions[${order
    }].MaxScore" class="body_size" style="width: 20px; margin-right: 30px;"/>
        <button class="circle delete" type="button">
        <i class="fa-solid fa-trash"></i>
        </button>
    </div>
    <div class="customRow">
        <div class="questTextBox" style="width: calc(100% - 250px);">
            <label class="checkbox_wrap">
            <span>Урав   нение</span>
            <input type="checkbox" name="OpenQuestions[${
        order}].IsEquation" class="checkbox_inp textTypeCheckbox">
            <span class="checkbox_mark"></span>
            </label>
            <textarea onkeyup="textAreaAdjust(this)" onfocus="onFocus(this)" onblur=onFocusOut(this) placeholder="Въпрос" type="text" name="OpenQuestions[${
        order}].Text">${question}</textarea>
            <span class="math-field open" style="display: none" onfocus="onFocus(this)" onblur=onFocusOut(this)>${question}</span>
            <span class="underline"></span>
        </div>
    </div>
    <div class="questTextBox questAnswer">
        <div>
        <span></span>
    <textarea onkeyup="textAreaAdjust(this)" onfocus="onFocus(this)" onblur=onFocusOut(this) placeholder="Отговор" type="text" value="@Model.Answer" name="OpenQuestions[${
        order}].Answer">${answer}</textarea>
    <span class="math-field openAnswer" style="display: none" onfocus="onFocus(this)" onblur=onFocusOut(this)>${answer}</span>
<span class="underline"></span>
        </div>
    </div>
</div>
        `
}

function answerPartialView(answer, order, questionOrder) {
    return `
                        <div class="questTextBox option">
                            <div>
                                    <input type="checkbox" name="ClosedQuestions[${questionOrder}].AnswerIndexes[${
        order}]"/>
                            </div>
                            <textarea onkeyup="textAreaAdjust(this)" onfocus="onFocus(this)" onblur=onFocusOut(this) type="text" placeholder="Опция @(i + 1)" type="text" name="ClosedQuestions[${questionOrder}].Answers[${order}]">${answer}</textarea>
<span class="math-field closedAnswer" style="display: none" onfocus="onFocus(this)" onblur=onFocusOut(this)>${answer}</span>
                                <span class="underline"></span>
                        </div>
            `
}

function closedQuestionPartialView(question, answers, order, maxScore) {
    return `
                <div class="questBox" name="ClosedQuestions${order}">
                    <div class="questionScore questTextBox" style="width: 200px; height: 45px">
                        <span>Точки:</span>
                                <input type="text" value="${maxScore}" class="body_size" name="ClosedQuestions[${order
        }].MaxScore" style="width: 20px;margin-right: 30px"/>
                        <button class="circle delete" type="button">
                            <i class="fa-solid fa-trash"></i>
                        </button>
                    </div>
                    <div class="customRow">
                    <label class="checkbox_wrap">
    <span>Урав   нение</span>
    <input type="checkbox" name="ClosedQuestions[${
            order}].IsEquation" class="checkbox_inp textTypeCheckbox">
    <span class="checkbox_mark"></span>
</label>
    <textarea onkeyup="textAreaAdjust(this)" onfocus="onFocus(this)" onblur=onFocusOut(this) placeholder="Въпрос" type="text"name="ClosedQuestions[${
            order}].Text">${question}</textarea>
    <span class="math-field closed" style="display: none" onfocus="onFocus(this)" onblur=onFocusOut(this)>${question}</span>
<span class="underline"></span>
                        </div>
                    </div>
                    <div class="choice" onFocus="onFocus(this)" onBlur="onFocusOut(this)">` +
        answers.map(a => answerPartialView(a.answer, a.order, order)).join('') +
        `
                    </div>
                </div>
            `
}