import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import axios from 'axios'

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('token') || '')
  const user = ref(JSON.parse(localStorage.getItem('user') || 'null'))

  const isLoggedIn = computed(() => !!token.value)

  function setAxiosAuth() {
    if (token.value) {
      axios.defaults.headers.common['Authorization'] = `Bearer ${token.value}`
    } else {
      delete axios.defaults.headers.common['Authorization']
    }
  }

  setAxiosAuth()

  async function login(email, password) {
    const { data } = await axios.post('/api/Auth/login', { email, password })
    token.value = data.token
    user.value = { id: data.userId, username: data.username, email: data.email, role: data.role }
    localStorage.setItem('token', data.token)
    localStorage.setItem('user', JSON.stringify(user.value))
    setAxiosAuth()
    return data
  }

  async function register(username, email, password) {
    const { data } = await axios.post('/api/Auth/register', { username, email, password })
    token.value = data.token
    user.value = { id: data.userId, username: data.username, email: data.email, role: data.role }
    localStorage.setItem('token', data.token)
    localStorage.setItem('user', JSON.stringify(user.value))
    setAxiosAuth()
    return data
  }

  function logout() {
    token.value = ''
    user.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    setAxiosAuth()
  }

  return { token, user, isLoggedIn, login, register, logout }
})