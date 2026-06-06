<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute, useRouter } from 'vue-router';
import { NButton, NLayout, NLayoutContent, NLayoutHeader, NSpace, NText } from 'naive-ui';
import { Route } from '@/app/providers/router';
import { useAuthStore } from '@/shared/stores/auth.ts';

defineOptions({ name: 'AppLayout' });

const { t } = useI18n();
const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();

const isAuthPage = computed(() => [Route.Login, Route.Register].includes(route.name as Route));

async function handleLogout() {
    authStore.logout();
    await router.push({ name: Route.Login });
}
</script>

<template>
    <NLayout :class="$cn()">
        <NLayoutHeader v-if="!isAuthPage" bordered :class="$cn('header')">
            <NSpace align="center" justify="space-between" :class="$cn('header-inner')">
                <NText strong :class="$cn('brand')" @click="router.push({ name: Route.ViewFormulas })">
                    {{ t('Common.ProjectName') }}
                </NText>

                <NSpace>
                    <NButton
                        v-if="authStore.isAdmin"
                        quaternary
                        @click="router.push({ name: Route.CreateFormula })"
                    >
                        {{ t('Common.CreateFormula') }}
                    </NButton>
                    <NButton quaternary @click="router.push({ name: Route.ViewFormulas })">
                        {{ t('Common.Formulas') }}
                    </NButton>
                    <NButton quaternary @click="handleLogout">{{ t('Common.SignOut') }}</NButton>
                </NSpace>
            </NSpace>
        </NLayoutHeader>

        <NLayoutContent :class="$cn('content')">
            <slot />
        </NLayoutContent>
    </NLayout>
</template>

<style scoped>
.app-layout {
    min-height: 100vh;
}

.app-layout__header {
    padding: 0 24px;
    height: 64px;
    display: flex;
    align-items: center;
}

.app-layout__header-inner {
    width: 100%;
}

.app-layout__brand {
    cursor: pointer;
    font-size: 18px;
}

.app-layout__content {
    padding: 24px;
    max-width: 960px;
    margin: 0 auto;
    width: 100%;
    box-sizing: border-box;
}
</style>
