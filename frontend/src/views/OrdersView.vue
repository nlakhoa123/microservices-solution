<template>
  <div class="page">
    <div class="container">
      <h1 class="section-title">My Orders</h1>

      <div v-if="loading" class="spinner"></div>
      <div v-else-if="orders.length === 0" class="card" style="text-align:center; padding:3rem;">
        <p style="color:var(--text-muted);">No orders yet.</p>
        <router-link to="/shop" class="btn btn-primary" style="margin-top:1rem;">Start Shopping</router-link>
      </div>
      <div v-else>
        <div v-for="order in orders" :key="order.id" class="card" style="margin-bottom:1rem;">
          <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom:1rem;">
            <div>
              <span style="font-weight:800; font-size:1.05rem;">Order #{{ order.id }}</span>
              <span class="badge badge-green" style="margin-left:.5rem;">{{ order.status }}</span>
            </div>
            <div style="text-align:right;">
              <div style="font-size:1.1rem; font-weight:800; color:var(--primary)">${{ order.finalAmount.toFixed(2) }}</div>
              <div style="font-size:.8rem; color:var(--text-muted);">{{ new Date(order.createdAt).toLocaleDateString() }}</div>
            </div>
          </div>
          <table class="table">
            <thead>
              <tr><th>Product</th><th>Unit Price</th><th>Qty</th><th>Subtotal</th></tr>
            </thead>
            <tbody>
              <tr v-for="(item, i) in order.items" :key="i">
                <td>{{ item.productName }}</td>
                <td>${{ item.unitPrice.toFixed(2) }}</td>
                <td>{{ item.quantity }}</td>
                <td>${{ item.subtotal.toFixed(2) }}</td>
              </tr>
            </tbody>
          </table>
          <div style="margin-top:.8rem; display:flex; gap:1.5rem; font-size:.85rem; color:var(--text-muted);">
            <span>Payment: <strong>{{ order.paymentMethod }}</strong></span>
            <span v-if="order.discountAmount > 0">Discount: <strong style="color:var(--success);">−${{ order.discountAmount.toFixed(2) }}</strong></span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import { useAuthStore } from '../stores/auth'

const auth = useAuthStore()
const orders = ref([])
const loading = ref(true)

onMounted(async () => {
  try {
    const { data } = await axios.get(`/api/orders/customer/${auth.user.id}`)
    orders.value = data
  } catch { /* empty */ } finally {
    loading.value = false
  }
})
</script>
