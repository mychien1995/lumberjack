import EventBus from "@services/eventbus";

const LayoutService = {
    loading() {
        EventBus.emit('layout-loading');
    },
    loaded() {
        EventBus.emit('layout-loaded');
    }
}

export default LayoutService;