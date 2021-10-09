import ApiService from "@services/api.service";

const ShardsService = {
  getAll() {
    return ApiService.get("api/admin/shards");
  },
};

export default ShardsService;
