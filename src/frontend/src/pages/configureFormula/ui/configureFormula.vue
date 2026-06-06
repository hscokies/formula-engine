<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import {
    NAlert,
    NButton,
    NCard,
    NForm,
    NFormItem,
    NInput,
    NSelect,
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
import type { FieldType } from '@/shared/api/formulas.types.ts';
import { getProblemMessage } from '@/shared/lib/errors/problemDetails.ts';

defineOptions({ name: 'ConfigureFormulaPage' });

interface ConfigureNavigationState {
    name?: string;
    expression?: string;
    fields?: string[];
}

const route = useRoute();
const router = useRouter();

const formulaId = ref(String(route.params.id));
const expression = ref('');
const name = ref('');
const fieldNames = ref<string[]>([]);
const fieldLabels = ref<Record<string, string>>({});
const fieldTypes = ref<Record<string, FieldType>>({});
const loading = ref(true);
const saving = ref(false);
const errorMessage = ref<string | null>(null);

const typeOptions = [
    { label: 'Integer', value: 'Integer' as FieldType },
    { label: 'Decimal', value: 'Decimal' as FieldType },
];

function initializeFieldState(fields: string[]) {
    fieldNames.value = fields;
    const labels: Record<string, string> = {};
    const types: Record<string, FieldType> = {};

    for (const fieldName of fields) {
        labels[fieldName] = fieldLabels.value[fieldName] || fieldName;
        types[fieldName] = fieldTypes.value[fieldName] || 'Decimal';
    }

    fieldLabels.value = labels;
    fieldTypes.value = types;
}

async function loadFromApi() {
    const formula = await formulasApi.get(formulaId.value);
    name.value = formula.name;
    expression.value = (history.state as ConfigureNavigationState).expression ?? '';
    initializeFieldState(Object.keys(formula.fields));

    for (const [internalName, config] of Object.entries(formula.fields)) {
        fieldLabels.value[internalName] = config.label;
        fieldTypes.value[internalName] = config.type;
    }
}

onMounted(async () => {
    loading.value = true;
    errorMessage.value = null;

    try {
        const navigationState = history.state as ConfigureNavigationState;

        if (navigationState?.fields?.length) {
            name.value = navigationState.name ?? '';
            expression.value = navigationState.expression ?? '';
            initializeFieldState(navigationState.fields);
        } else {
            await loadFromApi();
        }
    } catch (error) {
        errorMessage.value = getProblemMessage(error, 'Unable to load formula configuration.');
    } finally {
        loading.value = false;
    }
});

async function handleSave() {
    if (!name.value.trim()) {
        errorMessage.value = 'Enter a formula name.';
        return;
    }

    saving.value = true;
    errorMessage.value = null;

    try {
        const fields = Object.fromEntries(
            fieldNames.value.map((fieldName) => [
                fieldName,
                {
                    label: fieldLabels.value[fieldName] || fieldName,
                    type: fieldTypes.value[fieldName] || 'Decimal',
                },
            ]),
        );

        await formulasApi.configure(formulaId.value, name.value.trim(), fields);
        await router.push({ name: Route.Formula, params: { id: formulaId.value } });
    } catch (error) {
        errorMessage.value = getProblemMessage(error, 'Unable to save formula configuration.');
    } finally {
        saving.value = false;
    }
}
</script>

<template>
    <NSpin :show="loading">
        <NCard title="Configure formula">
            <NText depth="3">
                Set the form name and configure each detected field before publishing.
            </NText>

            <NAlert v-if="errorMessage" type="error" class="configure-formula__alert">
                {{ errorMessage }}
            </NAlert>

            <NForm v-if="!loading" class="configure-formula__form" @submit.prevent="handleSave">
                <NFormItem v-if="expression" label="Expression">
                    <NInput :value="expression" type="textarea" :autosize="{ minRows: 2 }" readonly />
                </NFormItem>

                <NFormItem label="Form name">
                    <NInput v-model:value="name" placeholder="Sales tax calculator" />
                </NFormItem>

                <NText strong>Fields</NText>
                <NTable :bordered="false" :single-line="false" class="configure-formula__table">
                    <NThead>
                        <NTr>
                            <NTh>Internal name</NTh>
                            <NTh>Label</NTh>
                            <NTh>Type</NTh>
                        </NTr>
                    </NThead>
                    <NTbody>
                        <NTr v-for="fieldName in fieldNames" :key="fieldName">
                            <NTd>
                                <code>{{ fieldName }}</code>
                            </NTd>
                            <NTd>
                                <NInput v-model:value="fieldLabels[fieldName]" :placeholder="fieldName" />
                            </NTd>
                            <NTd>
                                <NSelect v-model:value="fieldTypes[fieldName]" :options="typeOptions" />
                            </NTd>
                        </NTr>
                    </NTbody>
                </NTable>

                <NSpace class="configure-formula__actions">
                    <NButton type="primary" attr-type="submit" :loading="saving">Save formula</NButton>
                    <NButton @click="router.push({ name: Route.ViewFormulas })">Cancel</NButton>
                </NSpace>
            </NForm>
        </NCard>
    </NSpin>
</template>

<style scoped>
.configure-formula__alert {
    margin-top: 16px;
}

.configure-formula__form {
    margin-top: 24px;
}

.configure-formula__table {
    margin-top: 12px;
}

.configure-formula__actions {
    margin-top: 24px;
}
</style>
