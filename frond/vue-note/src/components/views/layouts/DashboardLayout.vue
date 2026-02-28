<template>
  <div class="flex h-screen overflow-hidden bg-gray-100">
    <!-- Sidebar -->
    <Sidebar
      :collapsed="sidebarCollapsed"
      :is-mobile="isMobile"
      @toggleSidebar="toggleSidebar"
    />

    <!-- Main content -->
    <div class="flex-1 flex flex-col overflow-auto">
      <!-- Navbar -->
      <Navbar
        @toggleSidebar="toggleSidebar"
        :username="username"
        :collapsed="sidebarCollapsed"
        :is-mobile="isMobile"
      />

      <!-- Main content -->
      <main class="p-4 sm:p-6 flex-1 min-h-[calc(100vh-4rem)]">
        <router-view />
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount, watch } from "vue";
import Navbar from "../../Navbar.vue";
import Sidebar from "../../Sidebar.vue";

const sidebarCollapsed = ref(false); // actual sidebar state
const isMobile = ref(false);
const username = ref("User");

// Toggle sidebar open/close
const toggleSidebar = () => {
  sidebarCollapsed.value = !sidebarCollapsed.value;
};

// Detect small screens and auto-collapse sidebar
const checkScreen = () => {
  const mobile = window.innerWidth < 640;
  isMobile.value = mobile;

  // Only force collapse on mobile if opening page
  if (mobile) {
    sidebarCollapsed.value = true; // hide sidebar initially
  } else {
    sidebarCollapsed.value = false; // show sidebar on desktop
  }
};

onMounted(() => {
  // Load username from localStorage
  const storedUser = localStorage.getItem("username");
  username.value = storedUser || "User";

  checkScreen();
  window.addEventListener("resize", checkScreen);
});

onBeforeUnmount(() => {
  window.removeEventListener("resize", checkScreen);
});
</script>