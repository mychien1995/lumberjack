<template>
  <div class="loader" v-if="loading">
    <div class="spinner-border text-dark" role="status">
      <span class="sr-only">Loading...</span>
    </div>
  </div>
</template>
<script>
import { ref } from "vue";
import EventBus from "@services/eventbus";
export default {
  name: "Spinner",
  setup() {
    let loading = ref(false);
    EventBus.on("layout-loading", () => {
      loading.value = true;
    });
    EventBus.on("layout-loaded", () => {
      loading.value = false;
    });
    return { loading };
  },
};
</script>