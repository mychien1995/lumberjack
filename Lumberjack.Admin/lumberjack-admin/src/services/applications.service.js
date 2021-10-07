import ApiService from "@services/api.service";

const ApplicationService = {
    getAll() {
        return ApiService.get("api/admin/applications");
    },

    persist(applicationData) {
        return ApiService.post('api/admin/applications', applicationData);
    },

    getById(id) {
        return ApiService.get(`api/admin/applications/${id}`);
    },

    delete(id) {
        return ApiService.delete(`api/admin/applications/${id}`);
    }
};

export default ApplicationService;