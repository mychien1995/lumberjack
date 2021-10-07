<template>
  <div class="row">
    <div class="col-md-12">
      <div class="card">
        <div class="card-header">
          <h4 class="card-title">
            {{ isEdit ? "Edit Application" : "Create Application" }}
          </h4>
        </div>
        <div class="card-body">
          <div v-for="(item, index) in errors" :key="index" class="text-danger">
            {{ item }}
          </div>
          <form v-on:submit="submit">
            <div class="row">
              <div class="col-md-6">
                <div class="form-group">
                  <label>Application Name</label>
                  <input
                    type="text"
                    class="form-control"
                    placeholder="Application Name"
                    v-model="application.ApplicationName"
                  />
                </div>
              </div>
              <div class="col-md-6">
                <div class="form-group">
                  <label>Application Unique Code</label>
                  <input
                    type="text"
                    class="form-control"
                    placeholder="Application Code"
                    v-model="application.ApplicationCode"
                  />
                </div>
              </div>
            </div>
            <div class="row">
              <div class="col-md-6">
                <div class="form-group">
                  <label>Application Instances</label>
                  <input
                    v-for="(item, index) in application.Instances"
                    v-on:blur="addOrRemoveInstance(index)"
                    :key="index"
                    type="text"
                    class="form-control"
                    placeholder="Application Instances"
                    v-model="item.InstanceName"
                    v-bind:class="{ 'mt-2': index > 0 }"
                  />
                </div>
              </div>
              <div class="col-md-6">
                <div class="form-group">
                  <label>API Keys</label>
                  <input
                    v-for="(item, index) in application.ApiKeys"
                    v-on:blur="addOrRemoveApiKey(index)"
                    :key="index"
                    type="text"
                    class="form-control"
                    placeholder="Key Value"
                    v-model="item.KeyValue"
                    v-bind:class="{ 'mt-2': index > 0 }"
                  />
                </div>
              </div>
            </div>
            <div class="row">
              <div class="col-md-6">
                <div class="form-group">
                  <label>Sort Order</label>
                  <input
                    type="number"
                    class="form-control"
                    placeholder="Sort Order"
                    v-model="application.SortOrder"
                  />
                </div>
              </div>
            </div>
            <div class="row">
              <div class="col-md-12">
                <router-link
                  :to="'/applications'"
                  class="btn btn-danger pull-right"
                  :tag="'button'"
                  >Cancel</router-link
                >
                <button type="submit" class="btn btn-primary pull-right">
                  Submit
                </button>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
import { useRoute } from "vue-router";
import { ref } from "vue";
import ApplicationService from "@services/applications.service";
import NotificationService from "@services/notification.service";
import LayoutService from "@services/layout.service";
export default {
  name: "ApplicationDetail",
  setup() {
    const route = useRoute();
    const id = route.params.id;
    let isEdit = ref(false);
    let errors = ref([]);
    let application = ref({
      Instances: [
        {
          InstanceName: "",
        },
      ],
      ApiKeys: [
        {
          KeyValue: "",
        },
      ],
      ApplicationName: "",
      ApplicationCode: "",
      SortOrder: "",
    });
    if (id) {
      isEdit.value = true;
      LayoutService.loading();
      ApplicationService.getById(id).then((res) => {
        application.value = res.data.Data;
        if (application.value.Instances.length == 0)
          application.value.Instances.push({ InstanceName: "" });
        if (application.value.ApiKeys.length == 0)
          application.value.ApiKeys.push({ KeyValue: "" });
        LayoutService.loaded();
      });
    }
    return {
      isEdit,
      errors,
      application,
    };
  },
  methods: {
    addOrRemoveInstance(index) {
      this.addOrRemoveValue(this.application.Instances, "InstanceName", index);
    },
    addOrRemoveApiKey(index) {
      this.addOrRemoveValue(this.application.ApiKeys, "KeyValue", index);
    },
    addOrRemoveValue(array, propertyName, index) {
      const entry = array[index];
      let tmp = {};
      tmp[propertyName] = "";
      if (index == array.length - 1 && entry[propertyName]) array.push(tmp);
      if (!entry[propertyName] && index != array.length - 1)
        array.splice(index, 1);
    },
    submit(e) {
      e.preventDefault();
      const errors = [];
      if (!this.application.ApplicationName)
        errors.push("Application Name is required");
      if (!this.application.ApplicationCode)
        errors.push("Application Code is required");
      if (this.application.Instances.filter((i) => i.InstanceName).length == 0)
        errors.push("Application Instances is required");
      if (this.application.ApiKeys.filter((i) => i.KeyValue).length == 0)
        errors.push("API Keys is required");
      this.errors = errors;
      if (errors.length > 0) return;
      const submitData = { ...this.application };
      submitData.Instances = submitData.Instances.filter((i) => i.InstanceName);
      submitData.ApiKeys = submitData.ApiKeys.filter((i) => i.KeyValue);
      LayoutService.loading();
      ApplicationService.persist(submitData)
        .then((response) => {
          LayoutService.loaded();
          if (response.data.Success) {
            NotificationService.success("Application Persisted");
            this.$router.push({ path: "/applications" });
          } else NotificationService.error(response.data.Errors);
        })
        .catch((ex) => {
          LayoutService.loaded();
          NotificationService.error(ex);
        });
    },
  },
};
</script>