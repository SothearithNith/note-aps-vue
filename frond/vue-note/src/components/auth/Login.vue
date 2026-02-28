<template>
  <div
    class="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-600 via-indigo-600 to-purple-600 font-sans px-4"
  >
    <!-- LOGIN CARD -->
    <div
      class="w-full max-w-full sm:max-w-md backdrop-blur-xl bg-white/90 p-6 sm:p-8 rounded-2xl shadow-2xl"
    >
      <div class="text-center mb-6 sm:mb-8">
        <h1 class="text-2xl sm:text-3xl font-bold text-gray-800">Welcome Back</h1>
        <p class="text-gray-500 mt-2 text-sm sm:text-base">Sign in to continue</p>
      </div>

      <form @submit.prevent="login" class="space-y-4 sm:space-y-6">
        <!-- Username -->
        <div class="relative">
          <input
            v-model="username"
            type="text"
            required
            class="peer w-full px-4 pt-6 pb-2 border rounded-lg focus:ring-2 focus:ring-indigo-500 outline-none"
            placeholder=" "
          />
          <label
            class="absolute left-4 top-2 text-gray-500 text-sm transition-all peer-placeholder-shown:top-4 peer-placeholder-shown:text-base peer-focus:top-2 peer-focus:text-sm"
          >
            Username
          </label>
        </div>

        <!-- Password -->
        <div class="relative">
          <input
            :type="showLoginPassword ? 'text' : 'password'"
            v-model="password"
            required
            class="peer w-full px-4 pt-6 pb-2 border rounded-lg focus:ring-2 focus:ring-indigo-500 outline-none"
            placeholder=" "
          />
          <label
            class="absolute left-4 top-2 text-gray-500 text-sm transition-all peer-placeholder-shown:top-4 peer-placeholder-shown:text-base peer-focus:top-2 peer-focus:text-sm"
          >
            Password
          </label>

          <button
            type="button"
            @click="showLoginPassword = !showLoginPassword"
            class="absolute right-4 top-4 text-sm text-indigo-600 hover:underline"
          >
            {{ showLoginPassword ? "Hide" : "Show" }}
          </button>
        </div>

        <button
          type="submit"
          :disabled="loading"
          class="w-full flex items-center justify-center gap-2 bg-indigo-600 text-white py-3 rounded-lg hover:bg-indigo-700 transition disabled:opacity-50"
        >
          {{ loading ? "Signing in..." : "Sign In" }}
        </button>
      </form>

      <p class="text-center text-sm text-gray-600 mt-4 sm:mt-6">
        Don’t have an account?
        <button
          @click="showSignup = true"
          class="text-indigo-600 font-medium hover:underline"
        >
          Sign up
        </button>
      </p>
    </div>
  </div>

  <!-- SIGNUP MODAL -->
  <transition name="fade">
    <div
      v-if="showSignup"
      class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm px-4"
    >
      <div
        class="w-full max-w-full sm:max-w-md bg-white rounded-2xl shadow-2xl p-6 sm:p-8 relative animate-scale-in"
      >
        <!-- Close X -->
        <button
          @click="closeSignup"
          class="absolute right-4 top-4 text-gray-400 hover:text-gray-600"
        >
          ✕
        </button>

        <h2 class="text-xl sm:text-2xl font-bold text-gray-800 text-center">
          Create Account
        </h2>
        <p class="text-center text-gray-500 mb-4 sm:mb-6 text-sm sm:text-base">
          Choose a username and password
        </p>

        <form @submit.prevent="signup" class="space-y-4 sm:space-y-5">
          <!-- Username -->
          <div class="relative">
            <input
              v-model="signupUsername"
              required
              class="peer w-full px-4 pt-6 pb-2 border rounded-lg focus:ring-2 focus:ring-indigo-500 outline-none"
              placeholder=" "
            />
            <label
              class="absolute left-4 top-2 text-gray-500 text-sm transition-all peer-placeholder-shown:top-4 peer-placeholder-shown:text-base peer-focus:top-2 peer-focus:text-sm"
            >
              Username
            </label>
          </div>

          <!-- Password -->
          <div class="relative">
            <input
              :type="showSignupPassword ? 'text' : 'password'"
              v-model="signupPassword"
              required
              class="peer w-full px-4 pt-6 pb-2 border rounded-lg focus:ring-2 focus:ring-indigo-500 outline-none"
              placeholder=" "
            />
            <label
              class="absolute left-4 top-2 text-gray-500 text-sm transition-all peer-placeholder-shown:top-4 peer-placeholder-shown:text-base peer-focus:top-2 peer-focus:text-sm"
            >
              Password
            </label>

            <button
              type="button"
              @click="showSignupPassword = !showSignupPassword"
              class="absolute right-4 top-4 text-sm text-indigo-600 hover:underline"
            >
              {{ showSignupPassword ? "Hide" : "Show" }}
            </button>
          </div>

          <button
            type="submit"
            :disabled="signupLoading"
            class="w-full bg-indigo-600 text-white py-3 rounded-lg hover:bg-indigo-700 transition disabled:opacity-50"
          >
            {{ signupLoading ? "Creating account..." : "Create Account" }}
          </button>
        </form>

        <!-- BACK TO LOGIN OPTION -->
        <div class="text-center mt-4 sm:mt-6 text-sm">
          <button
            @click="closeSignup"
            class="text-indigo-600 hover:underline font-medium"
          >
            ← Back to Login
          </button>
        </div>
      </div>
    </div>
  </transition>
