<script setup lang="ts">
import { computed } from "vue";
import { useRoute, useRouter } from "vue-router";

const props = defineProps<{
  collapsed: boolean;
  isMobile: boolean;
}>();

const emit = defineEmits<{ (e: 'toggleSidebar'): void }>();

const route = useRoute();
const router = useRouter();

const navItems = [
  { name: "Dashboard", path: "/dashboard", icon: "📊" },
  { name: "Notes", path: "/dashboard/notes", icon: "📝" },
];

const sidebarClasses = computed(() => {
  // Mobile: always translate-x-0 if open
  if (props.isMobile) return [
    "fixed top-0 left-0 h-screen bg-indigo-700 text-white flex flex-col transition-all duration-300 z-50",
    props.collapsed ? "-translate-x-full" : "translate-x-0 w-64",
  ];

  // Desktop
  return [
    "fixed top-0 left-0 h-screen bg-indigo-700 text-white flex flex-col transition-all duration-300 z-50",
    props.collapsed ? "-translate-x-full sm:translate-x-0 w-16" : "translate-x-0 w-64",
  ];
});

const isActive = (path: string) => route.path === path;

const navigate = (path: string) => {
  router.push(path);
  if (props.isMobile) emit('toggleSidebar'); // close overlay on mobile
};
</script>

<template>
  <!-- Mobile overlay -->
  <div
    v-if="!props.collapsed && props.isMobile"
    class="fixed inset-0 bg-black bg-opacity-40 z-40"
    @click="emit('toggleSidebar')"
  ></div>

  <aside :class="sidebarClasses">
    <!-- Logo -->
    <div class="flex items-center justify-between p-4 border-b border-indigo-600">
      <div class="flex items-center gap-2">
        <span class="text-2xl">⚡</span>
        <span v-if="!props.collapsed" class="text-lg font-bold">Vue Note</span>
      </div>

      <!-- Desktop toggle -->
      <button
        @click="emit('toggleSidebar')"
        class="hidden sm:inline-flex p-1 rounded hover:bg-indigo-600 transition"
      >
        <svg
          :class="[props.collapsed ? 'rotate-180' : '', 'w-5 h-5']"
          fill="none"
          stroke="currentColor"
          stroke-width="2"
          viewBox="0 0 24 24"
          stroke-linecap="round"
          stroke-linejoin="round"
        >
          <path d="M15 19l-7-7 7-7" />
        </svg>
      </button>
    </div>

    <!-- Navigation -->
    <nav class="flex-1 mt-6 flex flex-col gap-2">
      <div
        v-for="item in navItems"
        :key="item.name"
        @click="navigate(item.path)"
        class="group flex items-center px-4 py-2 rounded hover:bg-indigo-600 transition relative cursor-pointer"
        :class="{ 'bg-indigo-600': isActive(item.path) }"
      >
        <span class="text-lg">{{ item.icon }}</span>
        <span v-if="!props.collapsed" class="ml-3">{{ item.name }}</span>

        <!-- Tooltip when collapsed -->
        <span
          v-if="props.collapsed && !props.isMobile"
          class="absolute left-16 bg-black text-white text-xs rounded py-1 px-2 opacity-0 group-hover:opacity-100 transition-opacity whitespace-nowrap"
        >
          {{ item.name }}
        </span>
      </div>
    </nav>
  </aside>
</template>