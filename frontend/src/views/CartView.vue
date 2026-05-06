<template>
  <div class="page">
    <div class="container">
      <h1 class="section-title">Your Cart</h1>

      <div v-if="cart.items.length === 0" class="card" style="text-align:center; padding:4rem;">
        <p style="color:var(--text-muted);">Your cart is empty.</p>
        <router-link to="/shop" class="btn btn-primary" style="margin-top:1rem;">Browse Shop</router-link>
      </div>

      <div v-else class="grid-2" style="gap:2rem; align-items:start;">
        <!-- Cart Items -->
        <div>
          <div v-for="item in cart.items" :key="item.id" class="card" style="display:flex; align-items:center; gap:1rem; margin-bottom:1rem;">
            <div style="flex:1;">
              <div style="font-weight:700;">{{ item.name }}</div>
              <div style="color:var(--text-muted); font-size:.85rem;">${{ item.price.toFixed(2) }} each</div>
            </div>
            <div style="display:flex; align-items:center; gap:.5rem;">
              <button class="btn btn-secondary btn-sm" @click="cart.updateQty(item.id, item.qty - 1)">−</button>
              <span style="min-width:2rem; text-align:center; font-weight:700;">{{ item.qty }}</span>
              <button class="btn btn-secondary btn-sm" @click="cart.updateQty(item.id, item.qty + 1)">+</button>
            </div>
            <div style="font-weight:800; color:var(--primary); min-width:5rem; text-align:right;">
              ${{ (item.price * item.qty).toFixed(2) }}
            </div>
            <button class="btn btn-danger btn-sm" @click="cart.removeItem(item.id)">✕</button>
          </div>
        </div>

        <!-- Order Summary -->
        <div class="card">
          <h3 style="margin-bottom:1.2rem;">Order Summary</h3>

          <div style="display:flex; justify-content:space-between; margin-bottom:.8rem;">
            <span style="color:var(--text-muted);">Subtotal</span>
            <span style="font-weight:700;">${{ cart.total.toFixed(2) }}</span>
          </div>

          <div v-if="discountAmt > 0" style="display:flex; justify-content:space-between; margin-bottom:.8rem; color:var(--success);">
            <span>Discount ({{ discountPct }}%)</span>
            <span>−${{ discountAmt.toFixed(2) }}</span>
          </div>

          <div style="display:flex; justify-content:space-between; margin-bottom:1.5rem; font-size:1.1rem; border-top:2px solid var(--border); padding-top:.8rem;">
            <span style="font-weight:800;">Total</span>
            <span style="font-weight:800; color:var(--primary);">${{ finalTotal.toFixed(2) }}</span>
          </div>

          <!-- Discount code -->
          <div class="form-group">
            <label>Discount Code</label>
            <div style="display:flex; gap:.5rem;">
              <input class="form-control" v-model="discountCode" placeholder="Enter code" :disabled="discountApplied" />
              <button class="btn btn-secondary btn-sm" @click="applyDiscount" :disabled="discountApplied || !auth.isLoggedIn">
                {{ discountApplied ? '✓' : 'Apply' }}
              </button>
            </div>
            <div v-if="discountMsg" :class="['alert', discountValid ? 'alert-success' : 'alert-error']" style="margin-top:.5rem;">{{ discountMsg }}</div>
            <p v-if="!auth.isLoggedIn" style="font-size:.8rem; color:var(--text-muted); margin-top:.3rem;">Login to use discount codes</p>
          </div>

          <!-- Payment method -->
          <div class="form-group">
            <label>Payment Method</label>
            <select class="form-control" v-model="paymentMethod">
              <option>Credit Card</option>
              <option>Debit Card</option>
              <option>PayPal</option>
              <option>Bank Transfer</option>
            </select>
          </div>

          <div v-if="orderError" class="alert alert-error">{{ orderError }}</div>
          <div v-if="orderSuccess" class="alert alert-success">{{ orderSuccess }}</div>

          <button class="btn btn-primary" style="width:100%;" :disabled="placing || !auth.isLoggedIn" @click="placeOrder">
            {{ placing ? 'Placing...' : auth.isLoggedIn ? '✓ Place Order' : 'Login to Checkout' }}
          </button>
          <router-link v-if="!auth.isLoggedIn" to="/login" class="btn btn-secondary" style="width:100%; margin-top:.5rem; justify-content:center;">Login</router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import axios from 'axios'
import { useCartStore } from '../stores/cart'
import { useAuthStore } from '../stores/auth'

const cart = useCartStore()
const auth = useAuthStore()

const discountCode = ref('')
const discountApplied = ref(false)
const discountValid = ref(false)
const discountPct = ref(0)
const discountMsg = ref('')
const paymentMethod = ref('Credit Card')
const placing = ref(false)
const orderError = ref('')
const orderSuccess = ref('')

const discountAmt = computed(() => discountValid.value ? cart.total * (discountPct.value / 100) : 0)
const finalTotal = computed(() => cart.total - discountAmt.value)

async function applyDiscount() {
  if (!discountCode.value || !auth.user) return
  try {
    const { data } = await axios.post('/api/discounts/validate', {
      code: discountCode.value,
      customerId: auth.user.id
    })
    discountValid.value = data.valid
    discountMsg.value = data.message
    if (data.valid) {
      discountPct.value = data.percentage
      discountApplied.value = true
    }
  } catch {
    discountMsg.value = 'Failed to validate discount code.'
  }
}

async function placeOrder() {
  if (!auth.user) return
  placing.value = true
  orderError.value = ''
  orderSuccess.value = ''
  try {
    const { data } = await axios.post('/api/orders', {
      customerId: auth.user.id,
      items: cart.items.map(i => ({ productId: i.id, quantity: i.qty })),
      discountCode: discountApplied.value ? discountCode.value : null,
      paymentMethod: paymentMethod.value
    })
    orderSuccess.value = `Order #${data.id} placed! Total paid: $${data.finalAmount.toFixed(2)}`
    cart.clear()
    discountApplied.value = false
    discountCode.value = ''
    window.showToast('Order placed successfully!')
  } catch (e) {
    orderError.value = e.response?.data?.message || 'Failed to place order.'
  } finally {
    placing.value = false
  }
}
</script>