</template>

<script setup lang="ts">
import { ref } from "vue";
import axios from "axios";
import Swal from "sweetalert2";

const username = ref("");
const password = ref("");
const showLoginPassword = ref(false);
const loading = ref(false);
const showSignup = ref(false);
const signupUsername = ref("");
const signupPassword = ref("");
const signupLoading = ref(false);
const showSignupPassword = ref(false);
const API_BASE = "http://localhost:5162/api/auth";

const closeSignup = () => {
  showSignup.value = false;
};

/* LOGIN */
import { useRouter } from "vue-router";

const router = useRouter();

const login = async () => {
  loading.value = true;

  try {
    const res = await axios.post(`${API_BASE}/login`, {
      username: username.value,
      passwordHash: password.value,
    });

    // Save token
    localStorage.setItem("jwtToken", res.data.token);
    localStorage.setItem("username", res.data.username);

    // Show success alert
    await Swal.fire({
      icon: "success",
      title: "Welcome back! 🎉",
      timer: 1500,
      showConfirmButton: false,
    });

    // Navigate to dashboard
    router.push("/dashboard"); // replace with your actual dashboard route
  } catch (err: any) {
    await Swal.fire({
      icon: "error",
      title: "Login Failed",
      text: err.response?.data?.message || "Something went wrong",
      confirmButtonColor: "#6366f1",
    });
  } finally {
    loading.value = false;
  }
};
/* SIGNUP */
const signup = async () => {
  if (signupUsername.value.length < 4) {
    await Swal.fire({
      icon: "error",
      title: "Username too short",
      text: "Minimum 4 characters required.",
      confirmButtonColor: "#6366f1",
    });
    return;
  }

  signupLoading.value = true;

  try {
    const res = await axios.post(`${API_BASE}/register`, {
      username: signupUsername.value,
      passwordHash: signupPassword.value,
    });

    await Swal.fire({
      icon: "success",
      title: "Account Created 🎉",
      text: "You can now login.",
      confirmButtonColor: "#6366f1",
    });

    showSignup.value = false;
  } catch (err: any) {
    await Swal.fire({
      icon: "error",
      title: "Signup Failed",
      text: err.response?.data?.message || "Something went wrong",
      confirmButtonColor: "#6366f1",
    });
  } finally {
    signupLoading.value = false;
  }
};
</script>
<style>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

@keyframes scaleIn {
  from {
    transform: scale(0.95);
    opacity: 0;
  }
  to {
    transform: scale(1);
    opacity: 1;
  }
}

.animate-scale-in {
  animation: scaleIn 0.2s ease-out;
}
</style>
