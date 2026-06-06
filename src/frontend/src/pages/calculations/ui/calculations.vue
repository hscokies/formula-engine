<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute, useRouter } from 'vue-router';
import {
    NAlert,
    NButton,
    NCard,
    NEmpty,
    NSpace,
    NSpin,
    NTable,
    NText,
    NTh,
    NTr,
    NTd,
    NThead,
    NTbody,
} from 'naive-ui';
import { Route } from '@/app/providers/router';
import { formulasApi } from '@/shared/api/formulasApi.ts';
import type { GetFormulaResult, SubmissionItem } from '@/shared/api/formulas.types.ts';
import { getProblemMessage } from '@/shared/lib/errors/problemDetails.ts';

defineOptions({ name: 'CalculationsPage' });

const { t } = useI18n();
const route = useRoute();
const router = useRouter();

const formula = ref<GetFormulaResult | null>(null);
const submissions = ref<SubmissionItem[]>([]);
const loading = ref(true);
const errorMessage = ref<string | null>(null);

const formulaId = computed(() => String(route.params.id));

const pageTitle = computed(() =>
    formula.value
        ? t('Pages.ViewCalculations.TitleWithName', { name: formula.value.name })
        : t('Pages.ViewCalculations.TitleFallback'),
);

const showUserColumn = computed(() => {
    const userIds = new Set(submissions.value.map(submission => submission.userId));
    return userIds.size > 1;
});

async function loadPage() {
    loading.value = true;
    errorMessage.value = null;

    try {
        const [formulaResult, submissionsResult] = await Promise.all([
            formulasApi.get(formulaId.value),
            formulasApi.listSubmissions(formulaId.value),
        ]);

        formula.value = formulaResult;
        submissions.value = submissionsResult.items;
    } catch (error) {
        errorMessage.value = getProblemMessage(error, t('Errors.UnableToLoadSubmissions'));
    } finally {
        loading.value = false;
    }
}

onMounted(loadPage);
</script>

<template>
    <NSpin :show="loading">
        <NCard :title="pageTitle">
            <NSpace vertical>
                <NText depth="3">{{ t('Pages.ViewCalculations.Description') }}</NText>

                <NAlert v-if="errorMessage" type="error" :class="$cn('alert')">
                    {{ errorMessage }}
                </NAlert>

                <NTable v-if="submissions.length" :bordered="false" :single-line="false" :class="$cn('table')">
                    <NThead>
                        <NTr>
                            <NTh>{{ t('Common.SubmissionId') }}</NTh>
                            <NTh v-if="showUserColumn">{{ t('Common.UserId') }}</NTh>
                            <NTh>{{ t('Common.Result') }}</NTh>
                        </NTr>
                    </NThead>
                    <NTbody>
                        <NTr v-for="submission in submissions" :key="submission.id">
                            <NTd>
                                <code>{{ submission.id }}</code>
                            </NTd>
                            <NTd v-if="showUserColumn">
                                <code>{{ submission.userId }}</code>
                            </NTd>
                            <NTd>{{ submission.result }}</NTd>
                        </NTr>
                    </NTbody>
                </NTable>

                <NEmpty
                    v-else-if="!loading && !errorMessage"
                    :description="t('Pages.ViewCalculations.Empty')"
                />

                <NSpace :class="$cn('actions')">
                    <NButton @click="router.push({ name: Route.Formula, params: { id: formulaId } })">
                        {{ t('Pages.ViewCalculations.BackToFormula') }}
                    </NButton>
                    <NButton @click="loadPage">{{ t('Common.Refresh') }}</NButton>
                </NSpace>
            </NSpace>
        </NCard>
    </NSpin>
</template>

<style scoped>
.calculations-page__actions {
    margin-top: 8px;
}
</style>
