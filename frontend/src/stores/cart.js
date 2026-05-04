import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useCartStore = defineStore('cart', () => {
  const items = ref(JSON.parse(localStorage.getItem('cart') || '[]'))

  const total = computed(() => items.value.reduce((s, i) => s + i.price * i.qty, 0))
  const count = computed(() => items.value.reduce((s, i) => s + i.qty, 0))

  function save() { localStorage.setItem('cart', JSON.stringify(items.value)) }

  function addItem(product) {
    const existing = items.value.find(i => i.id === product.id)
    if (existing) existing.qty++
    else items.value.push({ id: product.id, name: product.name, price: product.price, qty: 1 })
    save()
  }

  function removeItem(id) {
    items.value = items.value.filter(i => i.id !== id)
    save()
  }

  function updateQty(id, qty) {
    const item = items.value.find(i => i.id === id)
    if (item) { item.qty = Math.max(1, qty); save() }
  }

  function clear() { items.value = []; save() }

  return { items, total, count, addItem, removeItem, updateQty, clear }
})
