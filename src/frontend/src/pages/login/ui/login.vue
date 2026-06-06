<script setup lang="ts">
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute, useRouter } from 'vue-router';
import { NAlert, NButton, NCard, NForm, NFormItem, NInput, NSpace, NText } from 'naive-ui';
import { Route } from '@/app/providers/router';
import { getProblemMessage } from '@/shared/lib/errors/problemDetails.ts';
import { useAuthStore } from '@/shared/stores/auth.ts';

defineOptions({ name: 'LoginPage' });

const { t } = useI18n();
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
        errorMessage.value = getProblemMessage(error, t('Errors.UnableToSignIn'));
    } finally {
        loading.value = false;
    }
}
</script>

<template>
    <div :class="$cn()">
        <NCard :class="$cn('card')" :title="t('Pages.Login.Title')">
            <NText depth="3">{{ t('Pages.Login.Description') }}</NText>

            <NAlert v-if="errorMessage" type="error" :class="$cn('alert')">
                {{ errorMessage }}
            </NAlert>

            <NForm :class="$cn('form')" @submit.prevent="handleSubmit">
                <NFormItem :label="t('Common.Email')">
                    <NInput
                        v-model:value="email"
                        type="text"
                        :placeholder="t('Pages.Login.EmailPlaceholder')"
                    />
                </NFormItem>

                <NFormItem :label="t('Common.Password')">
                    <NInput
                        v-model:value="password"
                        type="password"
                        show-password-on="click"
                        :placeholder="t('Pages.Login.PasswordPlaceholder')"
                    />
                </NFormItem>

                <NSpace vertical>
                    <NButton type="primary" attr-type="submit" block :loading="loading">
                        {{ t('Pages.Login.Submit') }}
                    </NButton>
                    <NButton text block @click="router.push({ name: Route.Register })">
                        {{ t('Pages.Login.CreateAccount') }}
                    </NButton>
                </NSpace>
            </NForm>
        </NCard>
    </div>
</template>

<style scoped>
.login-page {
    min-height: calc(100vh - 48px);
    display: flex;
    align-items: center;
    justify-content: center;
}

.login-page__card {
    width: 100%;
    max-width: 420px;
}

.login-page__alert {
    margin-top: 16px;
}

.login-page__form {
    margin-top: 24px;
}
</style>
