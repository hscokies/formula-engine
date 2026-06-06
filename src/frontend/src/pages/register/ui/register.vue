<script setup lang="ts">
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';
import { NAlert, NButton, NCard, NForm, NFormItem, NInput, NSpace, NText } from 'naive-ui';
import { Route } from '@/app/providers/router';
import { getProblemMessage } from '@/shared/lib/errors/problemDetails.ts';
import { useAuthStore } from '@/shared/stores/auth.ts';

defineOptions({ name: 'RegisterPage' });

const { t } = useI18n();
const router = useRouter();
const authStore = useAuthStore();

const email = ref('');
const password = ref('');
const confirmPassword = ref('');
const loading = ref(false);
const errorMessage = ref<string | null>(null);

async function handleSubmit() {
    if (password.value !== confirmPassword.value) {
        errorMessage.value = t('Errors.PasswordsDoNotMatch');
        return;
    }

    if (password.value.length < 12) {
        errorMessage.value = t('Errors.PasswordTooShort');
        return;
    }

    loading.value = true;
    errorMessage.value = null;

    try {
        await authStore.register(email.value, password.value);
        await router.push({ name: Route.ViewFormulas });
    } catch (error) {
        errorMessage.value = getProblemMessage(error, t('Errors.UnableToCreateAccount'));
    } finally {
        loading.value = false;
    }
}
</script>

<template>
    <div :class="$cn()">
        <NCard :class="$cn('card')" :title="t('Pages.Register.Title')">
            <NText depth="3">{{ t('Pages.Register.Description') }}</NText>

            <NAlert v-if="errorMessage" type="error" :class="$cn('alert')">
                {{ errorMessage }}
            </NAlert>

            <NForm :class="$cn('form')" @submit.prevent="handleSubmit">
                <NFormItem :label="t('Common.Email')">
                    <NInput
                        v-model:value="email"
                        type="text"
                        :placeholder="t('Pages.Register.EmailPlaceholder')"
                    />
                </NFormItem>

                <NFormItem :label="t('Common.Password')">
                    <NInput
                        v-model:value="password"
                        type="password"
                        show-password-on="click"
                        :placeholder="t('Pages.Register.PasswordPlaceholder')"
                    />
                </NFormItem>

                <NFormItem :label="t('Pages.Register.ConfirmPassword')">
                    <NInput
                        v-model:value="confirmPassword"
                        type="password"
                        show-password-on="click"
                        :placeholder="t('Pages.Register.ConfirmPasswordPlaceholder')"
                    />
                </NFormItem>

                <NSpace vertical>
                    <NButton type="primary" attr-type="submit" block :loading="loading">
                        {{ t('Pages.Register.Submit') }}
                    </NButton>
                    <NButton text block @click="router.push({ name: Route.Login })">
                        {{ t('Pages.Register.SignIn') }}
                    </NButton>
                </NSpace>
            </NForm>
        </NCard>
    </div>
</template>

<style scoped>
.register-page {
    min-height: calc(100vh - 48px);
    display: flex;
    align-items: center;
    justify-content: center;
}

.register-page__card {
    width: 100%;
    max-width: 420px;
}

.register-page__alert {
    margin-top: 16px;
}

.register-page__form {
    margin-top: 24px;
}
</style>
