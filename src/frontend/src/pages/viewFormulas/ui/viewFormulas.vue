<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import { NAlert, NButton, NCard, NEmpty, NInput, NList, NListItem, NSpace, NSpin, NText, useMessage } from 'naive-ui';
import { Route } from '@/app/providers/router';
import { formulasApi } from '@/shared/api/formulasApi.ts';
import { copyFormulaLink } from '@/shared/lib/formula/copyFormulaLink.ts';
import { getProblemMessage } from '@/shared/lib/errors/problemDetails.ts';
import { useAuthStore } from '@/shared/stores/auth.ts';

defineOptions({ name: 'ViewFormulasPage' });

const router = useRouter();
const message = useMessage();
const authStore = useAuthStore();

const items = ref<Array<{ id: string; name: string }>>([]);
const search = ref('');
const loading = ref(true);
const errorMessage = ref<string | null>(null);

async function loadFormulas() {
    if (!authStore.isAdmin) {
        loading.value = false;
        return;
    }

    loading.value = true;
    errorMessage.value = null;

    try {
        const response = await formulasApi.list(search.value || undefined);
        items.value = response.items;
    } catch (error) {
        errorMessage.value = getProblemMessage(error, 'Unable to load formulas.');
    } finally {
        loading.value = false;
    }
}

async function handleCopyLink(formulaId: string) {
    try {
        await copyFormulaLink(router, formulaId);
        message.success('Form link copied to clipboard');
    } catch {
        message.error('Unable to copy link');
    }
}

onMounted(loadFormulas);
</script>

<template>
    <NCard title="Formulas">
        <NSpace vertical>
            <NText depth="3">
                Open a formula to calculate and submit values. Admins can create new formulas.
            </NText>

            <NAlert v-if="!authStore.isAdmin" type="info">
                Formula listing is available to administrators. Open a formula directly if you have
                its link.
            </NAlert>

            <NAlert v-if="errorMessage" type="error">
                {{ errorMessage }}
            </NAlert>

            <NSpace v-if="authStore.isAdmin">
                <NInput v-model:value="search" placeholder="Search by name" />
                <NButton @click="loadFormulas">Search</NButton>
                <NButton type="primary" @click="router.push({ name: Route.CreateFormula })">
                    Create formula
                </NButton>
            </NSpace>

            <NSpin :show="loading">
                <NList v-if="authStore.isAdmin && items.length" bordered>
                    <NListItem v-for="item in items" :key="item.id">
                        <NSpace align="center" justify="space-between" style="width: 100%">
                            <NText>{{ item.name }}</NText>
                            <NSpace>
                                <NButton @click="handleCopyLink(item.id)">Copy link</NButton>
                                <NButton @click="router.push({ name: Route.Formula, params: { id: item.id } })">
                                    Open
                                </NButton>
                            </NSpace>
                        </NSpace>
                    </NListItem>
                </NList>

                <NEmpty
                    v-else-if="authStore.isAdmin && !loading"
                    description="No formulas found"
                />
            </NSpin>
        </NSpace>
    </NCard>
</template>
