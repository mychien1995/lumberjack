<template>
  <div class="sidebar" data-color="white" data-active-color="danger">
    <div class="logo">
      <a href="https://www.creative-tim.com" class="simple-text logo-mini">
        <div class="logo-image-small">
          <img src="/img/logo.png" />
        </div>
        <!-- <p>CT</p> -->
      </a>
      <a href="https://www.creative-tim.com" class="simple-text logo-normal">
        Lumberjack
      </a>
    </div>
    <div class="sidebar-wrapper">
      <ul class="nav">
        <li
          v-bind:class="
            !currentRoute || currentRoute == 'Dashboard' ? 'active' : ''
          "
        >
          <router-link :to="'/'">
            <i class="nc-icon nc-bank"></i>
            <p>Dashboard</p>
          </router-link>
        </li>
        <li
          v-bind:class="
            currentRoute && currentRoute.indexOf('Application') > -1
              ? 'active'
              : ''
          "
        >
          <router-link :to="'/applications'">
            <i class="nc-icon nc-tile-56"></i>
            <p>Applications</p>
          </router-link>
        </li>
        <li
          v-bind:class="currentRoute && currentRoute == 'Logs' ? 'active' : ''"
        >
          <router-link :to="'/logs'">
            <i class="nc-icon nc-map-big"></i>
            <p>Log Viewer</p>
          </router-link>
        </li>
        <li
          v-bind:class="
            currentRoute && currentRoute == 'Stream' ? 'active' : ''
          "
        >
          <router-link :to="'/stream'">
            <i class="nc-icon nc-sound-wave"></i>
            <p>Log Stream</p>
          </router-link>
        </li>
      </ul>
    </div>
  </div>
</template>

<script>
export default {
  name: "Sidebar",
  computed: {
    currentRoute() {
      return this.$route.name;
    },
  },
};
</script>