<script setup lang="ts">
import { computed, nextTick, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'

import { useAuthSession } from './authSession'

const route = useRoute()
const router = useRouter()
const { login } = useAuthSession()

const username = ref('')
const password = ref('')
const loading = ref(false)
const error = ref<string | null>(null)
const usernameInput = ref<HTMLInputElement | null>(null)

const canSubmit = computed(() => username.value.trim() !== '' && password.value !== '')
const sessionNotice = computed(() => {
  const reason = route.query.reason
  const value = Array.isArray(reason) ? reason[0] : reason

  return value === 'session-check-failed'
    ? 'No hemos podido confirmar tu sesion. Vuelve a iniciar sesion para continuar con seguridad.'
    : null
})
const feedbackId = computed(() => {
  if (error.value) {
    return 'login-error-message'
  }

  return sessionNotice.value ? 'login-session-notice' : undefined
})
const credentialsMissing = computed(() => Boolean(error.value) && !canSubmit.value)

onMounted(() => {
  void nextTick(() => usernameInput.value?.focus())
})

function targetAfterLogin() {
  const redirect = route.query.redirect
  const value = Array.isArray(redirect) ? redirect[0] : redirect
  return value?.startsWith('/') && !value.startsWith('//') ? value : '/polizas'
}

async function submitLogin() {
  error.value = null

  if (!canSubmit.value) {
    error.value = 'Introduce usuario y contrasena.'
    return
  }

  loading.value = true
  try {
    await login({ username: username.value, password: password.value })
    await router.replace(targetAfterLogin())
  } catch (exception) {
    error.value = exception instanceof Error ? exception.message : 'No se pudo iniciar sesion.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <main class="login-page">
    <a class="skip-link" href="#login-form">Saltar al formulario</a>

    <section class="login-brand" aria-label="iLiniumTech">
      <div class="login-mark" aria-hidden="true">
        <span></span>
      </div>
      <p class="section-kicker">MVP</p>
      <h1>iLiniumTech</h1>
    </section>

    <section class="login-panel" aria-labelledby="login-title">
      <div>
        <p class="section-kicker">Acceso</p>
        <h2 id="login-title">Iniciar sesion</h2>
      </div>

      <form
        id="login-form"
        class="login-form"
        novalidate
        :aria-busy="loading"
        :aria-describedby="feedbackId"
        @submit.prevent="submitLogin"
      >
        <label class="login-field" for="auth-username">
          <span>Usuario</span>
          <input
            id="auth-username"
            ref="usernameInput"
            v-model="username"
            name="username"
            type="text"
            autocomplete="username"
            :aria-invalid="credentialsMissing ? 'true' : undefined"
            required
          />
        </label>

        <label class="login-field" for="auth-password">
          <span>Contrasena</span>
          <input
            id="auth-password"
            v-model="password"
            name="password"
            type="password"
            autocomplete="current-password"
            :aria-invalid="credentialsMissing ? 'true' : undefined"
            required
          />
        </label>

        <p
          v-if="sessionNotice && !error"
          id="login-session-notice"
          class="login-notice"
          role="status"
        >
          {{ sessionNotice }}
        </p>

        <p v-if="error" id="login-error-message" class="login-error" role="alert">
          {{ error }}
        </p>

        <button class="login-submit" type="submit" :disabled="loading">
          <i class="pi pi-sign-in" aria-hidden="true"></i>
          <span>{{ loading ? 'Entrando...' : 'Entrar' }}</span>
        </button>
        <span v-if="loading" class="sr-only" role="status">Validando acceso.</span>
      </form>
    </section>
  </main>
</template>
