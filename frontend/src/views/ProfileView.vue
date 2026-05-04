<template>
  <div class="page">
    <div class="container" style="max-width:540px;">
      <h1 class="section-title">👤 My Profile</h1>

      <div class="card">
        <div v-if="saved" class="alert alert-success">Profile updated!</div>
        <div v-if="error" class="alert alert-error">{{ error }}</div>

        <div style="display:flex; align-items:center; gap:1rem; margin-bottom:1.5rem;">
          <div style="width:60px; height:60px; border-radius:50%; background:var(--primary); display:flex; align-items:center; justify-content:center; color:#fff; font-size:1.5rem; font-weight:700;">
            {{ auth.user?.username?.[0]?.toUpperCase() }}
          </div>
          <div>
            <div style="font-weight:800; font-size:1.1rem;">{{ auth.user?.username }}</div>
            <div class="badge badge-blue">{{ auth.user?.role }}</div>
          </div>
        </div>

        <div class="form-group">
          <label>Username</label>
          <input class="form-control" v-model="form.username" />
        </div>
        <div class="form-group">
          <label>Email</label>
          <input class="form-control" type="email" v-model="form.email" />
        </div>
        <div class="form-group">
          <label>Customer ID</label>
          <input class="form-control" :value="auth.user?.id" disabled style="background:#f8fafc;" />
          <small style="color:var(--text-muted); font-size:.8rem;">This ID is shared across Goods, Discount, and Payment services.</small>
        </div>

        <button class="btn btn-primary" @click="save" :disabled="saving">
          {{ saving ? 'Saving...' : 'Save Changes' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import axios from 'axios'
import { useAuthStore } from '../stores/auth'

const auth = useAuthStore()
const form = ref({ username: auth.user?.username || '', email: auth.user?.email || '' })
const saving = ref(false)
const saved = ref(false)
const error = ref('')

async function save() {
  saving.value = true
  saved.value = false
  error.value = ''
  try {
    const { data } = await axios.put('/api/Auth/profile', form.value)
    auth.user.username = data.username
    auth.user.email = data.email
    localStorage.setItem('user', JSON.stringify(auth.user))
    saved.value = true
    window.showToast('Profile saved!')
  } catch (e) {
    error.value = e.response?.data?.message || 'Failed to update profile.'
  } finally {
    saving.value = false
  }
}
</script>
