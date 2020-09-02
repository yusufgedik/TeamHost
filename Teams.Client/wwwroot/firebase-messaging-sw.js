self.addEventListener('push', function (event) {
    if (event && event.data) {
        self.pushdata = event.data.json();
        if (self.pushdata) {
            event.waitUntil(
                self.registration.showNotification(self.pushdata.data.title, {
                body: self.pushdata.data.body,
                    icon: self.pushdata.data.icon,
                    url: self.pushdata.data.url

            }).then(function () {
            }));
        }
    }
});