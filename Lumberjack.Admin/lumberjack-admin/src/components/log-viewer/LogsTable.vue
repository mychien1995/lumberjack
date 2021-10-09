<template>
  <div class="row">
    <div class="col-md-12">
      <div class="card">
        <div class="card-header">
          <h4 class="card-title">Logs Viewer</h4>
        </div>
        <div class="card-body">
          <form @submit.prevent="loadData()">
            <div class="row">
              <div class="col-md-3">
                <div class="form-group">
                  <select class="form-control" v-model="query.ApplicationId">
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
              <div class="col-md-3">
                <div class="form-group">
                  <input
                    class="form-control"
                    placeholder="Instance"
                    v-model="query.Instance"
                  />
                </div>
              </div>
              <div class="col-md-3">
                <div class="form-group">
                  <input
                    class="form-control"
                    placeholder="Text"
                    v-model="query.Text"
                  />
                </div>
              </div>
              <div class="col-md-3">
                <div class="form-group">
                  <select class="form-control" v-model="query.LogLevel">
                    <option value="">Select a log level</option>
                    <option
                      v-for="(item, index) of logLevels"
                      :key="index"
                      v-bind:value="item.key"
                    >
                      {{ item.name }}
                    </option>
                  </select>
                </div>
              </div>
            </div>
            <div class="row">
              <div class="col-md-3">
                <div class="form-group">
                  <Datepicker
                    placeholder="Start time"
                    hideInputIcon
                    textInput
                    :format="previewDate"
                    :previewFormat="previewDate"
                    v-model="startTime"
                  ></Datepicker>
                </div>
              </div>
              <div class="col-md-3">
                <div class="form-group">
                  <Datepicker
                    placeholder="End time"
                    hideInputIcon
                    textInput
                    :format="previewDate"
                    :previewFormat="previewDate"
                    v-model="endTime"
                  ></Datepicker>
                </div>
              </div>
              <div class="col-md-3">
                <div class="form-group">
                  <select class="form-control" v-model="query.TableName">
                    <option
                      v-for="(item, index) of shards"
                      :key="index"
                      v-bind:value="item.TableName"
                    >
                      Shard {{ item.Number }}
                      {{ item.IsCurrent ? " (current)" : "" }}
                      {{ formatTime(item.CreatedDate) }}
                    </option>
                  </select>
                </div>
              </div>
            </div>
            <div class="row">
              <div class="col-md-12">
                <button type="submit" class="btn btn-primary pull-right">
                  Search
                </button>
                <button type="button" @click="toPaging" class="mr-3 btn btn-info pull-right">
                  To Paging 😁
                </button>
              </div>
            </div>
          </form>
          <DataTable
            :columns="columns"
            :entries="rows"
            :total="total"
            :pageSize="100"
            :tableClass="'table-compact'"
            @next="loadData"
            @prev="loadData"
            @moveTo="loadData"
            :rowClass="renderRowClass"
            :serverSide="true"
          ></DataTable>
        </div>
      </div>
    </div>
  </div>
  <LogDetailDialog
    :show="showModal"
    :logItem="currentItem"
    @close="
      showModal = false;
      currentItem = null;
    "
  />
</template>
<script>
import DataTable from "@components/shared/DataTable";
import LogDetailDialog from "./LogDetailDialog.vue";
import { ref, reactive } from "vue";
import ApplicationService from "@services/applications.service";
import ShardsService from "@services/shards.service";
import LogsQueryService from "@services/logs.service";
import LayoutService from "@services/layout.service";
import NotificationService from "@services/notification.service";
import Datepicker from "vue3-date-time-picker";
import moment from "moment";
export default {
  name: "LogsTable",
  components: { DataTable, Datepicker, LogDetailDialog },
  setup() {
    let rows = ref([]);
    let applications = ref([]);
    let shards = ref([]);
    let showModal = ref(false);
    let currentItem = ref(null);
    let total = ref(0);
    let startTime = ref(null);
    let endTime = ref(null);
    let query = reactive({
      PageIndex: 1,
      PageSize: 100,
      ApplicationId: "",
      Instance: "",
      Text: "",
      LogLevel: null,
      StartTime: null,
      EndTime: null,
      TableName: null,
    });
    getApplications();
    getShardsAndLoadData();

    function getApplications() {
      ApplicationService.getAll().then((response) => {
        applications.value = response.data.Data.Data;
      });
    }

    function getShardsAndLoadData() {
      ShardsService.getAll().then((response) => {
        shards.value = response.data.Data;
        query.TableName = shards.value[0].TableName;
        loadData();
      });
    }

    function previewDate(date) {
      if (!date) return "";
      return moment(date).format("DD/MM/YYYY HH:mm");
    }
    function loadData(pageIndex = 1) {
      query.PageIndex = pageIndex;
      if (startTime.value) query.StartTime = startTime.value.getTime() / 1000;
      else query.StartTime = null;
      if (endTime.value) query.EndTime = endTime.value.getTime() / 1000;
      else query.EndTime = null;

      LayoutService.loading();
      LogsQueryService.query(query).then((res) => {
        LayoutService.loaded();
        if (res.data.Success) {
          rows.value = res.data.Data.Data;
          total.value = res.data.Data.Total;
        } else {
          NotificationService.error(res.data.Errors);
        }
      });
    }
    return {
      rows,
      applications,
      shards,
      total,
      query,
      loadData,
      getApplications,
      getShardsAndLoadData,
      previewDate,
      startTime,
      endTime,
      showModal,
      currentItem,
    };
  },
  methods: {
    renderRowClass: function (item) {
      if (item.LogLevel == 4) return "bg-error";
      if (item.LogLevel == 3) return "bg-warning";
      return "";
    },
    showDetail: function (item) {
      this.showModal = true;
      this.currentItem = item;
    },
    formatTime(timestamp) {
      if (!timestamp) return "";
      const utcDate = new Date(timestamp * 1000);
      return moment(utcDate).format("DD/MM/YYYY HH:mm:ss");
    },
    toPaging() {
      window.scrollTo(0, document.body.scrollHeight);
    },
  },
  data() {
    const $this = this;
    return {
      columns: [
        {
          Label: "Timestamp",
          MinWidth: 150,
          Render: function (item) {
            const utcDate = new Date(item.Timestamp * 1000);
            return moment(utcDate).format("DD/MM/YYYY HH:mm:ss");
          },
          OnClick: function (item) {
            $this.showDetail(item);
          },
        },
        {
          Label: "Application",
          Render: function (item) {
            return $this.applications.find((f) => f.Id == item.ApplicationId)
              .ApplicationCode;
          },
        },
        {
          Label: "Message",
          Html: true,
          Value: "Message",
        },
      ],
      logLevels: [
        {
          key: 1,
          name: "DEBUG",
        },
        {
          key: 2,
          name: "INFO",
        },
        {
          key: 3,
          name: "WARNING",
        },
        {
          key: 4,
          name: "ERROR",
        },
        {
          key: 5,
          name: "OTHERS",
        },
      ],
    };
  },
};
</script>