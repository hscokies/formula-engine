<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute, useRouter } from 'vue-router';
import { Route } from '@/app/providers/router';
import { NAlert, NButton, NCard, NForm, NFormItem, NInputNumber, NSpace, NSpin, NText, useMessage } from 'naive-ui';
import { formulasApi } from '@/shared/api/formulasApi.ts';
import type { FieldType, FormulaArguments, GetFormulaResult } from '@/shared/api/formulas.types.ts';
import { getProblemMessage } from '@/shared/lib/errors/problemDetails.ts';
import { useAuthStore } from '@/shared/stores/auth.ts';

defineOptions({ name: 'FormulaPage' });

const { t } = useI18n();
const route = useRoute();
const router = useRouter();
const message = useMessage();
const { isAdmin } = useAuthStore();

const formula = ref<GetFormulaResult | null>(null);
const fieldValues = ref<Record<string, number | null>>({});
const loading = ref(true);
const acting = ref(false);
const errorMessage = ref<string | null>(null);
const evaluationId = ref<string | null>(null);
const result = ref<number | null>(null);
const calculatedSnapshot = ref<string | null>(null);

const formulaId = computed(() => String(route.params.id));

const sortedFields = computed(() => {
    if (!formula.value) {
        return [];
    }

    return Object.entries(formula.value.fields).map(([internalName, config]) => ({
        internalName,
        ...config,
    }));
});

const valuesSnapshot = computed(() => JSON.stringify(fieldValues.value));

const isCalculated = computed(
    () =>
        evaluationId.value !== null &&
        calculatedSnapshot.value !== null &&
        calculatedSnapshot.value === valuesSnapshot.value,
);

function buildArguments(): FormulaArguments {
    const args: FormulaArguments = {};

    for (const field of sortedFields.value) {
        const value = fieldValues.value[field.internalName];
        if (value === null || value === undefined) {
            throw new Error(t('Pages.Formula.EnterValueFor', { label: field.label }));
        }

        if (field.type === 'Integer') {
            args[field.internalName] = { $type: 'Integer', value: Math.trunc(value) };
        } else {
            args[field.internalName] = { $type: 'Decimal', value };
        }
    }

    return args;
}

function resetCalculation() {
    evaluationId.value = null;
    result.value = null;
    calculatedSnapshot.value = null;
}

watch(valuesSnapshot, snapshot => {
    if (calculatedSnapshot.value !== null && calculatedSnapshot.value !== snapshot) {
        resetCalculation();
    }
});

async function loadFormula() {
    loading.value = true;
    errorMessage.value = null;
    resetCalculation();

    try {
        formula.value = await formulasApi.get(formulaId.value);
        fieldValues.value = Object.fromEntries(Object.keys(formula.value.fields).map(fieldName => [fieldName, null]));
    } catch (error) {
        errorMessage.value = getProblemMessage(error, t('Errors.UnableToLoadFormula'));
    } finally {
        loading.value = false;
    }
}

async function handleCalculate() {
    acting.value = true;
    errorMessage.value = null;
    resetCalculation();

    try {
        const args = buildArguments();
        const response = await formulasApi.evaluate(formulaId.value, args);
        evaluationId.value = response.key;
        result.value = response.result;
        calculatedSnapshot.value = valuesSnapshot.value;
    } catch (error) {
        errorMessage.value = getProblemMessage(error, t('Errors.UnableToCalculateFormula'));
    } finally {
        acting.value = false;
    }
}

async function handleSubmit() {
    if (!evaluationId.value) {
        return;
    }

    acting.value = true;
    errorMessage.value = null;

    try {
        const args = buildArguments();
        await formulasApi.submit(formulaId.value, evaluationId.value, args);
        message.success(t('Messages.CalculationSubmitted'));
        resetCalculation();
    } catch (error) {
        errorMessage.value = getProblemMessage(error, t('Errors.UnableToSubmitCalculation'));
    } finally {
        acting.value = false;
    }
}

function inputPrecision(type: FieldType) {
    return type === 'Integer' ? 0 : 2;
}

onMounted(loadFormula);
</script>

<template>
    <NSpin :show="loading">
        <NCard v-if="formula" :title="formula.name">
            <NSpace align="center" justify="space-between" :class="$cn('header')">
                <NText depth="3">{{ t('Pages.Formula.Description') }}</NText>
                <NButton
                    v-if="isAdmin"
                    quaternary
                    @click="router.push({ name: Route.ViewCalculations, params: { id: formulaId } })"
                >
                    {{ t('Pages.Formula.ViewSubmissions') }}
                </NButton>
            </NSpace>

            <NAlert v-if="errorMessage" type="error" :class="$cn('alert')">
                {{ errorMessage }}
            </NAlert>

            <NForm :class="$cn('form')">
                <NFormItem v-for="field in sortedFields" :key="field.internalName" :label="field.label">
                    <NInputNumber
                        v-model:value="fieldValues[field.internalName]"
                        :class="$cn('input')"
                        :precision="inputPrecision(field.type)"
                        :placeholder="field.type"
                    />
                </NFormItem>

                <NAlert v-if="result !== null" type="info" :class="$cn('result')">
                    {{ t('Common.Result') }}: <strong>{{ result }}</strong>
                </NAlert>

                <NSpace>
                    <NButton v-if="isCalculated" type="primary" :loading="acting" @click="handleSubmit">
                        {{ t('Pages.Formula.Submit') }}
                    </NButton>
                    <NButton v-else type="primary" :loading="acting" @click="handleCalculate">
                        {{ t('Pages.Formula.Calculate') }}
                    </NButton>
                </NSpace>
            </NForm>
        </NCard>
    </NSpin>
</template>

<style scoped>
.formula-page__header {
    width: 100%;
}

.formula-page__alert {
    margin-top: 16px;
}

.formula-page__form {
    margin-top: 24px;
}

.formula-page__input {
    width: 100%;
    max-width: 320px;
}

.formula-page__result {
    margin-bottom: 16px;
}
</style>
