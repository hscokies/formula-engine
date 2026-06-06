<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';
import { NAlert, NButton, NCard, NEmpty, NInput, NList, NListItem, NSpace, NSpin, NText, useMessage } from 'naive-ui';
import { Route } from '@/app/providers/router';
import { formulasApi } from '@/shared/api/formulasApi.ts';
import { copyFormulaLink } from '@/shared/lib/formula/copyFormulaLink.ts';
import { getProblemMessage } from '@/shared/lib/errors/problemDetails.ts';
import { useAuthStore } from '@/shared/stores/auth.ts';

defineOptions({ name: 'ViewFormulasPage' });

const { t } = useI18n();
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
        errorMessage.value = getProblemMessage(error, t('Errors.UnableToLoadFormulas'));
    } finally {
        loading.value = false;
    }
}

async function handleCopyLink(formulaId: string) {
    try {
        await copyFormulaLink(router, formulaId);
        message.success(t('Messages.FormLinkCopied'));
    } catch {
        message.error(t('Errors.UnableToCopyLink'));
    }
}

onMounted(loadFormulas);
</script>

<template>
    <NCard :title="t('Pages.ViewFormulas.CardTitle')">
        <NSpace vertical>
            <NText depth="3">{{ t('Pages.ViewFormulas.Description') }}</NText>

            <NAlert v-if="!authStore.isAdmin" type="info">
                {{ t('Pages.ViewFormulas.AdminOnlyInfo') }}
            </NAlert>

            <NAlert v-if="errorMessage" type="error" :class="$cn('alert')">
                {{ errorMessage }}
            </NAlert>

            <NSpace v-if="authStore.isAdmin">
                <NInput v-model:value="search" :placeholder="t('Pages.ViewFormulas.SearchPlaceholder')" />
                <NButton @click="loadFormulas">{{ t('Common.Search') }}</NButton>
                <NButton type="primary" @click="router.push({ name: Route.CreateFormula })">
                    {{ t('Common.CreateFormula') }}
                </NButton>
            </NSpace>

            <NSpin :show="loading">
                <NList v-if="authStore.isAdmin && items.length" bordered>
                    <NListItem v-for="item in items" :key="item.id">
                        <NSpace align="center" justify="space-between" :class="$cn('list-item')">
                            <NText>{{ item.name }}</NText>
                            <NSpace>
                                <NButton @click="handleCopyLink(item.id)">{{ t('Common.CopyLink') }}</NButton>
                                <NButton @click="router.push({ name: Route.Formula, params: { id: item.id } })">
                                    {{ t('Common.Open') }}
                                </NButton>
                            </NSpace>
                        </NSpace>
                    </NListItem>
                </NList>

                <NEmpty
                    v-else-if="authStore.isAdmin && !loading"
                    :description="t('Pages.ViewFormulas.Empty')"
                />
            </NSpin>
        </NSpace>
    </NCard>
</template>

<style scoped>
.view-formulas-page__list-item {
    width: 100%;
}
</style>
