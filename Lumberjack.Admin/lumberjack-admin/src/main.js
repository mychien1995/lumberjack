import { createApp } from 'vue'
import App from './App.vue'
import router from "./router";
import ApiService from './services/api.service';
import NotificationService from './services/notification.service';
import VueToast from 'vue-toast-notification';
import { vfmPlugin } from 'vue-final-modal'

const app = createApp(App).use(router).use(VueToast).use(vfmPlugin);
ApiService.init();
NotificationService.init(app);
app.mount('#app');