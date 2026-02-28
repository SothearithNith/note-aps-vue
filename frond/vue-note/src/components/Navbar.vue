<template>
  <header
    :class="[
      'h-16 bg-white shadow-md flex items-center justify-between pr-4 sm:pr-6 sticky top-0 z-40 transition-all duration-300',
      headerPadding
    ]"
  >
    <div class="flex items-center gap-4">
      <!-- Mobile hamburger -->
      <button
        @click="$emit('toggleSidebar')"
        class="sm:hidden p-2 text-gray-600 hover:text-indigo-600 rounded"
      >
        <svg class="w-6 h-6" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
          <path d="M4 6h16M4 12h16M4 18h16" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </button>

      <h1 class="font-semibold text-lg">
        Welcome back, <span class="text-indigo-600">{{ username || "Guest" }}</span>!
      </h1>
    </div>

    <button
      @click="logout"
      class="bg-red-500 text-white px-3 py-1 rounded-lg text-sm hover:bg-red-600 transition"
    >
      Logout
    </button>
  </header>
</template>

<script setup lang="ts">
import { defineProps, computed } from "vue";
import { useRouter } from "vue-router";
import Swal from "sweetalert2";

const props = defineProps<{
  username?: string;
  collapsed: boolean;
  isMobile: boolean;
}>();

const router = useRouter();

const logout = async () => {
  const result = await Swal.fire({
    title: "Logout?",
    icon: "warning",
    showCancelButton: true,
    confirmButtonColor: "#ef4444",
    cancelButtonColor: "#6b7280",
    confirmButtonText: "Yes, logout",
  });

  if (result.isConfirmed) {
    localStorage.removeItem("jwtToken");
    localStorage.removeItem("username");
    router.push("/");

    Swal.fire({
      icon: "success",
      title: "Logged out",
      timer: 1200,
      showConfirmButton: false,
    });
  }
};

const headerPadding = computed(() => {
  if (props.isMobile) return "pl-4 sm:pl-6"; // mobile: no shift
  return props.collapsed ? "pl-20" : "pl-72"; // desktop: shift according to sidebar
});
</script>