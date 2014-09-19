define(['jquery', 'rsvp'], function () {
    window.httpRequester = (function () {
        function getJSON(requestUrl) {
            var promise = new RSVP.Promise(function (resolve, reject) {
                $.ajax({
                    url: requestUrl,
                    type: "GET",
                    dataType: "json",
                    success: function (data) {
                        resolve(data);
                    },
                    error: function (err) {
                        reject(err);
                    }
                });
            });
            return promise;
        }

        function postJSON(requestUrl, data, sessionKey) {
            var promise = new RSVP.Promise(function (resolve, reject) {
                $.ajax({
                    url: requestUrl,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(data),
                    headers: { "X-SessionKey": sessionKey } || "",
                    dataType: "json",
                    success: function (data) {
                        resolve(data);
                    },
                    error: function (err) {
                        reject(err);
                    }
                });
            });
            return promise;
        }

        function putJSON(requestUrl, data) {
            var promise = new RSVP.Promise(function (resolve, reject) {
                $.ajax({
                    url: requestUrl,
                    type: "PUT",
                    headers: { "X-SessionKey": data },
                    success: function (data) {
                        resolve(data);
                    },
                    error: function (err) {
                        reject(err);
                    }
                });
            });
            return promise;
        }
        return {
            getJSON: getJSON,
            postJSON: postJSON,
            putJSON: putJSON
        }

    }());
});
