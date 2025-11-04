import { createRouter, createWebHistory } from "vue-router";
import Login from "../views/Login.vue";
import Register from "../views/Register.vue";
import Dashboard from "../views/Dashboard.vue";
import { useAuthStore } from "../store/authStore";

const routes = [
    { path: "/", redirect: "/login" },

    {
        path: "/login",
        component: Login,
        meta: { guestOnly: true }
    },

    {
        path: "/register",
        component: Register,
        meta: { guestOnly: true }
    },

    {
        path: "/dashboard",
        component: Dashboard,
        meta: { requiresAuth: true }
    },
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

// ✅ Route Guard
router.beforeEach((to, from, next) => {
    const auth = useAuthStore();
    const isLoggedIn = !!auth.token;

    // ✅ Nếu chưa login mà vào trang cần login → chuyển về /login
    if (to.meta.requiresAuth && !isLoggedIn) {
        return next("/login");
    }

    // ✅ Nếu đã login mà vào login/register → chuyển về /dashboard
    if (to.meta.guestOnly && isLoggedIn) {
        return next("/dashboard");
    }

    next();
});

export default router;
