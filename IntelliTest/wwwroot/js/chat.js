﻿$(function () {
    $('ul#users-list').on('click', 'li', function () {
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

    $('#emojis-container').on('click', 'button', function () {
        var emojiValue = $(this).data("value");
        var input = $('#message-input');
        input.val(input.val() + emojiValue + " ");
        input.focus();
        input.change();
    });

    $("#btn-show-emojis").click(function () {
        $("#emojis-container").toggleClass("d-none");
    });

    $("#message-input, .messages-container, #btn-send-message, #emojis-container button").click(function () {
        $("#emojis-container").addClass("d-none");
    });

    $("#expand-sidebar").click(function () {
        $(".sidebar").toggleClass("open");
        $(".users-container").removeClass("open");
    });

    $("#expand-users-list").click(function () {
        $(".users-container").toggleClass("open");
        $(".sidebar").removeClass("open");
    });

    $(document).on("click", ".sidebar.open ul li a, #users-list li", function () {
        $(".sidebar, .users-container").removeClass("open");
    });

    $(".modal").on("shown.bs.modal", function () {
        $(this).find("input[type=text]:first-child").focus();
    });

    $('.modal').on('hidden.bs.modal', function () {
        $(".modal-body input:not(#newRoomName)").val("");
    });

    $(".alert .btn-close").on('click', function () {
        $(this).parent().hide();
    });

    $('body').tooltip({
        selector: '[data-bs-toggle="tooltip"]',
        delay: { show: 500 }
    });

    $("#remove-message-modal").on("shown.bs.modal", function (e) {
        const id = e.relatedTarget.getAttribute('data-messageId');
        $("#itemToDelete").val(id);
    });

    $(document).on("mouseenter", ".ismine", function () {
        $(this).find(".actions").removeClass("d-none");
    });

    $(document).on("mouseleave", ".ismine", function () {
        var isDropdownOpen = $(this).find(".dropdown-menu.show").length > 0;
        if (!isDropdownOpen)
            $(this).find(".actions").addClass("d-none");
    });

    $(document).on("hidden.bs.dropdown", ".actions .dropdown", function () {
        $(this).closest(".actions").addClass("d-none");
    });
});

$(document).ready(function () {
    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

    connection.start().then(function () {
        console.log('SignalR Started...')
        viewModel.roomList();
        viewModel.userList();
    }).catch(function (err) {
        return console.error(err);
    });

    connection.on("newMessage", function (messageView) {
        var isMine = messageView.fromUserName === viewModel.myProfile().userName();
        var message = new ChatMessage(messageView.id, messageView.content, messageView.timestamp, messageView.fromUserName, messageView.fromFullName, isMine, messageView.avatar);
        viewModel.chatMessages.push(message);
        $(".messages-container").animate({ scrollTop: $(".messages-container")[0].scrollHeight }, 1000);
    });

    connection.on("getProfileInfo", function (user) {
        viewModel.myProfile(new ProfileInfo(user.userName, user.fullName, user.avatar));
        viewModel.isLoading(false);
    });

    connection.on("addUser", function (user) {
        viewModel.userAdded(new ChatUser(user.userName, user.fullName, user.avatar, user.currentRoom, user.device));
    });

    connection.on("removeUser", function (user) {
        viewModel.userRemoved(user.userName);
    });

    connection.on("addChatRoom", function (room) {
        viewModel.roomAdded(new ChatRoom(room.id, room.name, room.admin, room.lastMessage, room.timeStamp));
    });

    connection.on("updateChatRoom", function (room) {
        viewModel.roomUpdated(new ChatRoom(room.id, room.name, room.admin, room.lastMessage, room.timeStamp));
    });

    connection.on("removeChatRoom", function (id) {
        viewModel.roomDeleted(id);
    });

    connection.on("removeChatMessage", function (id) {
        viewModel.messageDeleted(id);
    });

    connection.on("onError", function (message) {
        viewModel.serverInfoMessage(message);
        $("#errorAlert").removeClass("d-none").show().delay(5000).fadeOut(500);
    });

    connection.on("onRoomDeleted", function () {
        if (viewModel.chatRooms().length == 0) {
            viewModel.joinedRoom(null);
        }
        else {
            // Join to the first room from the list
            viewModel.joinRoom(viewModel.chatRooms()[0]);
        }
    });


    function AppViewModel() {
        var self = this;
        self.message = ko.observable("");
        self.chatRooms = ko.observableArray([]);
        self.chatUsers = ko.observableArray([]);
        self.chatMessages = ko.observableArray([]);
        self.joinedRoom = ko.observable();
        self.serverInfoMessage = ko.observable("");
        self.myProfile = ko.observable();
        self.isLoading = ko.observable(true);

        self.showAvatar = ko.computed(function () {
            return self.isLoading() == false && self.myProfile().avatar() != null;
        });

        self.showRoomActions = ko.computed(function () {
            return self.joinedRoom()?.admin() == self.myProfile()?.userName();
        });

        self.onEnter = function (d, e) {
            if (e.keyCode === 13) {
                self.sendNewMessage();
            }
            return true;
        }
        self.filter = ko.observable("");
        self.filteredChatUsers = ko.computed(function () {
            if (!self.filter()) {
                return self.chatUsers();
            } else {
                return ko.utils.arrayFilter(self.chatUsers(), function (user) {
                    var fullName = user.fullName().toLowerCase();
                    return fullName.includes(self.filter().toLowerCase());
                });
            }
        });

        self.sendNewMessage = function () {
            var text = self.message();
            if (text.startsWith("/")) {
                var receiver = text.substring(text.indexOf("(") + 1, text.indexOf(")"));
                var message = text.substring(text.indexOf(")") + 1, text.length);
                self.sendPrivate(receiver, message);
            }
            else {
                self.sendToRoom(self.joinedRoom(), self.message());
            }

            self.message("");
        }

        self.sendToRoom = function (room, message) {
            if (room.name()?.length > 0 && message.length > 0) {
                $.ajax({
                    url: `/Messages/Create?room=${room.name()}&content=${message}`,
                    method: 'GET',
                });
            }
        }

        self.sendPrivate = function (receiver, message) {
            if (receiver.length > 0 && message.length > 0) {
                connection.invoke("SendPrivate", receiver.trim(), message.trim());
            }
        }

        self.joinRoom = function (room) {
            connection.invoke("Join", room.name()).then(function () {
                self.joinedRoom(room);
                self.userList();
                self.messageHistory();
            });
        }

        self.roomList = function () {
            fetch('/Rooms/Get')
                .then(response => response.json())
                .then(data => {
                    self.chatRooms.removeAll();
                    for (var i = 0; i < data.length; i++) {
                        self.chatRooms.push(new ChatRoom(data[i].id, data[i].name, data[i].admin, data[i].lastMessage, data[i].timeStamp));
                    }

                    if (self.chatRooms().length > 0)
                        self.joinRoom(self.chatRooms()[0]);
                });
        }

        self.userList = function () {
            connection.invoke("GetUsers", self.joinedRoom()?.name()).then(function (result) {
                self.chatUsers.removeAll();
                for (var i = 0; i < result.length; i++) {
                    console.log(result[i])
                    self.chatUsers.push(new ChatUser(result[i].userName,
                        result[i].fullName,
                        result[i].avatar,
                        result[i].currentRoom,
                        result[i].device))
                    console.log("comp")
                }
            });
        }

        self.createRoom = function () {
            var roomName = $("#roomName").val();
            fetch('/Rooms/Create?name=' + roomName, {
                method: 'GET',
                headers: { 'Content-Type': 'application/json' }
            });
        }

        self.editRoom = function () {
            var roomId = self.joinedRoom().id();
            var roomName = $("#newRoomName").val();
            fetch('/Rooms/Edit/' + roomId + "?name=" + roomName);
        }

        self.deleteRoom = function () {
            fetch('/Rooms/Delete/' + self.joinedRoom().id());
        }

        self.messageHistory = function () {
            fetch('/Messages/Room/' + viewModel.joinedRoom().name())
                .then(response => response.json())
                .then(data => {
                    self.chatMessages.removeAll();
                    for (var i = 0; i < data.length; i++) {
                        var isMine = data[i].fromUserName == self.myProfile().userName();
                        self.chatMessages.push(new ChatMessage(data[i].id,
                            data[i].content,
                            data[i].timestamp,
                            data[i].fromUserName,
                            data[i].fromFullName,
                            isMine,
                            data[i].avatar))
                    }

                    $(".messages-container").animate({ scrollTop: $(".messages-container")[0].scrollHeight }, 1000);
                });
        }

        self.roomAdded = function (room) {
            self.chatRooms.push(room);
        }

        self.roomUpdated = function (updatedRoom) {
            var room = ko.utils.arrayFirst(self.chatRooms(), function (item) {
                return updatedRoom.id() == item.id();
            });

            room.name(updatedRoom.name());

            if (self.joinedRoom().id() == room.id()) {
                self.joinRoom(room);
            }
        }

        self.roomDeleted = function (id) {
            var temp;
            ko.utils.arrayForEach(self.chatRooms(), function (room) {
                if (room.id() == id)
                    temp = room;
            });
            self.chatRooms.remove(temp);
        }

        self.messageDeleted = function (id) {
            var temp;
            ko.utils.arrayForEach(self.chatMessages(), function (message) {
                if (message.id() == id)
                    temp = message;
            });
            self.chatMessages.remove(temp);
        }

        self.userAdded = function (user) {
            self.chatUsers.push(user);
        }

        self.userRemoved = function (id) {
            var temp;
            ko.utils.arrayForEach(self.chatUsers(), function (user) {
                if (user.userName() == id)
                    temp = user;
            });
            self.chatUsers.remove(temp);
        }

        self.uploadFiles = function () {
            var form = document.getElementById("uploadForm");
            $.ajax({
                type: "POST",
                url: '/Upload',
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function () {
                    $("#UploadedFile").val("");
                },
                error: function (error) {
                    alert('Error: ' + error.responseText);
                }
            });
        }
    }

    function ChatRoom(id, name, admin, lastMessage, date) {
        var self = this;
        self.id = ko.observable(id);
        self.name = ko.observable(name);
        self.admin = ko.observable(admin);
        self.lastMessage = ko.observable(lastMessage);
        self.date = ko.observable(date)
    }

    function ChatUser(userName, fullName, avatar, currentRoom, device) {
        var self = this;
        self.userName = ko.observable(userName);
        self.fullName = ko.observable(fullName);
        self.avatar = ko.observable(avatar);
        self.currentRoom = ko.observable(currentRoom);
        self.device = ko.observable(device);
    }
    function getTimestampRelative(timestamp) {
        // Get diff
        var date = new Date(timestamp());
        var now = new Date();
        var diff = Math.round((date.getTime() - now.getTime()) / (1000 * 3600 * 24));

        // Format date
        var { dateOnly, timeOnly } = formatDate(date);

        // Generate relative datetime
        if (diff == 0)
            return `Today`;
        if (diff == -1)
            return `Yestrday`;

        return `${dateOnly}`;
    }
   
    function ChatMessage(id, content, timestamp, fromUserName, fromFullName, isMine, avatar) {
        var self = this;
       
        self.id = ko.observable(id);
        self.content = ko.observable(content);
        self.timestamp = timestamp;

        var date = new Date(timestamp);
        var now = new Date();
        var diff = Math.round((date.getTime() - now.getTime()) / (1000 * 3600 * 24));

        var { dateOnly, timeOnly } = formatDate(date);
        if (diff == 0)
            self.timestampRelative = timestamp;
        else if (diff == -1)
            self.timestampRelative = `Yestrday`;
        else
            self.timestampRelative = `${dateOnly}`;

        self.time = `${timeOnly}`
        var { fullDateTime } = formatDate(date);
        self.timestampFull = fullDateTime
        self.fromUserName = ko.observable(fromUserName);
        self.fromFullName = ko.observable(fromFullName);
        self.isMine = ko.observable(isMine);
        self.avatar = ko.observable(avatar);
        self.deleteMessage = function () {
            console.log(id)
            fetch('/Messages/Delete/' + id);
        }
    }

    function ProfileInfo(userName, fullName, avatar) {
        var self = this;
        self.userName = ko.observable(userName);
        self.fullName = ko.observable(fullName);
        self.avatar = ko.observable(avatar);
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

    var viewModel = new AppViewModel();
    ko.applyBindings(viewModel);
});
