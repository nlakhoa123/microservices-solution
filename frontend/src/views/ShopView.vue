<template>
  <div class="page">
    <div class="container">
      <h1 class="section-title">Shop</h1>
      <p class="section-subtitle">Browse our products and add them to your cart</p>

      <!-- Loading -->
      <div v-if="loading" class="spinner"></div>

      <!-- Error -->
      <div v-else-if="error" class="alert alert-error">{{ error }}</div>

      <!-- Products -->
      <div v-else class="grid-4">
        <div v-for="p in products" :key="p.id" class="card" style="display:flex; flex-direction:column;">
          <img :src="p.imageUrl || 'https://via.placeholder.com/200x140?text=' + p.name"
            style="width:100%; height:140px; object-fit:cover; border-radius:8px; margin-bottom:1rem; background:#f1f5f9;" />
          <div style="flex:1;">
            <span class="badge badge-blue" style="margin-bottom:.5rem;">{{ p.category }}</span>
            <h3 style="font-size:1rem; margin:.4rem 0;">{{ p.name }}</h3>
            <p style="color:var(--text-muted); font-size:.8rem; margin-bottom:.8rem;">{{ p.description }}</p>
          </div>
          <div style="display:flex; justify-content:space-between; align-items:center; margin-top:auto;">
            <span style="font-size:1.1rem; font-weight:800; color:var(--primary)">${{ p.price.toFixed(2) }}</span>
            <span style="font-size:.8rem; color:var(--text-muted);">Stock: {{ p.stock }}</span>
          </div>
          <button class="btn btn-primary btn-sm" style="margin-top:.8rem; width:100%;"
            :disabled="p.stock === 0"
            @click="addToCart(p)">
            {{ p.stock === 0 ? 'Out of Stock' : '+ Add to Cart' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import { useCartStore } from '../stores/cart'

const products = ref([])
const loading = ref(true)
const error = ref('')
const cart = useCartStore()

onMounted(async () => {
  try {
    const { data } = await axios.get('/api/products')
    products.value = data
  } catch {
    error.value = 'Failed to load products. Make sure the backend is running.'
  } finally {
    loading.value = false
  }
})

function addToCart(p) {
  cart.addItem(p)
  window.showToast(`${p.name} added to cart!`)
}
</script>
