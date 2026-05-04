<template>
  <div class="page">
    <div class="container">
      <!-- Hero -->
      <div class="hero card" style="text-align:center; padding: 4rem 2rem; margin-bottom: 2rem; background: linear-gradient(135deg, #6366f1, #8b5cf6);">
        <h1 style="font-size: 2.5rem; color: #fff; margin-bottom: 1rem;">Welcome to MicroShop 🚀</h1>
        <p style="color: #e0e7ff; font-size: 1.1rem; margin-bottom: 2rem;">A full microservices demo: Shop, URL Shortener, and Contact — all in one place.</p>
        <div style="display:flex; gap:1rem; justify-content:center; flex-wrap:wrap;">
          <router-link to="/shop" class="btn btn-primary" style="background:#fff; color:#6366f1;">Browse Shop</router-link>
          <router-link to="/url-shortener" class="btn" style="background:rgba(255,255,255,.2); color:#fff;">Try URL Shortener</router-link>
        </div>
      </div>

      <!-- Features -->
      <div class="grid-3">
        <div class="card" style="text-align:center;">
          <div style="font-size: 2.5rem; margin-bottom: 1rem;">🛒</div>
          <h3 style="margin-bottom:.5rem;">Online Shop</h3>
          <p style="color: var(--text-muted); font-size:.9rem;">Browse products, apply discount codes, and checkout instantly via our Goods & Payment service.</p>
          <router-link to="/shop" class="btn btn-primary btn-sm" style="margin-top:1rem;">Go to Shop</router-link>
        </div>
        <div class="card" style="text-align:center;">
          <div style="font-size: 2.5rem; margin-bottom: 1rem;">🔗</div>
          <h3 style="margin-bottom:.5rem;">URL Shortener</h3>
          <p style="color: var(--text-muted); font-size:.9rem;">Shorten any URL instantly. Redis-cached for blazing-fast redirects. Track click analytics.</p>
          <router-link to="/url-shortener" class="btn btn-primary btn-sm" style="margin-top:1rem;">Shorten URL</router-link>
        </div>
        <div class="card" style="text-align:center;">
          <div style="font-size: 2.5rem; margin-bottom: 1rem;">💬</div>
          <h3 style="margin-bottom:.5rem;">Contact Us</h3>
          <p style="color: var(--text-muted); font-size:.9rem;">Send a message via our RabbitMQ message broker. Messages are queued and processed reliably.</p>
          <router-link to="/contact" class="btn btn-primary btn-sm" style="margin-top:1rem;">Send Message</router-link>
        </div>
      </div>

      <!-- Architecture note -->
      <div class="card" style="margin-top:2rem; background:#f8fafc;">
        <h3 style="margin-bottom:1rem;">🏗️ Architecture</h3>
        <div class="grid-4" style="gap:1rem;">
          <div v-for="svc in services" :key="svc.name" style="display:flex; align-items:center; gap:.5rem; padding:.7rem; background:#fff; border-radius:8px; border:1px solid var(--border);">
            <span style="font-size:1.3rem;">{{ svc.icon }}</span>
            <div>
              <div style="font-weight:700; font-size:.85rem;">{{ svc.name }}</div>
              <div style="color:var(--text-muted); font-size:.75rem;">{{ svc.port }}</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
const services = [
  { icon: '🚪', name: 'API Gateway', port: 'Port 5000 (Ocelot)' },
  { icon: '👤', name: 'User Service', port: 'Port 5001' },
  { icon: '🔗', name: 'URL Service', port: 'Port 5002' },
  { icon: '🛒', name: 'Goods Service', port: 'Port 5003' },
  { icon: '💬', name: 'Message Broker', port: 'Port 5004' },
  { icon: '⚡', name: 'Redis Cache', port: 'Port 6379' },
  { icon: '🐰', name: 'RabbitMQ', port: 'Port 5672' },
  { icon: '🖥️', name: 'Vue Frontend', port: 'Port 3000' },
]
</script>
