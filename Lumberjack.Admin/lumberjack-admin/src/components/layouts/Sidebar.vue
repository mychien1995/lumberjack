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
        <li class="active">
          <router-link :to="'/'">
            <i class="nc-icon nc-bank"></i>
            <p>Dashboard</p>
          </router-link>
        </li>
        <li>
          <router-link :to="'/applications'">
            <i class="nc-icon nc-tile-56"></i>
            <p>Applications</p>
          </router-link>
        </li>
        <li>
          <router-link :to="'/logs'">
            <i class="nc-icon nc-map-big"></i>
            <p>Log Viewer</p>
          </router-link>
        </li>
        <li>
          <a href="">
            <i class="nc-icon nc-sound-wave"></i>
            <p>Log Stream</p>
          </a>
        </li>
      </ul>
    </div>
  </div>
</template>

<script>
export default {
  name: "Sidebar",
};
</script>