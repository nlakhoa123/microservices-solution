<template>
  <div class="page">
    <div class="container" style="max-width:720px;">
      <h1 class="section-title">🔗 URL Shortener</h1>
      <p class="section-subtitle">Powered by Redis cache for instant redirects</p>

      <!-- Shorten form -->
      <div class="card" style="margin-bottom:1.5rem;">
        <div class="form-group" style="margin-bottom:.8rem;">
          <label>Long URL</label>
          <input class="form-control" v-model="longUrl" placeholder="https://example.com/very/long/url/here"
            @keyup.enter="shorten" style="font-size:1rem;" />
        </div>
        <div style="display:flex; gap:1rem; align-items:end;">
          <div class="form-group" style="margin-bottom:0; flex:1;">
            <label>Expires in (days, optional)</label>
            <input class="form-control" type="number" v-model="expiresIn" placeholder="Leave blank for no expiry" min="1" />
          </div>
          <button class="btn btn-primary" @click="shorten" :disabled="loading || !longUrl">
            {{ loading ? 'Shortening...' : '⚡ Shorten' }}
          </button>
        </div>

        <!-- Result -->
        <div v-if="result" class="alert alert-success" style="margin-top:1rem; display:flex; align-items:center; justify-content:space-between; flex-wrap:wrap; gap:.5rem;">
          <div>
            <div style="font-size:.8rem; font-weight:600; margin-bottom:.2rem;">Short URL:</div>
            <a :href="result.shortUrl" target="_blank" style="font-weight:800; color:#065f46;">{{ result.shortUrl }}</a>
          </div>
          <button class="btn btn-success btn-sm" @click="copy(result.shortUrl)">📋 Copy</button>
        </div>

        <div v-if="error" class="alert alert-error" style="margin-top:1rem;">{{ error }}</div>
      </div>

      <!-- My URLs (if logged in) -->
      <div v-if="auth.isLoggedIn" class="card">
        <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom:1rem;">
          <h3>My Shortened URLs</h3>
          <button class="btn btn-secondary btn-sm" @click="loadMyUrls">↻ Refresh</button>
        </div>

        <div v-if="myUrls.length === 0" style="text-align:center; padding:1.5rem; color:var(--text-muted);">
          No URLs yet. Shorten something!
        </div>
        <table v-else class="table">
          <thead>
            <tr><th>Short URL</th><th>Original</th><th>Clicks</th><th>Created</th><th></th></tr>
          </thead>
          <tbody>
            <tr v-for="u in myUrls" :key="u.id">
              <td>
                <a :href="u.shortUrl" target="_blank" style="color:var(--primary); font-weight:600; font-size:.85rem;">
                  /r/{{ u.shortCode }}
                </a>
              </td>
              <td style="max-width:200px; overflow:hidden; text-overflow:ellipsis; white-space:nowrap; font-size:.8rem; color:var(--text-muted);">
                {{ u.originalUrl }}
              </td>
              <td><span class="badge badge-blue">{{ u.clickCount }}</span></td>
              <td style="font-size:.8rem; color:var(--text-muted);">{{ new Date(u.createdAt).toLocaleDateString() }}</td>
              <td>
                <button class="btn btn-danger btn-sm" @click="deleteUrl(u.shortCode)">✕</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <div v-else class="card" style="text-align:center; padding:2rem; background:#f8fafc;">
        <p style="color:var(--text-muted);">
          <router-link to="/login" style="color:var(--primary); font-weight:600;">Login</router-link> to save and manage your shortened URLs.
        </p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import { useAuthStore } from '../stores/auth'

const auth = useAuthStore()
const longUrl = ref('')
const expiresIn = ref('')
const loading = ref(false)
const result = ref(null)
const error = ref('')
const myUrls = ref([])

async function shorten() {
  if (!longUrl.value) return
  loading.value = true
  error.value = ''
  result.value = null
  try {
    const { data } = await axios.post('/api/url/shorten', {
      originalUrl: longUrl.value,
      expiresInDays: expiresIn.value ? parseInt(expiresIn.value) : null
    })
    result.value = data
    longUrl.value = ''
    expiresIn.value = ''
    if (auth.isLoggedIn) loadMyUrls()
    window.showToast('URL shortened!')
  } catch (e) {
    error.value = e.response?.data?.message || 'Failed to shorten URL.'
  } finally {
    loading.value = false
  }
}

async function loadMyUrls() {
  if (!auth.isLoggedIn) return
  try {
    const { data } = await axios.get('/api/url/my-urls')
    myUrls.value = data
  } catch { /* empty */ }
}

async function deleteUrl(code) {
  try {
    await axios.delete(`/api/url/${code}`)
    window.showToast('URL deleted.', 'error')
    loadMyUrls()
  } catch { /* empty */ }
}

function copy(text) {
  navigator.clipboard.writeText(text)
  window.showToast('Copied to clipboard!')
}

onMounted(() => { if (auth.isLoggedIn) loadMyUrls() })
</script>
