define(['handlebars', 'rsvp', 'jquery', 'cryptojs', 'httpRequester', 'underscore'], function () {
    var Controller = (function () {

        var Controller = function (resourceUrl, content, container) {
            this.resourceUrl = resourceUrl;
            this.content = content;
            this.container = container;
            this.page = 0;
        }

        Controller.prototype.loadLoginForm = function () {
            $(this.content).load('src/view/login-template.html');
        };

        Controller.prototype.loadRegisterForm = function () {
            $(this.content).load('src/view/register-template.html');
        };

        Controller.prototype.loadPostForm = function () {
            var _this = this;

            $(this.content).load("src/view/notes-list-template.html");//, function () {
                //_this.setStage();
           // });
        };

        Controller.prototype.setStage = function () {
            var user = this.getUserInfo(),
                _this = this;
            $('#username').html(user.Username);

            //_this.loadPosts();
        };

        Controller.prototype.sortPosts = function (allPosts, sortBy, order) {
            var sortedPosts;
            if (sortBy === 'title') {
                sortedPosts = _.sortBy(allPosts, function (post) { return post.title.toLowerCase() });
            }
            else {
                sortedPosts = _.sortBy(allPosts, sortBy);
            }
            if (order === 'descending') {
                sortedPosts = sortedPosts.reverse();
            }

            return sortedPosts;
        };

        Controller.prototype.isLoggedIn = function () {
            var user = this.getUserInfo();
            return (user.username != null) && (user.sessionKey != null);
        };

        Controller.prototype.getUserInfo = function () {
            var username = localStorage.getItem("username");
            var sessionKey = localStorage.getItem("sessionKey");

            return { Username: username, SessionKey: sessionKey };
        };

        Controller.prototype.saveUserData = function (userData) {
            localStorage.setItem("username", userData.Username);
            localStorage.setItem("sessionKey", userData.SessionKey);
        };

        Controller.prototype.setEventHandler = function () {
            var _this = this,
                username, nickname, noteText, notePriority, noteDeadline, note, password, encryptedPass, user, createTitle, createPassword, createNumber, game, post, title, body, noteTitle;

            $(this.container).on('click', '#create-note-btn', function () {
                $('#main-content').load('src/view/create-note-template.html')
            });

            $(this.content).on('click', '#submit-note', function () {
                noteTitle = $('#note-title').val();
                noteText = $('#note-text').val();
                notePriority = $('#priority').val();
                noteDeadline = $('#deadline').val();
                var today = new Date();
                var dd = today.getDate();
                var mm = today.getMonth() + 1; //January is 0!

                var yyyy = today.getFullYear();
                if (dd < 10) {
                    dd = '0' + dd
                }
                if (mm < 10) {
                    mm = '0' + mm
                }
                var today = yyyy + '-' + mm + '-' + dd;
                note = { Title: noteTitle, Text: noteText, EntryDate: today, DueDate: noteDeadline, Priority: notePriority };

                currentUser = _this.getUserInfo();

                $.ajax({
                    url: _this.resourceUrl + 'api/Note/create?sessionKey=' + currentUser.SessionKey,
                    type: "POST",
                    data: note
                }).then(function (data) {
                    window.location = '#/post';
                }, function (err) {
                    console.log(err)
                });
            });

            $(this.content).on('click', '#login-btn', function () {
                username = $('#login-username').val();
                password = $('#login-password').val();
                if (username != "" && (username.length < 6 || username.length > 40)) {
                    window.location = '#/login';
                    $('#main-content').append($('<div/>').html('Username must be between 6 and 40 symbols!'))
                }
                //encryptedPass = CryptoJS.SHA1(password).toString();
                user = { Username: username, AuthCode: password };

                $.ajax({
                    url: _this.resourceUrl + 'api/User/login',
                    type: "POST",
                    data: user
                }).then(function (data) {
                    _this.saveUserData(data);
                    window.location = '#/post';
                }, function (err) {
                    console.log(err)
                });
            });

            $(this.container).on('click', '#sign-up-form-btn', function () {
                window.location = '#/register';
            });

            $(this.container).on('click', '#login-form-btn', function () {
                window.location = '#/login';
            });

            $(this.container).on('click', '#logout-btn', function () {
                currentUser = _this.getUserInfo();

                $.ajax({
                    url: _this.resourceUrl + 'api/User/logout?sessionKey=' + currentUser.SessionKey,
                    type: "GET",
                }).then(function (data) {
                    localStorage.clear();
                    window.location = '#/login';
                }, function (err) {
                    console.log(err)
                });
            });

            $(this.content).on('click', '#register-btn', function () {
                username = $('#reg-username').val();
                if (username != "" && (username.length < 6 || username.length > 40)) {
                    window.location = '#/register';
                    $('#main-content').append($('<div/>').html('Username must be between 6 and 40 symbols!'))
                }
                password = $('#reg-password').val();
                //encryptedPass = CryptoJS.SHA1(password).toString();
                user = { Username: username, AuthCode: password };

                $.ajax({
                    url: _this.resourceUrl + 'api/User/register',
                    type: "POST",
                    data: user
                }).then(function (data) {
                    window.location = '#/login';
                    alert("Success");
                }, function (err) {
                    console.log(err)
                });;

                return false;
            });

            $(this.content).on('click', '#send-post', function () {
                currentUser = _this.getUserInfo();
                title = $('#title-post').val();
                body = $('#body-post').val();
                post = { title: title, body: body };

                httpRequester.postJSON(_this.resourceUrl + 'post', post, currentUser.sessionKey)
                             .then(function (data) {
                                 _this.loadPosts();
                                 console.log(data);
                             },
                                   function (err) { console.log(err) });
            });

            $(this.content).on('click', '#posts-btn', function () {
                $(_this.content).load("views/postforma1.html", function () {
                    _this.setStage();
                    $('#send-new-post').hide();
                });
            });

            $(this.content).on('change', '#sortby', function () {
                _this.loadPosts();
            });

            $(this.content).on('change', '#sort-order', function () {
                _this.loadPosts();
            });

            //prevents from entergin text
            $(this.content).on('keypress', '#posts-per-page', function (e) {
                var key_codes = [48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 0, 8];

                if (!($.inArray(e.which, key_codes) >= 0)) {
                    e.preventDefault();
                }
            });

            $(this.content).on('change', '#posts-per-page', function () {
                _this.loadPosts();
            });

            $(this.content).on('click', '#prev-page', function () {
                if (_this.page > 0) {
                    _this.page--;
                }
                _this.loadPosts();
            });

            $(this.content).on('click', '#next-page', function () {
                _this.page++;
                _this.loadPosts();
            });
        };

        return Controller;
    }());

    return Controller;
});