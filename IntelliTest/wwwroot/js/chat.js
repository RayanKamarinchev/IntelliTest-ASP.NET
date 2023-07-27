function createElementFromHTML(htmlString) {
    var div = document.createElement('div');
    div.innerHTML = htmlString.trim();

    return div.firstChild;
}

$(function () {
    $('ul#users-list').on('click',
        'li',
        function() {
            var username = $(this).data("username");
            var input = $('#message-input');

            var text = input.val();
            if (text.startsWith("/")) {
                text = text.split(")")[1];
            }

            text = "/private(" + username + ") " + text.trim();
            input.val(text);
            input.change();
            input.focus();
        });

    $('#emojis-container').on('click',
        'button',
        function() {
            var emojiValue = $(this).data("value");
            var input = $('#message-input');
            input.val(input.val() + emojiValue + " ");
            input.focus();
            input.change();
        });

    $("#btn-show-emojis").click(function() {
        $("#emojis-container").toggleClass("d-none");
    });

    $("#message-input, .messages-container, #btn-send-message, #emojis-container button").click(function() {
        $("#emojis-container").addClass("d-none");
    });

    $("#expand-sidebar").click(function() {
        $(".sidebar").toggleClass("open");
        $(".users-container").removeClass("open");
    });

    $("#expand-users-list").click(function() {
        $(".users-container").toggleClass("open");
        $(".sidebar").removeClass("open");
    });

    $(document).on("click",
        ".sidebar.open ul li a, #users-list li",
        function() {
            $(".sidebar, .users-container").removeClass("open");
        });

    $(".modal").on("shown.bs.modal",
        function() {
            $(this).find("input[type=text]:first-child").focus();
        });

    $('.modal').on('hidden.bs.modal',
        function() {
            $(".modal-body input:not(#newRoomName)").val("");
        });

    $(".alert .btn-close").on('click',
        function() {
            $(this).parent().hide();
        });

    $('body').tooltip({
        selector: '[data-bs-toggle="tooltip"]',
        delay: { show: 500 }
    });

    $("#remove-message-modal").on("shown.bs.modal",
        function(e) {
            const id = e.relatedTarget.getAttribute('data-messageId');
            $("#itemToDelete").val(id);
        });

    $(document).on("mouseenter",
        ".ismine",
        function() {
            $(this).find(".actions").removeClass("d-none");
        });

    $(document).on("mouseleave",
        ".ismine",
        function() {
            var isDropdownOpen = $(this).find(".dropdown-menu.show").length > 0;
            if (!isDropdownOpen)
                $(this).find(".actions").addClass("d-none");
        });

    $(document).on("hidden.bs.dropdown",
        ".actions .dropdown",
        function() {
            $(this).closest(".actions").addClass("d-none");
        });
});
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.start().catch(function(err) {
    return console.error(err);
});

connection.on("newMessage",
    function (messageView) {
        debugger;
        var isMine = messageView.fromFullName === username;
        var message = new ChatMessage(messageView.id,
            messageView.content,
            messageView.timestamp,
            messageView.fromUserName,
            isMine);
        chatMessages.push(message);
        appendMessage(message);
    });
connection.on("deleteMessage",
    function(msgId) {
        for (var i = 0; i < chatMessages.length; i++) {
            if (chatMessages[i].id === msgId) {
                chatMessages.splice(i, 1);
                break;;
            }
        }
        let messageElement = e;
        while (!messageElement.classList.contains("right-chat-message")) {
            messageElement = messageElement.parentNode;
        }
        messageElement.remove();
    });

connection.on("onError",
    function(message) {
        viewModel.serverInfoMessage(message);
        $("#errorAlert").removeClass("d-none").show().delay(5000).fadeOut(500);
    });

let joinedRoomName = "";
let chatMessages = [];
let noJoinedRoomPanel;
let roomContainer;
let joinedRoomNameElement;
let messagesContainer;
let messageInputField;
$(document).ready(function() {
    noJoinedRoomPanel = document.getElementById("noJoinedRoomPanel");
    roomContainer = document.getElementById("messagesPanel");
    joinedRoomNameElement = document.getElementById("joinedRoomName");
    messagesContainer = document.getElementById("messages");
    messageInputField = document.getElementById("messageInputField");
});


function joinRoom(roomName) {
    connection.invoke("Join", roomName).then(function() {
        joinedRoomName = roomName;
        viewRoomAndMessages();
        messageHistory();
    });
}

