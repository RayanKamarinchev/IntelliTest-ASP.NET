let connection = null;
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
let lessonError;
let openQuestionAddBtn;
let closedQuestionAddBtn;
let generateBtn;
let genLesson;
let questions;
let lessonNameInput;
let promptInput;
let questionsToGenerateCount;
let mathFields;

function isNumeric(value) {
    return /^-?\d+$/.test(value);
}

function getWordsCount(str) {
    return str.trim().split(/\s+/).length;
}

function RemoveHiddenCheckboxes() {
    document.querySelectorAll('input[type=hidden]:not(.textTypeCheckbox)').forEach(e => e.remove());
}

function GetQuestionTextarea(element) {
    return element.children[1].children[0].children[0];
}

function GetAnswerTextarea(element) {
    return element.children[2].children[0].children[0];
}

function createElementFromHTML(htmlString) {
    let div = document.createElement('div');
    div.innerHTML = htmlString.trim();

    return div.firstChild;
}

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

function ToggleInputType(event){
    if (event.currentTarget.checked) {
        //Math question
        event.currentTarget.parentElement.parentElement.children[1].style.display = "none";
        event.currentTarget.parentElement.parentElement.children[2].style.display = "block";
        [...event.currentTarget.parentElement.parentElement.parentElement.parentElement.children[2].children].forEach(x=>{
            x.children[1].style.display = "none"
            x.children[2].style.display = "block"
        });
    } else {
        event.currentTarget.parentElement.parentElement.children[2].style.display = "none"
        event.currentTarget.parentElement.parentElement.children[1].style.display = "block"
        textAreaAdjust(event.currentTarget.parentElement.parentElement.children[1]);
        [...event.currentTarget.parentElement.parentElement.parentElement.parentElement.children[2].children].forEach(x=>{
            x.children[1].style.display = "block"
            textAreaAdjust(x.children[1]);
            x.children[2].style.display = "none"
        });
    }
}
function makeDeleteAble(item) {
    item.addEventListener('click',
        e => {
            let element = e.target;
            while (!element.classList.contains("questBox")) {
                element = element.parentNode;
            }
            element.remove();
            questionCount--;
        });
}

function setUpLatestFileUpload(){
    let fileSelects= document.getElementsByClassName('file-upload')
    let fileDrags      = document.getElementsByClassName('file-drag');
    fileSelects[fileSelects.length - 1].addEventListener('change', fileSelectHandler, false);

    let fileDrag = fileDrags[fileDrags.length - 1];
    fileDrag.addEventListener('dragover', fileDragHover, false);
    fileDrag.addEventListener('dragleave', fileDragHover, false);
    fileDrag.addEventListener('drop', fileSelectHandler, false);
}

function ActivateNewQuestionsMathFields(question){
    let textTypeCheckboxes = document.getElementsByClassName("textTypeCheckbox");
    let questionTextTypeCheckbox = textTypeCheckboxes[textTypeCheckboxes.length - 1];
    questionTextTypeCheckbox.addEventListener('change', ToggleInputType)


    let mathFields = document.getElementsByClassName("math-field")
    let newMathFieldsCount = 0;
    let questionChoices = [...question.children[1].children].find(x=>x.className==="choice");
    if (questionChoices == null) {
        //The question is open, so we need to activate 2 fields
        newMathFieldsCount = 2;
    }else{
        newMathFieldsCount = 1 + questionChoices.children.length;
    }
    console.log(questionChoices)
    console.log([...question.children])
    for (let i = mathFields.length - newMathFieldsCount; i < mathFields.length; i++) {
        RegisterMathField(mathFields[i]);
    }
}

