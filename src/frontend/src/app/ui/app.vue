<script setup lang="ts">
import { computed } from 'vue';
import { useRoute } from 'vue-router';
import { NConfigProvider, NMessageProvider, NDialogProvider } from 'naive-ui';
import { Route } from '@/app/providers/router';
import AppLayout from '@/shared/ui/AppLayout.vue';

const route = useRoute();

const isAuthPage = computed(() => [Route.Login, Route.Register].includes(route.name as Route));
</script>

<template>
    <NConfigProvider>
        <NMessageProvider>
            <NDialogProvider>
                <AppLayout v-if="!isAuthPage">
                    <router-view />
                </AppLayout>
                <router-view v-else />
            </NDialogProvider>
        </NMessageProvider>
    </NConfigProvider>
</template>

<style>
body {
    margin: 0;
    font-family:
        system-ui,
        -apple-system,
        BlinkMacSystemFont,
        'Segoe UI',
        sans-serif;
    background: #f7f8fa;
    color: #1f2329;
}

code {
    font-family: ui-monospace, SFMono-Regular, Menlo, Consolas, monospace;
    background: #eef0f3;
    padding: 2px 6px;
    border-radius: 4px;
}
</style>
