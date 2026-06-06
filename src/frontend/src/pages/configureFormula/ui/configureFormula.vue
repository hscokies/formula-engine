<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';
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

const { t } = useI18n();
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

const typeOptions = computed(() => [
    { label: t('Fields.Integer'), value: 'Integer' as FieldType },
    { label: t('Fields.Decimal'), value: 'Decimal' as FieldType },
]);

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
        errorMessage.value = getProblemMessage(error, t('Errors.UnableToLoadFormulaConfiguration'));
    } finally {
        loading.value = false;
    }
});

async function handleSave() {
    if (!name.value.trim()) {
        errorMessage.value = t('Errors.EnterFormulaName');
        return;
    }

    saving.value = true;
    errorMessage.value = null;

    try {
        const fields = Object.fromEntries(
            fieldNames.value.map(fieldName => [
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
        errorMessage.value = getProblemMessage(error, t('Errors.UnableToSaveFormulaConfiguration'));
    } finally {
        saving.value = false;
    }
}
</script>

<template>
    <NSpin :show="loading">
        <NCard :title="t('Pages.ConfigureFormula.CardTitle')">
            <NText depth="3">{{ t('Pages.ConfigureFormula.Description') }}</NText>

            <NAlert v-if="errorMessage" type="error" :class="$cn('alert')">
                {{ errorMessage }}
            </NAlert>

            <NForm v-if="!loading" :class="$cn('form')" @submit.prevent="handleSave">
                <NFormItem v-if="expression" :label="t('Common.Expression')">
                    <NInput :value="expression" type="textarea" :autosize="{ minRows: 2 }" readonly />
                </NFormItem>

                <NFormItem :label="t('Pages.ConfigureFormula.FormName')">
                    <NInput
                        v-model:value="name"
                        :placeholder="t('Pages.ConfigureFormula.FormNamePlaceholder')"
                    />
                </NFormItem>

                <NText strong>{{ t('Common.Fields') }}</NText>
                <NTable :bordered="false" :single-line="false" :class="$cn('table')">
                    <NThead>
                        <NTr>
                            <NTh>{{ t('Common.InternalName') }}</NTh>
                            <NTh>{{ t('Common.Label') }}</NTh>
                            <NTh>{{ t('Common.Type') }}</NTh>
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

                <NSpace :class="$cn('actions')">
                    <NButton type="primary" attr-type="submit" :loading="saving">
                        {{ t('Pages.ConfigureFormula.Save') }}
                    </NButton>
                    <NButton @click="router.push({ name: Route.ViewFormulas })">
                        {{ t('Common.Cancel') }}
                    </NButton>
                </NSpace>
            </NForm>
        </NCard>
    </NSpin>
</template>

<style scoped>
.configure-formula-page__alert {
    margin-top: 16px;
}

.configure-formula-page__form {
    margin-top: 24px;
}

.configure-formula-page__table {
    margin-top: 12px;
}

.configure-formula-page__actions {
    margin-top: 24px;
}
</style>
