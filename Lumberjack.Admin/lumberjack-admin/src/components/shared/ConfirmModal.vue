<template>
  <vue-final-modal
    v-model="showModal"
    classes="modal-dialog modal-dialog-centered"
    content-class="modal-content"
  >
    <div class="modal-header">
      <h5 class="modal__title">{{ title }}</h5>
    </div>
    <div class="modal-body">
      {{ content }}
    </div>
    <div class="modal-footer">
      <button type="button" class="btn btn-secondary" @click="close">
        Close
      </button>
      <button type="button" class="btn btn-primary" @click="confirm">
        Confirm
      </button>
    </div>
  </vue-final-modal>
</template>
<script>
import { ref, watch } from "vue";
export default {
  name: "ConfirmModal",
  props: {
    title: String,
    content: String,
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
    confirm() {
      this.$emit("confirm");
    },
    close() {
      this.$emit("close");
    },
  },
};
</script>