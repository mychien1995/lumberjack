<template>
  <div>
    <div class="row">
      <div class="col-md-12">
        <div class="card">
          <div class="card-header">
            <h4 class="card-title">Logs Stream</h4>
          </div>
          <div class="card-body">
            <form>
              <div class="row">
                <div class="col-md-3">
                  <div class="form-group">
                    <select
                      class="form-control"
                      v-model="applicationId"
                      @change="onSelectApplication"
                    >
                      <option value="">Select an application</option>
                      <option
                        v-for="(item, index) of applications"
                        :key="index"
                        v-bind:value="item.Id"
                      >
                        {{ item.ApplicationName }}
                      </option>
                    </select>
                  </div>
                </div>
              </div>
            </form>
            <div
              class="table-responsive"
              style="overflow-y: scroll; max-height: 680px"
            >
              <table class="table table-super-compact table-borderless">
                <tbody>
                  <tr v-for="(item, index) in logs" :key="index">
                    <td>
                      <div class="clamp">
                        {{ formatTime(item.Timestamp) }} - {{ item.Message }}
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
import ApplicationService from "@services/applications.service";
import moment from "moment";
let sseClient;
export default {
  name: "LogStream",
  data() {
    return {
      applicationId: "",
      applications: [],
      logs: [],
    };
  },
  mounted() {
    ApplicationService.getAll().then((response) => {
      this.applications = response.data.Data.Data;
    });
  },
  methods: {
    handleMessage(ev) {
      if (ev.ApplicationId == this.applicationId) {
        this.logs.unshift(ev);
        if (this.logs.length > 10500) {
          this.logs = this.logs.slice(0, 10000);
        }
      }
    },
    formatTime(timestamp) {
      if (!timestamp) return "";
      const utcDate = new Date(timestamp * 1000);
      return moment(utcDate).format("DD/MM/YYYY HH:mm:ss");
    },
    onSelectApplication() {
      if (!this.applicationId) return;
      this.logs = [0];
      if (!sseClient) {
        sseClient = this.$sse.create({
          url: `${window.globalApiUrl}/api/logs/source`,
          format: "json",
          polyfill: true,
        });

        sseClient.on("error", (e) => {
          console.error("lost connection or failed to parse!", e);
        });

        sseClient.on("message", this.handleMessage);
        sseClient
          .connect()
          .then(() => {
            console.log("We're connected!");
          })
          .catch((err) => {
            console.error("Failed to connect to server", err);
          });
      }
    },
  },
  beforeUnmount() {
    if (sseClient) sseClient.disconnect();
  },
};
</script>