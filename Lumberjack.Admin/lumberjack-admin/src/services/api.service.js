import axios from "axios";
const ApiService = {
    adminUrl: '',
    header: {
        headers: {
            'Content-Type': 'application/json',
        }
    },
    init() {
        this.adminUrl = window.globalApiUrl;
    },
    get(resource) {
        return axios.get(`${this.adminUrl}/${resource}`);
    },

    post(resource, params) {
        return axios.post(`${this.adminUrl}/${resource}`, params, this.header);
    },

    put(resource, params) {
        return axios.put(`${this.adminUrl}/${resource}`, params, this.header);
    },

    delete(resource) {
        return axios.delete(`${this.adminUrl}/${resource}`);
    }
}

export default ApiService;