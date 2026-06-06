<script setup lang="ts">
import { ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { NAlert, NButton, NCard, NForm, NFormItem, NInput, NSpace, NText } from 'naive-ui';
import { Route } from '@/app/providers/router';
import { getProblemMessage } from '@/shared/lib/errors/problemDetails.ts';
import { useAuthStore } from '@/shared/stores/auth.ts';

defineOptions({ name: 'LoginPage' });

const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();

const email = ref('');
const password = ref('');
const loading = ref(false);
const errorMessage = ref<string | null>(null);

async function handleSubmit() {
    loading.value = true;
    errorMessage.value = null;

    try {
        await authStore.login(email.value, password.value);
        const redirect = typeof route.query.redirect === 'string' ? route.query.redirect : null;
        await router.push(redirect ?? { name: Route.ViewFormulas });
    } catch (error) {
        errorMessage.value = getProblemMessage(error, 'Unable to sign in. Check your credentials.');
    } finally {
        loading.value = false;
    }
}
</script>

<template>
    <div class="auth-page">
        <NCard class="auth-page__card" title="Sign in">
            <NText depth="3">Use your account to access formulas.</NText>

            <NAlert v-if="errorMessage" type="error" class="auth-page__alert">
                {{ errorMessage }}
            </NAlert>

            <NForm class="auth-page__form" @submit.prevent="handleSubmit">
                <NFormItem label="Email">
                    <NInput v-model:value="email" type="text" placeholder="you@example.com" />
                </NFormItem>

                <NFormItem label="Password">
                    <NInput
                        v-model:value="password"
                        type="password"
                        show-password-on="click"
                        placeholder="Your password"
                    />
                </NFormItem>

                <NSpace vertical>
                    <NButton type="primary" attr-type="submit" block :loading="loading">Sign in</NButton>
                    <NButton text block @click="router.push({ name: Route.Register })">
                        Create an account
                    </NButton>
                </NSpace>
            </NForm>
        </NCard>
    </div>
</template>

<style scoped>
.auth-page {
    min-height: calc(100vh - 48px);
    display: flex;
    align-items: center;
    justify-content: center;
}

.auth-page__card {
    width: 100%;
    max-width: 420px;
}

.auth-page__alert {
    margin-top: 16px;
}

.auth-page__form {
    margin-top: 24px;
}
</style>
