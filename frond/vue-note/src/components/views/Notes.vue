<template>
  <div class="p-6 max-w-5xl mx-auto">
    <!-- Header -->
    <h1 class="text-4xl font-bold mb-6 text-center text-gray-800">My Notes</h1>

    <!-- Add Note Button -->
    <div class="flex justify-end mb-6">
      <button
        @click="openModal()"
        class="bg-green-500 text-white px-5 py-2 rounded-lg hover:bg-green-600 shadow-md transition duration-300"
      >
        + Add Note
      </button>
    </div>

    <!-- Search and Sort -->
    <div class="flex flex-col md:flex-row mb-6 gap-3">
      <input
        v-model="search"
        type="text"
        placeholder="Search notes..."
        class="border border-gray-300 rounded-lg p-3 flex-1 focus:ring-2 focus:ring-green-400 focus:outline-none transition"
      />
      <select
        v-model="sortOrder"
        class="border border-gray-300 rounded-lg p-3 focus:ring-2 focus:ring-green-400 focus:outline-none transition"
      >
        <option value="desc">Newest First</option>
        <option value="asc">Oldest First</option>
      </select>
    </div>

    <!-- Notes Grid -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
      <div
        v-for="note in filteredNotes"
        :key="note.id"
        class="bg-white rounded-xl shadow-lg hover:shadow-2xl transition p-5 flex flex-col justify-between"
      >
        <div>
          <h2 class="font-semibold text-xl text-gray-800 mb-2">
            {{ note.title }}
          </h2>
          <p class="text-gray-600 mb-3 line-clamp-4">{{ note.content }}</p>
          <p class="text-gray-400 text-sm">
            Created: {{ formatDate(note.createdAt) }}
          </p>
        </div>
        <div class="flex gap-2 mt-4">
          <button
            @click="openModal(note)"
            class="flex-1 bg-blue-500 text-white py-2 rounded-lg hover:bg-blue-600 transition shadow-sm"
          >
            Edit
          </button>
          <button
            @click="deleteNote(note.id)"
            class="flex-1 bg-red-500 text-white py-2 rounded-lg hover:bg-red-600 transition shadow-sm"
          >
            Delete
          </button>
        </div>
      </div>
    </div>

    <p v-if="notes.length === 0" class="text-gray-400 mt-6 text-center">
      No notes yet.
    </p>

    <!-- Modal -->
    <transition name="fade">
      <div
        v-if="isModalOpen"
        class="fixed inset-0 z-50 flex items-center justify-center px-4"
        style="background: rgba(255, 255, 255, 0.2); backdrop-filter: blur(6px)"
      >
        <div
          class="bg-white rounded-2xl p-6 w-full max-w-md shadow-2xl transform transition-all duration-300 scale-95 opacity-0 animate-drop-in"
        >
          <h2 class="text-2xl font-bold mb-5 text-gray-800">
            {{ editingNote ? "Edit Note" : "Add Note" }}
          </h2>
          <input
            v-model="modalNote.title"
            type="text"
            placeholder="Title"
            class="border border-gray-300 p-3 rounded-lg w-full mb-4 focus:ring-2 focus:ring-green-400 focus:outline-none transition"
          />
          <textarea
            v-model="modalNote.content"
            placeholder="Content (optional)"
            class="border border-gray-300 p-3 rounded-lg w-full mb-5 h-32 resize-none focus:ring-2 focus:ring-green-400 focus:outline-none transition"
          ></textarea>
          <div class="flex justify-end gap-3">
            <button
              @click="closeModal"
              class="bg-gray-300 px-5 py-2 rounded-lg hover:bg-gray-400 transition"
            >
              Cancel
            </button>
            <button
              @click="saveNote"
              class="bg-green-500 text-white px-5 py-2 rounded-lg hover:bg-green-600 transition"
            >
              {{ editingNote ? "Update" : "Add" }}
            </button>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import axios from "axios";
import Swal from "sweetalert2";

interface Note {
  id: number;
  title: string;
  content?: string;
  createdAt: string;
  updatedAt?: string;
}

const notes = ref<Note[]>([]);
const search = ref("");
const sortOrder = ref("desc");

const isModalOpen = ref(false);
const editingNote = ref<Note | null>(null);
const modalNote = ref({ title: "", content: "" });

const token = localStorage.getItem("jwtToken") || "";
const api = axios.create({
  baseURL: "http://localhost:5162/api/notes",
  headers: { Authorization: `Bearer ${token}` },
});

const fetchNotes = async () => {
  try {
    const { data } = await api.get<Note[]>("");
    notes.value = data;
  } catch (error) {
    Swal.fire("Error", "Failed to fetch notes", "error");
  }
};

const openModal = (note?: Note) => {
  if (note) {
    editingNote.value = note;
    modalNote.value.title = note.title;
    modalNote.value.content = note.content || "";
  } else {
    editingNote.value = null;
    modalNote.value.title = "";
    modalNote.value.content = "";
  }
  isModalOpen.value = true;
};

const closeModal = () => (isModalOpen.value = false);

const saveNote = async () => {
  if (!modalNote.value.title.trim())
    return Swal.fire("Warning", "Title is required", "warning");

  try {
    if (editingNote.value) {
      await api.put(`/${editingNote.value.id}`, {
        ...editingNote.value,
        title: modalNote.value.title,
        content: modalNote.value.content,
      });
      editingNote.value.title = modalNote.value.title;
      editingNote.value.content = modalNote.value.content;
      Swal.fire("Success", "Note updated successfully", "success");
    } else {
      const { data } = await api.post("", modalNote.value);
      notes.value.push({
        id: data.id,
        title: modalNote.value.title,
        content: modalNote.value.content,
        createdAt: new Date().toISOString(),
      });
      Swal.fire("Success", "Note added successfully", "success");
    }
    closeModal();
  } catch (error) {
    Swal.fire("Error", "Failed to save note", "error");
  }
};

const deleteNote = async (id: number) => {
  const result = await Swal.fire({
    title: "Are you sure?",
    text: "You won't be able to revert this!",
    icon: "warning",
    showCancelButton: true,
    confirmButtonColor: "#d33",
    cancelButtonColor: "#3085d6",
    confirmButtonText: "Yes, delete it!",
  });

  if (result.isConfirmed) {
    try {
      await api.delete(`/${id}`);
      notes.value = notes.value.filter((n) => n.id !== id);
      Swal.fire("Deleted!", "Note has been deleted.", "success");
    } catch (error) {
      Swal.fire("Error", "Failed to delete note", "error");
    }
  }
};

const filteredNotes = computed(() => {
  let filtered = notes.value.filter((n) =>
    n.title.toLowerCase().includes(search.value.toLowerCase()),
  );
  filtered.sort((a, b) =>
    sortOrder.value === "desc"
      ? new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
      : new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime(),
  );
  return filtered;
});

const formatDate = (date: string) => new Date(date).toLocaleString();

onMounted(fetchNotes);
</script>

<style scoped>
@keyframes dropIn {
  0% {
    transform: translateY(-50px) scale(0.95);
    opacity: 0;
  }
  100% {
    transform: translateY(0) scale(1);
    opacity: 1;
  }
}
.animate-drop-in {
  animation: dropIn 0.3s ease-out forwards;
}
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
