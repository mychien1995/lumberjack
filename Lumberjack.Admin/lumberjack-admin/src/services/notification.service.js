const NotificationService = {
    init(app) {
        this.toast = app.config.globalProperties.$toast;
    },
    success(message) {
        this.toast.success(message);
    },
    error(message) {
        this.toast.error(message);
    }
}

export default NotificationService;