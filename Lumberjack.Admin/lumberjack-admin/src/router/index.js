import { createWebHistory, createRouter } from "vue-router";
import ApplicationsTable from '@components/applications/Applications';
import ApplicationDetail from '@components/applications/ApplicationDetail';
import Dashboard from '@components/dashboards/Dashboard';
import LogsTable from '@components/log-viewer/LogsTable';
import LogStream from '@components/log-viewer/LogsStream';

const routes = [{
    path: "/",
    component: Dashboard,
    name: 'Dashboard'
}, {
    path: "/applications",
    component: ApplicationsTable,
    name: 'Applications'
}, {
    path: "/applications/create",
    component: ApplicationDetail,
    name: 'ApplicationCreate'
}, {
    path: "/applications/edit/:id",
    component: ApplicationDetail,
    name: 'ApplicationEdit'
}, {
    path: "/logs",
    component: LogsTable,
    name: 'Logs'
}, {
    path: "/stream",
    component: LogStream,
    name: 'Stream'
}];
const router = createRouter({
    history: createWebHistory(),
    routes
});
export default router;