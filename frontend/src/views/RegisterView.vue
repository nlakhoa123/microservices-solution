<template>
  <div class="page" style="display:flex; align-items:center; justify-content:center;">
    <div class="card" style="width:100%; max-width:420px;">
      <h2 style="margin-bottom:.3rem;">Create Account 🎉</h2>
      <p style="color:var(--text-muted); margin-bottom:1.5rem; font-size:.9rem;">Join MicroShop today</p>

      <div v-if="error" class="alert alert-error">{{ error }}</div>

      <div class="form-group">
        <label>Username</label>
        <input class="form-control" v-model="username" placeholder="johndoe" />
      </div>
      <div class="form-group">
        <label>Email</label>
        <input class="form-control" type="email" v-model="email" placeholder="you@example.com" />
      </div>
      <div class="form-group">
        <label>Password</label>
        <input class="form-control" type="password" v-model="password" placeholder="Min. 6 characters" />
      </div>

      <button class="btn btn-primary" style="width:100%;" :disabled="loading" @click="doRegister">
        {{ loading ? 'Creating...' : 'Create Account' }}
      </button>

      <p style="text-align:center; margin-top:1rem; font-size:.9rem; color:var(--text-muted);">
        Already have an account? <router-link to="/login" style="color:var(--primary); font-weight:600;">Sign In</router-link>
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
const username = ref('')
const email = ref('')
const password = ref('')
const error = ref('')
const loading = ref(false)

async function doRegister() {
  error.value = ''
  if (!username.value || !email.value || !password.value) {
    error.value = 'All fields are required.'
    return
  }
  loading.value = true
  try {
    await auth.register(username.value, email.value, password.value)
    window.showToast('Account created!')
    router.push('/')
  } catch (e) {
    error.value = e.response?.data?.message || 'Registration failed.'
  } finally {
    loading.value = false
  }
}
</script>
