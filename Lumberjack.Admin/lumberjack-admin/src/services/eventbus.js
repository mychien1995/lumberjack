import mitt from 'mitt';
const EventBus = {
    emitter: mitt(),
    on(channel, eventHanlder) {
        this.emitter.on(channel, eventHanlder);
    },

    emit(channel, event) {
        this.emitter.emit(channel, event);
    }
}
export default EventBus;