function sendToRoom(event) {
    if (('keyCode' in event && event.keyCode === 13) || !('keyCode' in event)) {
        let message = messageInputField.value;
        messageInputField.value = "";
        $.ajax({
            url: `/Messages/Create?room=${encodeURI(joinedRoomName)}&content=${encodeURI(message)}&userId=${userId}`,
            method: 'GET',
        });
    }
}

function viewRoomAndMessages() {
    noJoinedRoomPanel.style.display = "none";
    roomContainer.style.display = "block";
    joinedRoomNameElement.textContent = joinedRoomName;
}

function messageHistory() {
    fetch('/Messages/Room/' + joinedRoomName)
        .then(response => response.json())
        .then(data => {
            chatMessages = [];
            for (var i = 0; i < data.length; i++) {
                var isMine = data[i].fromFullName !== username;
                chatMessages.push(new ChatMessage(data[i].id,
                    data[i].content,
                    data[i].timestamp,
                    data[i].fromUserName,
                    isMine));
            }
            //$(".messages-container").animate({ scrollTop: $(".messages-container")[0].scrollHeight }, 1000);
            appendMessages();
        });
}

function appendMessages() {
    chatMessages.forEach(message => {
        appendMessage(message);
    });
}

function appendMessage(message) {
    if (message.isMine) {
        let messageElement = createElementFromHTML(mineMessage(message.content, message.timestampRelative));
        messagesContainer.appendChild(messageElement);
    } else {
        let messageElement = createElementFromHTML(notMineMessage(message.content,
            message.timestampRelative,
            message.id));
        messagesContainer.appendChild(messageElement);
    }
}

function notMineMessage(content, timestampFull) {
    return `
                        <div class="left-chat-message fs-13 mb-2">
                                    <p class="mb-0 mr-3 pr-4">${content}</p>
                                        <div class="message-time"">${timestampFull}</div>
                                    <div class="message-options">
                                    </div>
                                </div>
            `
}

function mineMessage(content, timestampFull, id) {
    return `
                        <div class="right-chat-message fs-13 mb-2">
                                    <div class="mb-0 mr-3 pr-4">
                                        <div class="d-flex flex-row">
                                            <div class="pr-2">${content}</div>
                                            <div class="pr-4"></div>
                                        </div>
                                    </div>
                                    <div class="message-options dark">
                                        <div class="message-time">
                                            <div class="d-flex flex-row">
                                                <div class="mr-2 message-text-tooltip">${timestampFull}</div>
                                                <div class="svg15 double-check"></div>
                                            </div>
                                        </div>
                                        <div class="message-arrow">
                                            <i class="fa-regular fa-trash-can fs-17" style="color: white;" onclick="deleteMessage(this,'${
        id}')"></i>
                                        </div>
                                    </div>
                                </div>
            `
}

function deleteMessage(e, id) {
    fetch('/Messages/Delete/' + id);
}

//self.uploadFiles = function() {
//    var form = document.getElementById("uploadForm");
//    $.ajax({
//        type: "POST",
//        url: '/Upload',
//        data: new FormData(form),
//        contentType: false,
//        processData: false,
//        success: function() {
//            $("#UploadedFile").val("");
//        },
//        error: function(error) {
//            alert('Error: ' + error.responseText);
//        }
//    });
//}

function ChatMessage(id, content, timestamp, fromUserName, isMine) {
    var self = this;

    self.id = id;
    self.content = content;
    self.timestamp = timestamp;

    var date = new Date(timestamp);
    var now = new Date();
    var diff = Math.round((date.getTime() - now.getTime()) / (1000 * 3600 * 24));

    var { dateOnly, timeOnly } = formatDate(date);
    if (diff == 0)
        self.timestampRelative = `${timeOnly}`;
    else if (diff == -1)
        self.timestampRelative = `Yestrday`;
    else
        self.timestampRelative = `${dateOnly}`;

    self.time = `${timeOnly}`;
    self.fromUserName = fromUserName;
    self.isMine = isMine;
}

function formatDate(date) {
    // Get fields
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var day = date.getDate();
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var d = hours >= 12 ? "PM" : "AM";

    // Correction
    if (hours > 12)
        hours = hours % 12;

    if (minutes < 10)
        minutes = "0" + minutes;

    // Result
    var dateOnly = `${day}/${month}/${year}`;
    var timeOnly = `${hours}:${minutes} ${d}`;
    var fullDateTime = `${dateOnly} ${timeOnly}`;

    return { dateOnly, timeOnly, fullDateTime };
}