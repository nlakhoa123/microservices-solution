<template>
  <div class="page">
    <div class="container" style="max-width:640px;">
      <h1 class="section-title">Contact Us</h1>
      <p class="section-subtitle">Messages are queued via RabbitMQ and processed reliably</p>

      <div class="card" style="margin-bottom:1.5rem;">
        <div v-if="success" class="alert alert-success">Message sent! Status: <strong>{{ success }}</strong>. We'll get back to you soon.
        </div>
        <div v-if="error" class="alert alert-error">{{ error }}</div>

        <div class="grid-2">
          <div class="form-group">
            <label>Your Name</label>
            <input class="form-control" v-model="form.name" placeholder="John Doe" />
          </div>
          <div class="form-group">
            <label>Email</label>
            <input class="form-control" type="email" v-model="form.email" placeholder="you@example.com" />
          </div>
        </div>
        <div class="form-group">
          <label>Subject</label>
          <input class="form-control" v-model="form.subject" placeholder="How can we help?" />
        </div>
        <div class="form-group">
          <label>Message</label>
          <textarea class="form-control" v-model="form.body" rows="5" placeholder="Write your message here..."></textarea>
        </div>

        <button class="btn btn-primary" style="width:100%;" :disabled="loading" @click="send">
          {{ loading ? 'Sending...' : 'Send Message' }}
        </button>
      </div>

      <!-- Info box -->
      <div class="card" style="background:#f8fafc; display:flex; gap:1rem; align-items:flex-start;">
        <div>
          <div style="font-weight:700; margin-bottom:.3rem;">Powered by RabbitMQ</div>
          <p style="font-size:.85rem; color:var(--text-muted);">Your message is published to a RabbitMQ queue and consumed by a background worker. This guarantees reliable delivery even during high load.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import axios from 'axios'
import { useAuthStore } from '../stores/auth'

const auth = useAuthStore()
const loading = ref(false)
const success = ref('')
const error = ref('')

const form = ref({ name: auth.user?.username || '', email: auth.user?.email || '', subject: '', body: '' })

async function send() {
  error.value = ''
  success.value = ''
  if (!form.value.name || !form.value.email || !form.value.body) {
    error.value = 'Name, email, and message are required.'
    return
  }
  loading.value = true
  try {
    const { data } = await axios.post('/api/contact/send', {
      name: form.value.name,
      email: form.value.email,
      subject: form.value.subject,
      body: form.value.body,
      customerId: auth.user?.id || null
    })
    success.value = data.status
    form.value = { name: auth.user?.username || '', email: auth.user?.email || '', subject: '', body: '' }
    window.showToast('Message sent via RabbitMQ!')
  } catch (e) {
    error.value = e.response?.data?.message || 'Failed to send message.'
  } finally {
    loading.value = false
  }
}
</script>
