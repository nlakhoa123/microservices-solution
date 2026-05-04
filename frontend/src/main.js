import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'
import './assets/main.css'
import axios from 'axios'

// Tự động dùng URL backend thật khi production, localhost khi dev
axios.defaults.baseURL = import.meta.env.VITE_API_URL || ''

const app = createApp(App)
app.use(createPinia())
app.use(router)
app.mount('#app')
