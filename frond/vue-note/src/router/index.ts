import { createRouter, createWebHistory } from "vue-router";
import Login from "../components/auth/Login.vue";  
import DashboardLayout from "../components/views/layouts/DashboardLayout.vue";
import Dashboard from "../components/views/Dashboard.vue";
import Notes from "../components/views/Notes.vue"; // new Notes page

const routes = [
  {
    path: "/",
    component: Login,
  },
  {
    path: "/dashboard",
    component: DashboardLayout,
    children: [
      {
        path: "",
        component: Dashboard,
      },
      {
        path: "users",
        component: {
          template: "<h1 class='text-2xl'>Users Page</h1>",
        },
      },
      {
        path: "notes",
        component: Notes, // added Notes route
      },
    ],
    meta: { requiresAuth: true },
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

// Navigation guard to check login
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem("jwtToken");

  if (to.meta.requiresAuth && !token) {
    // Not logged in, redirect to login
    next({ path: "/" });
  } else if (to.path === "/" && token) {
    // Already logged in, redirect to dashboard
    next({ path: "/dashboard" });
  } else {
    next();
  }
});

export default router;