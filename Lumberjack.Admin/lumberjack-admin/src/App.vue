<template>
  <div id="app">
    <Sidebar />
    <ApplicationLayout />
    <Spinner />
  </div>
</template>

<script>
import Sidebar from "./components/layouts/Sidebar.vue";
import ApplicationLayout from "./components/layouts/Layout.vue";
import Spinner from "./components/layouts/Loader.vue";
export default {
  name: "App",
  components: { Sidebar, ApplicationLayout, Spinner },
};
</script>

<style lang="css">
@import "./assets/css/bootstrap.min.css";
@import '../node_modules/vue3-date-time-picker/dist/main.css';
</style>
<style lang="scss">
@import "./assets/scss/paper-dashboard.scss";
</style>
