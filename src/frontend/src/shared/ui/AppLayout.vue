<script setup lang="ts">
import { computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { NButton, NLayout, NLayoutContent, NLayoutHeader, NSpace, NText } from 'naive-ui';
import { Route } from '@/app/providers/router';
import { useAuthStore } from '@/shared/stores/auth.ts';

defineOptions({ name: 'AppLayout' });

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
    <NLayout class="app-layout">
        <NLayoutHeader v-if="!isAuthPage" bordered class="app-layout__header">
            <NSpace align="center" justify="space-between" class="app-layout__header-inner">
                <NText strong class="app-layout__brand" @click="router.push({ name: Route.ViewFormulas })">
                    Formula Engine
                </NText>

                <NSpace>
                    <NButton
                        v-if="authStore.isAdmin"
                        quaternary
                        @click="router.push({ name: Route.CreateFormula })"
                    >
                        Create formula
                    </NButton>
                    <NButton quaternary @click="router.push({ name: Route.ViewFormulas })">Formulas</NButton>
                    <NButton quaternary @click="handleLogout">Sign out</NButton>
                </NSpace>
            </NSpace>
        </NLayoutHeader>

        <NLayoutContent class="app-layout__content">
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
