﻿function isNumeric(value) {
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

function GetByDictKey(allowed, value) {
    return Object.keys(value)
        .filter(key => key.includes(allowed))
        .reduce((obj, key) => {
                obj[key] = value[key];
                return obj;
            },
            {});
}

function handleSubmit(event) {
    event.preventDefault();

    const data = new FormData(event.target);

    let value = Object.fromEntries(data.entries());
    console.log(value);
    let res = {
        OpenQuestions: [],
        ClosedQuestions: [],
        QuestionsOrder: []
    };
    let i = 0;
    while (true) {
        let question = GetByDictKey(`OpenQuestions[${i}]`, value);
        if (isQuestionEmpty(question)) {
            question = GetByDictKey(`ClosedQuestions[${i}]`, value);
        } else {
            res["OpenQuestions"].push(createOpenQuestion(question, i));
            res["QuestionsOrder"].push(0);
            i++;
            continue;
        }
        if (isQuestionEmpty(question)) {
            break;
        } else {
            res["ClosedQuestions"].push(createClosedQuestion(question, i));
            res["QuestionsOrder"].push(1);
        }
        i++;
    }
    res["id"] = id;
    res["title"] = document.getElementById("title").value;
    res["description"] = document.getElementById("desc").value;
    res["time"] = parseInt(document.getElementById("time").value);
    res["grade"] = parseInt(document.getElementById("grade").value);
    
    $.ajax({
        url: "/Tests/Edit/" + id,
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(res),
        success: function (response) {
            if (response === "redirect") {
                window.location.href = "/Tests";
            }
            $(body).html(response);
        }
    });
}
function isQuestionEmpty(question) {
    return Object.keys(question).length === 0;
}
function GetParameter(questionParams, questionIndex, parameter, isMultiple = false) {
    let foundValues = Object.values(GetByDictKey(questionIndex + "." + parameter, questionParams));
    if (isMultiple) {
        return foundValues;
    } else {
        return foundValues[0];
    }
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

function createOpenQuestion(questionParams, index) {
    let questionIndex = `OpenQuestions[${index}]`;
    return{
        text: GetParameter(questionParams, questionIndex, "Text"),
        answer: GetParameter(questionParams, questionIndex, "Answer"),
        maxScore: parseInt(GetParameter(questionParams, questionIndex, "MaxScore"))
    }
}
function createClosedQuestion(questionParams, index) {
    let questionIndex = `ClosedQuestions[${index}]`;
    return {
        text: GetParameter(questionParams, questionIndex, "Text"),
        answers: GetParameter(questionParams, questionIndex, "Answers", true),
        answerIndexes: GetAnswerIndexes(questionParams, questionIndex, "AnswerIndexes"), 
        maxScore: parseInt(GetParameter(questionParams, questionIndex, "MaxScore"))
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
                        <span>Points:</span>
                            <input type="text" value="${maxScore}" name="OpenQuestions[${order
        }].MaxScore" class="body_size" style="width: 20px; margin-right: 30px;"/>
                                <button class="circle delete" type="button">
                            <i class="fa-solid fa-trash"></i>
                        </button>
                    </div>
                    <div class="customRow">
                        <div class="questTextBox" style="width: calc(100% - 250px);">
                            <textarea onkeyup="textAreaAdjust(this)" onfocus="onFocus(this)" onblur=onFocusOut(this)  placeholder="Въпрос" type="text" name="OpenQuestions[${
        order}].Text">${question}</textarea>
                            <span class="underline"></span>
                        </div>
                    </div>
                    <div class="questTextBox questAnswer">
                        <div>
<textarea onkeyup="textAreaAdjust(this)" onfocus="onFocus(this)" onblur=onFocusOut(this) placeholder="Отговор" type="text" name="OpenQuestions[${
        order}].Answer">${answer}</textarea>
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
<textarea onkeyup="textAreaAdjust(this)" onfocus="onFocus(this)" onblur=onFocusOut(this) placeholder="Опция ${order + 1
        }" type="text" name="ClosedQuestions[${questionOrder}].Answers[${order}]">${answer}</textarea>

                                <span class="underline"></span>
                        </div>
            `
}

function closedQuestionPartialView(question, answers, order, maxScore) {
    return `
                <div class="questBox" name="ClosedQuestions${order}">
                    <div class="questionScore questTextBox" style="width: 200px; height: 45px">
                        <span>Points:</span>
                                <input type="text" value="${maxScore}" class="body_size" name="ClosedQuestions[${order
        }].MaxScore" style="width: 20px;margin-right: 30px"/>
                        <button class="circle delete" type="button">
                            <i class="fa-solid fa-trash"></i>
                        </button>
                    </div>
                    <div class="customRow">
                        <div class="questTextBox" style="width: calc(100% - 250px);">
<textarea onkeyup="textAreaAdjust(this)" onfocus="onFocus(this)" onblur=onFocusOut(this) placeholder="Въпрос" type="text" name="ClosedQuestions[${
        order}].Text">${question}</textarea>
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