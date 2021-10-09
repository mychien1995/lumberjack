<template>
  <vue-final-modal
    v-model="showModal"
    classes="modal-dialog modal-dialog-centered modal-lg"
    content-class="modal-content"
  >
    <div class="modal-header">
      <h5 class="modal__title">Log Detail</h5>
    </div>
    <div class="modal-body">
      <div class="form-group row">
        <div class="col-md-6">
          <label>Instance</label>:
          <div>{{ logItem?.Instance }}</div>
        </div>
        <div class="col-md-6">
          <label>Time</label>:
          <div>{{ formatTime(logItem) }}</div>
        </div>
      </div>
      <div class="form-group row">
        <div class="col-md-6">
          <label>Level</label>:
          <div>{{ formatLevel(logItem) }}</div>
        </div>
      </div>
      <div class="form-group" v-if="logItem?.Namespace">
        <label>Namespace</label>:
        <div style="overflow: hidden; text-overflow: ellipsis" v-bind:title="logItem?.Namespace">
          {{ logItem?.Namespace }}
        </div>
      </div>
      <div class="form-group" v-if="logItem?.Request">
        <label>Request</label>:
        <div>
          <div
            style="overflow: hidden; text-overflow: ellipsis"
            v-bind:title="logItem?.Request"
          >
            {{ logItem?.Request }}
          </div>
        </div>
      </div>
      <div class="form-group">
        <label>Message</label>:
        <div>
          <textarea
            disabled
            style="min-height: 150px"
            class="form-control f-12"
            v-model="message"
          ></textarea>
        </div>
      </div>
      <div class="form-group" v-if="logItem?.RequestContext">
        <label>Context</label>:
        <div>
          <textarea
            disabled
            style="min-height: 150px"
            class="form-control f-12"
            v-model="requestContext"
          ></textarea>
        </div>
      </div>
    </div>
    <div class="modal-footer">
      <button type="button" class="btn btn-secondary" @click="close">
        Close
      </button>
    </div>
  </vue-final-modal>
</template>
<script>
import { ref, watch } from "vue";
import moment from "moment";
export default {
  name: "LogDetailModal",
  props: {
    logItem: Object,
    show: Boolean,
  },
  setup(props) {
    let showModal = ref(false);
    watch(
      () => props.show,
      () => {
        showModal.value = props.show;
      }
    );
    return {
      showModal,
    };
  },
  methods: {
    close() {
      this.$emit("close");
    },
    formatTime(item) {
      if (!item) return "";
      const utcDate = new Date(item.Timestamp * 1000);
      return moment(utcDate).format("DD/MM/YYYY HH:mm:ss");
    },
    formatLevel(item) {
      if (!item) return "";
      if (item.LogLevel == 1) return "DEBUG";
      if (item.LogLevel == 2) return "INFO";
      if (item.LogLevel == 3) return "WARNING";
      if (item.LogLevel == 4) return "ERROR";
      return "OTHER";
    },
  },
  computed: {
    message() {
      return this.logItem ? this.logItem.Message : "";
    },
    requestContext() {
      return this.logItem ? this.logItem.RequestContext : "";
    },
  },
};
</script>