function AddQuestionToDom(question) {
    makeDeleteAble(question.querySelector(".delete"));
    questions.appendChild(question);
    questionCount++;

    ActivateNewQuestionsMathFields(question);
    setUpLatestFileUpload();
}

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
            setUpFileUpload();
        });
    closedQuestionAddBtn.addEventListener("click",
        () => {
            let createdQuestion = createElementFromHTML(closedQuestionPartialView("", emptyAnswers, questionCount, 0));
            AddQuestionToDom(createdQuestion);
            setUpFileUpload();
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

function GetMathTyperParentIndex(element){
    return parseInt(element.parentElement.children[1].name.match(/\d+/g)[0])
}

function RegisterMathField(mathFieldSpan){
    let MQ = MathQuill.getInterface(2); // for backcompat
    let mathField = MQ.MathField(mathFieldSpan, {
        spaceBehavesLikeTab: true,
    });
    if (mathFieldSpan.classList[1] === "closedAnswer"){
        let index = GetMathTyperParentIndex(mathFieldSpan)
        while (mathFields["closedAnswer"].length <= index){
            mathFields["closedAnswer"].push([])
        }
        mathFields["closedAnswer"][index].push(mathField)
    }
    else
        mathFields[mathFieldSpan.classList[1]].push(mathField);
}

function GetKeysContaining(searchWord, object) {
    return Object.keys(object)
        .filter(key => key.includes(searchWord))
        .reduce((obj, key) => {
                obj[key] = object[key];
                return obj;
            },
            {});
}

function getImagePath(index){
    return document.getElementsByClassName("questBox")[index+2].children[0].children[1].children[0].getAttribute("src");
}

function GetAnswerIndexes(formValues, questionIndex) {
    let foundKeys = Object.keys(GetKeysContaining(questionIndex + ".AnswerIndexes", formValues));
    let answersCount = Object.keys(GetKeysContaining(questionIndex + ".Answers", formValues)).length;
    let indexes = [];
    for (let i = 0; i < answersCount; i++) {
        let isContained = foundKeys
            .map(k => parseInt(k
                .replace(questionIndex + ".AnswerIndexes[", "")
                .replace("]", "")))
            .some(k => k === i);
        if (isContained) {
            formValues[questionIndex + `.AnswerIndexes[${i}]`] = true
        } else {
            formValues[questionIndex + `.AnswerIndexes[${i}]`] = false
        }
    }
    return indexes;
}

function handleSubmit(event) {
    event.preventDefault();
    const data = new FormData(event.target);

    let formValues = Object.fromEntries(data.entries());
    let i = 0;
    let openQuestions = 0
    while (true) {
        let question = GetKeysContaining(`OpenQuestions[${i}]`, formValues);
        if (isQuestionEmpty(question)) {
            question = GetKeysContaining(`ClosedQuestions[${i}]`, formValues);
        } else {
            let isEquationProp = GetKeysContaining(`OpenQuestions[${i}].IsEquation`, formValues);
            let isEquation = isEquationProp[Object.keys(isEquationProp)[0]]
            if (isEquation) {
                formValues[`OpenQuestions[${i}].Text`] = mathFields["open"][openQuestions].latex()
                formValues[`OpenQuestions[${i}].Answer`] = mathFields["openAnswer"][openQuestions].latex()
            }
            formValues[`OpenQuestions[${i}].ImagePath`] = getImagePath(i);
            formValues[`QuestionsOrder[${i}]`] = 0;
            openQuestions++;
            i++;
            continue;
        }
        if (isQuestionEmpty(question)) {
            break;
        } else {
            let isEquationProp = GetKeysContaining(`ClosedQuestions[${i}].IsEquation`, formValues)
            let isEquation = isEquationProp[Object.keys(isEquationProp)[0]]
            if (isEquation) {
                formValues[`ClosedQuestions[${i}].IsEquation`] = true;
                formValues[`ClosedQuestions[${i}].Text`] = mathFields["closed"][i - openQuestions].latex()
                for (let answerIndex = 0; answerIndex < mathFields["closedAnswer"][i].length; answerIndex++) {
                    formValues[`ClosedQuestions[${i}].Answers[${answerIndex}]`] = mathFields["closedAnswer"][i][answerIndex].latex()
                }
            }
            formValues[`ClosedQuestions[${i}].ImagePath`] = getImagePath(i);
            GetAnswerIndexes(formValues, `ClosedQuestions[${i}]`);
            formValues[`QuestionsOrder[${i}]`] = 1;
        }
        i++;
    }
    formValues["id"] = id;
    formValues["title"] = document.getElementById("title").value;
    formValues["Description"] = document.getElementById("desc").value;
    formValues["time"] = parseInt(document.getElementById("time").value);
    formValues["grade"] = parseInt(document.getElementById("grade").value);

    let orderedFormValues = Object.keys(formValues).sort().reduce(
        (obj, key) => {
            obj[key] = formValues[key];
            return obj;
        },
        {}
    );

    let newQuestionsProperties = {}
    let openQuestionsIterated = -1;
    let closedQuestionsIterated = -1;
    let lastClosedQuestionObjectName = ""
    let lastOpenQuestionObjectName = ""
    for (key in orderedFormValues) {
        if (orderedFormValues.hasOwnProperty(key)) {
            if (key.includes("ClosedQuestions")){
                let closedQuestionObjectName = key.substring(0, key.indexOf('.'))
                console.log(closedQuestionObjectName)
                if (lastClosedQuestionObjectName != closedQuestionObjectName){
                    closedQuestionsIterated++;
                }
                lastClosedQuestionObjectName = closedQuestionObjectName;
                let questionPropName = key.substring(key.indexOf('.')+1)
                newQuestionsProperties[`ClosedQuestions[${closedQuestionsIterated}].${questionPropName}`] = orderedFormValues[key];
                delete orderedFormValues[key];
            }
            else if (key.includes("OpenQuestions")){
                let openQuestionObjectName = key.substring(0, key.indexOf('.'))
                if (lastOpenQuestionObjectName != openQuestionObjectName){
                    openQuestionsIterated++;
                }
                lastOpenQuestionObjectName = openQuestionObjectName;
                let questionPropName = key.substring(key.indexOf('.')+1)
                newQuestionsProperties[`OpenQuestions[${openQuestionsIterated}].${questionPropName}`] = orderedFormValues[key];
                delete orderedFormValues[key];
            }
        }
    }
    orderedFormValues = {
        ...newQuestionsProperties,
        ...orderedFormValues
    }
    console.log(orderedFormValues);

    let f = new FormData();
    for ( var key in orderedFormValues ) {
        f.append(key, orderedFormValues[key]);
    }
    fetch("/Tests/Edit/" + id, {method: "POST", body: f})
        .then(x=>x.text())
        .then((response) => {
            console.log(response)
            if (response === "redirect") {
                window.location.href = "/Tests";
            }
            $("body").html(response);
            setUp();
            // var head= document.getElementsByTagName('head')[0];
            // var script= document.createElement('script');
            // script.src= '../../js/testEdit.js';
            // head.appendChild(script);
        })
}



function isQuestionEmpty(question) {
    return Object.keys(question).length === 0;
}

function setUp(){
    RemoveHiddenCheckboxes();

    EstablishSignalRConnection();

    lessonError = document.getElementById("lessonError");
    openQuestionAddBtn = document.getElementById("openQuestionAdd");
    closedQuestionAddBtn = document.getElementById("closedQuestionAdd");
    generateBtn = document.getElementById("gen");
    genLesson = document.getElementById("genLesson");
    questions = document.getElementById("questions");
    lessonNameInput = document.getElementById("lessonName");
    promptInput = document.getElementById("prompt");
    questionsToGenerateCount = document.getElementById("questionCount");
    DeleteButtonsFunctionality();

    AddButtonsFunctionality();

    GenerateButtonsFunctionality();

    [...document.getElementsByClassName("textTypeCheckbox")].forEach(x=>x.addEventListener('change', ToggleInputType));
    mathFields = {
        open: [],
        openAnswer: [],
        closed: [],
        closedAnswer: []
    };
    [...document.getElementsByClassName("math-field")].forEach(RegisterMathField);

    btns = [...document.getElementsByClassName("addImageToQuestionBtn")];
    for (const btn of btns) {
        btn.addEventListener("click", showImageUpload)
    }

    const form = document.querySelector("form");
    form.addEventListener("submit", handleSubmit);
}

setUp();

function imageUploaderPartialView(order, questionType){
    return `<div class="form-group uploader" style="display: none">
        <input id="file-upload${questionType}${order}" class="file-upload" type="file" accept="image/*" name="${questionType}[${order
    }].Image"/>

        <label for="file-upload${questionType}${order}" for="file-upload" class="file-drag" style="float:none">
            <img alt="Preview" src="" class="hidden file-image" style="max-width: 100%">
                <div class="start">
                    <i class="fa fa-download" aria-hIdden="true"></i>
                    <div>Select a file or drag here</div>
                    <div class="hidden notimage">Please select an image</div>
                    <span class="btn btn-primary file-upload-btn">Select a file</span>
                </div>
                <div class="hidden response">
                    <div class="messages"></div>
                </div>
        </label>
    </div>`;
}

function openQuestionPartialView(question, answer, order, maxScore) {
    return `
                    <div class="questBox">
                    ${imageUploaderPartialView(order, "OpenQuestions")}
<div style="position: relative">
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
            <input value="true" type="checkbox" name="OpenQuestions[${
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
    <button class="addImageToQuestionBtn btn btn-secondary" onclick="showImageUpload(this)" type="button">Покажи Изображение</button>
</div>
        `
}

function answerPartialView(answer, order, questionOrder) {
    return `
                        <div class="questTextBox option">
  <div>
    <input type="checkbox" value="false" name="ClosedQuestions[${questionOrder}].AnswerIndexes[${
        order}]"/>
  </div>
  <textarea onkeyup="textAreaAdjust(this)" onfocus="onFocus(this)" onblur=onFocusOut(this) type="text" placeholder="Опция ${order+1}" type="text" name="ClosedQuestions[${questionOrder}].Answers[${order}]">${answer}</textarea>
  <span class="math-field closedAnswer" style="display: none" onfocus="onFocus(this)" onblur=onFocusOut(this)>${answer}</span>
  <span class="underline"></span>
</div>
            `
}

function closedQuestionPartialView(question, answers, order, maxScore) {
    return `
<div class="questBox">
  ${imageUploaderPartialView(order, "ClosedQuestions")}
  <div style="position: relative">
    <div class="questionScore questTextBox" style="width: 200px; height: 45px">
      <span>Точки:</span>
      <input type="text" value="${maxScore}" class="body_size" name="ClosedQuestions[${order
        }].MaxScore" style="width: 20px;margin-right: 30px"/>
      <button class="circle delete" type="button">
      <i class="fa-solid fa-trash"></i>
      </button>
    </div>
    <div class="customRow">
      <div class="questTextBox" style="width: calc(100% - 250px);">
        <label class="checkbox_wrap">
        <span>Урав   нение</span>
        <input value="true" type="checkbox" name="ClosedQuestions[${
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
<button class="addImageToQuestionBtn btn btn-secondary" onclick="showImageUpload(this)" type="button">Покажи Изображение</button>
            `
}