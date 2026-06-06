<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { NAlert, NButton, NCard, NForm, NFormItem, NInput, NSpace, NText } from 'naive-ui';
import { Route } from '@/app/providers/router';
import { getProblemMessage } from '@/shared/lib/errors/problemDetails.ts';
import { useAuthStore } from '@/shared/stores/auth.ts';

defineOptions({ name: 'RegisterPage' });

const router = useRouter();
const authStore = useAuthStore();

const email = ref('');
const password = ref('');
const confirmPassword = ref('');
const loading = ref(false);
const errorMessage = ref<string | null>(null);

async function handleSubmit() {
    if (password.value !== confirmPassword.value) {
        errorMessage.value = 'Passwords do not match.';
        return;
    }

    if (password.value.length < 12) {
        errorMessage.value = 'Password must be at least 12 characters long.';
        return;
    }

    loading.value = true;
    errorMessage.value = null;

    try {
        await authStore.register(email.value, password.value);
        await router.push({ name: Route.ViewFormulas });
    } catch (error) {
        errorMessage.value = getProblemMessage(error, 'Unable to create account.');
    } finally {
        loading.value = false;
    }
}
</script>

<template>
    <div class="auth-page">
        <NCard class="auth-page__card" title="Create account">
            <NText depth="3">
                Password must be at least 12 characters and include upper, lower, digit, and special
                characters.
            </NText>

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
                        placeholder="Create a password"
                    />
                </NFormItem>

                <NFormItem label="Confirm password">
                    <NInput
                        v-model:value="confirmPassword"
                        type="password"
                        show-password-on="click"
                        placeholder="Repeat your password"
                    />
                </NFormItem>

                <NSpace vertical>
                    <NButton type="primary" attr-type="submit" block :loading="loading">Sign up</NButton>
                    <NButton text block @click="router.push({ name: Route.Login })">
                        Already have an account? Sign in
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
