import ApiService from "@services/api.service";

const LogsQueryService = {
    query(data) {
        return ApiService.post("api/admin/logs", data);
    }
}

export default LogsQueryService;