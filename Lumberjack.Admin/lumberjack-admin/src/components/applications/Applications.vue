<template>
  <div class="row">
    <div class="col-md-12">
      <div class="card">
        <div class="card-header">
          <h4 class="card-title">Applications</h4>
        </div>
        <div class="card-body">
          <router-link
            :to="'/applications/create'"
            class="btn btn-primary pull-right"
            :tag="button"
            >Create</router-link
          >
          <DataTable
            :columns="columns"
            :entries="rows"
            :total="rows.length"
          ></DataTable>
          <ConfirmModal
            :title="'Confirm Delete'"
            :content="'Are you sure you want to delete this application'"
            :show="showDelete"
            @close="showDelete = false"
            @confirm="confirmDelete()"
          ></ConfirmModal>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
import DataTable from "@components/shared/DataTable";
import { ref } from "vue";
import ApplicationService from "@services/applications.service";
import LayoutService from "@services/layout.service";
import NotificationService from "@services/notification.service";
import ConfirmModal from "@components/shared/ConfirmModal.vue";
export default {
  name: "ApplicationsTable",
  components: { DataTable, ConfirmModal },
  setup() {
    let rows = ref([]);
    let showDelete = ref(false);
    let selectedId = ref("");
    loadData();

    function confirmDelete() {
      LayoutService.loading();
      ApplicationService.delete(this.selectedId).then(() => {
        NotificationService.success("Application Deleted");
        this.showDelete = false;
        loadData();
        LayoutService.loaded();
      });
    }

    function loadData() {
      LayoutService.loading();
      ApplicationService.getAll().then((response) => {
        rows.value = response.data.Data.Data;
        LayoutService.loaded();
      });
    }
    return { rows, showDelete, selectedId, confirmDelete, loadData };
  },
  data() {
    let $this = this;
    return {
      columns: [
        { Label: "Name", Value: "ApplicationName" },
        { Label: "Code", Value: "ApplicationCode" },
        { Label: "Active", Value: "IsActive" },
        {
          Label: "Action",
          Action: function (item) {
            return [
              {
                icon: "fa fa-2x fa-edit",
                link: "/applications/edit/" + item.Id,
                title: "Edit",
              },
              {
                icon: "fa fa-2x fa-trash",
                confirm: true,
                modalTitle: "Are you sure you want to delete this application",
                callback: function (item) {
                  $this.showDelete = true;
                  $this.selectedId = item.Id;
                },
                title: "Delete",
              },
            ];
          },
        },
      ],
    };
  },
};
</script>