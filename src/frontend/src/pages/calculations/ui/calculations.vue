<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
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

const route = useRoute();
const router = useRouter();

const formula = ref<GetFormulaResult | null>(null);
const submissions = ref<SubmissionItem[]>([]);
const loading = ref(true);
const errorMessage = ref<string | null>(null);

const formulaId = computed(() => String(route.params.id));

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
        errorMessage.value = getProblemMessage(error, 'Unable to load submissions.');
    } finally {
        loading.value = false;
    }
}

onMounted(loadPage);
</script>

<template>
    <NSpin :show="loading">
        <NCard :title="formula ? `Submissions — ${formula.name}` : 'Submissions'">
            <NSpace vertical>
                <NText depth="3">Submitted calculation results for this formula.</NText>

                <NAlert v-if="errorMessage" type="error">
                    {{ errorMessage }}
                </NAlert>

                <NTable v-if="submissions.length" :bordered="false" :single-line="false">
                    <NThead>
                        <NTr>
                            <NTh>Submission ID</NTh>
                            <NTh>User ID</NTh>
                            <NTh>Result</NTh>
                        </NTr>
                    </NThead>
                    <NTbody>
                        <NTr v-for="submission in submissions" :key="submission.id">
                            <NTd>
                                <code>{{ submission.id }}</code>
                            </NTd>
                            <NTd>
                                <code>{{ submission.userId }}</code>
                            </NTd>
                            <NTd>{{ submission.result }}</NTd>
                        </NTr>
                    </NTbody>
                </NTable>

                <NEmpty v-else-if="!loading && !errorMessage" description="No submissions yet" />

                <NSpace>
                    <NButton @click="router.push({ name: Route.Formula, params: { id: formulaId } })">
                        Back to formula
                    </NButton>
                    <NButton @click="loadPage">Refresh</NButton>
                </NSpace>
            </NSpace>
        </NCard>
    </NSpin>
</template>
