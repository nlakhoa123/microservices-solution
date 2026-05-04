import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const routes = [
  { path: '/', component: () => import('../views/HomeView.vue') },
  { path: '/login', component: () => import('../views/LoginView.vue') },
  { path: '/register', component: () => import('../views/RegisterView.vue') },
  { path: '/shop', component: () => import('../views/ShopView.vue') },
  { path: '/cart', component: () => import('../views/CartView.vue') },
  { path: '/orders', component: () => import('../views/OrdersView.vue'), meta: { requiresAuth: true } },
  { path: '/url-shortener', component: () => import('../views/UrlShortenerView.vue') },
  { path: '/contact', component: () => import('../views/ContactView.vue') },
  { path: '/profile', component: () => import('../views/ProfileView.vue'), meta: { requiresAuth: true } },
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to) => {
  const auth = useAuthStore()
  if (to.meta.requiresAuth && !auth.isLoggedIn) {
    return '/login'
  }
})

export default router
