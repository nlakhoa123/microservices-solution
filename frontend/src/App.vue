<template>
  <div>
    <nav class="navbar">
      <div class="container navbar-inner">
        <router-link to="/" class="navbar-brand"> MicroShop</router-link>
        <router-link to="/shop" class="nav-link" active-class="active">Shop</router-link>
        <router-link to="/url-shortener" class="nav-link" active-class="active">URL Shortener</router-link>
        <router-link to="/contact" class="nav-link" active-class="active">Contact</router-link>
        <router-link to="/cart" class="nav-link" active-class="active">
           Cart <span v-if="cart.count" class="badge badge-blue">{{ cart.count }}</span>
        </router-link>
        <template v-if="auth.isLoggedIn">
          <router-link to="/orders" class="nav-link" active-class="active">Orders</router-link>
          <router-link to="/profile" class="nav-link" active-class="active">👤 {{ auth.user?.username }}</router-link>
          <button class="btn btn-secondary btn-sm" @click="doLogout">Logout</button>
        </template>
        <template v-else>
          <router-link to="/login" class="nav-link" active-class="active">Login</router-link>
          <router-link to="/register" class="btn btn-primary btn-sm">Register</router-link>
        </template>
      </div>
    </nav>

    <router-view />

    <!-- Toast notifications -->
    <div class="toast-container">
      <div v-for="t in toasts" :key="t.id" :class="['toast', `toast-${t.type}`]">{{ t.msg }}</div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from './stores/auth'
import { useCartStore } from './stores/cart'

const auth = useAuthStore()
const cart = useCartStore()
const router = useRouter()
const toasts = ref([])

function doLogout() {
  auth.logout()
  router.push('/login')
}

// Global toast helper
window.showToast = (msg, type = 'success') => {
  const id = Date.now()
  toasts.value.push({ id, msg, type })
  setTimeout(() => { toasts.value = toasts.value.filter(t => t.id !== id) }, 3000)
}
</script>
