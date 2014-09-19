define(function () {
    require.config({
        paths: {
            'jquery': 'libs/jquery-2.1.1',
            'sammy': 'libs/sammy',
            'rsvp': 'libs/rsvp',
            'httpRequester': 'app/http-requester1',
            'controller': 'app/controller',
            'cryptojs': 'libs/cryptojs',
            'underscore': 'libs/underscoremin',
            'handlebars': 'libs/handlebars-v2.0.0',
            'bootstrap' : ''
        }
    });

    require(['jquery', 'sammy', 'controller'], function ($, Sammy, Controller) {
        var appController = new Controller('http://localhost:22415/', '#main-content', '.container');
        appController.setEventHandler();

        var app = Sammy('#main-content', function () {
            this.get("#/login", function () {
                if (appController.isLoggedIn()) {
                    window.location = '#/post';
                    return;
                }

                appController.loadLoginForm();
            });

            this.get("#/register", function () {
                appController.loadRegisterForm();
            });

            this.get("#/post", function () {
                appController.loadPostForm();
            });
        });

        app.run('#/login');
    });
});