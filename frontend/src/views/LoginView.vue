<template>
  <div class="page" style="display:flex; align-items:center; justify-content:center;">
    <div class="card" style="width:100%; max-width:420px;">
      <h2 style="margin-bottom:.3rem;">Welcome back 👋</h2>
      <p style="color:var(--text-muted); margin-bottom:1.5rem; font-size:.9rem;">Sign in to your account</p>

      <div v-if="error" class="alert alert-error">{{ error }}</div>

      <div class="form-group">
        <label>Email</label>
        <input class="form-control" type="email" v-model="email" placeholder="you@example.com" @keyup.enter="doLogin" />
      </div>
      <div class="form-group">
        <label>Password</label>
        <input class="form-control" type="password" v-model="password" placeholder="••••••••" @keyup.enter="doLogin" />
      </div>

      <button class="btn btn-primary" style="width:100%;" :disabled="loading" @click="doLogin">
        {{ loading ? 'Signing in...' : 'Sign In' }}
      </button>

      <p style="text-align:center; margin-top:1rem; font-size:.9rem; color:var(--text-muted);">
        Don't have an account? <router-link to="/register" style="color:var(--primary); font-weight:600;">Register</router-link>
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const auth = useAuthStore()
const router = useRouter()
const email = ref('')
const password = ref('')
const error = ref('')
const loading = ref(false)

async function doLogin() {
  error.value = ''
  loading.value = true
  try {
    await auth.login(email.value, password.value)
    window.showToast('Logged in successfully!')
    router.push('/')
  } catch (e) {
    error.value = e.response?.data?.message || 'Login failed. Check your credentials.'
  } finally {
    loading.value = false
  }
}
</script>
