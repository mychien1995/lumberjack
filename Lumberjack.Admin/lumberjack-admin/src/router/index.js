import { createWebHistory, createRouter } from "vue-router";
import ApplicationsTable from '@components/applications/Applications';
import ApplicationDetail from '@components/applications/ApplicationDetail';
import Dashboard from '@components/dashboards/Dashboard';
import LogsTable from '@components/logs/LogsTable';

const routes = [{
    path: "/",
    component: Dashboard
}, {
    path: "/applications",
    component: ApplicationsTable
}, {
    path: "/applications/create",
    component: ApplicationDetail
}, {
    path: "/applications/edit/:id",
    component: ApplicationDetail
}, {
    path: "/logs",
    component: LogsTable
}];
const router = createRouter({
    history: createWebHistory(),
    routes
});
export default router;