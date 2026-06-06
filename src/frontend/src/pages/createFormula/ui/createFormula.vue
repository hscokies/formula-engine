<script setup lang="ts">
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';
import { NAlert, NButton, NCard, NForm, NFormItem, NInput, NSpace, NText } from 'naive-ui';
import { Route } from '@/app/providers/router';
import { formulasApi } from '@/shared/api/formulasApi.ts';
import { getProblemMessage } from '@/shared/lib/errors/problemDetails.ts';

defineOptions({ name: 'CreateFormulaPage' });

const { t } = useI18n();
const router = useRouter();

const expression = ref('');
const name = ref('');
const loading = ref(false);
const errorMessage = ref<string | null>(null);

async function handleContinue() {
    const trimmedExpression = expression.value.trim();
    const trimmedName = name.value.trim();

    if (!trimmedExpression) {
        errorMessage.value = t('Errors.EnterFormulaExpression');
        return;
    }

    if (!trimmedName) {
        errorMessage.value = t('Errors.EnterFormulaName');
        return;
    }

    loading.value = true;
    errorMessage.value = null;

    try {
        const created = await formulasApi.create(trimmedName, trimmedExpression);
        await router.push({
            name: Route.ConfigureFormula,
            params: { id: created.id },
            state: {
                name: trimmedName,
                expression: trimmedExpression,
                fields: created.fields,
            },
        });
    } catch (error) {
        errorMessage.value = getProblemMessage(error, t('Errors.UnableToCreateFormula'));
    } finally {
        loading.value = false;
    }
}
</script>

<template>
    <NCard :title="t('Pages.CreateFormula.CardTitle')">
        <NText depth="3">
            {{ t('Pages.CreateFormula.Description') }}
            <code>{{ t('Pages.CreateFormula.Example1') }}</code>
            {{ t('Pages.CreateFormula.DescriptionOr') }}
            <code>{{ t('Pages.CreateFormula.Example2') }}</code>.
        </NText>

        <NAlert v-if="errorMessage" type="error" :class="$cn('alert')">
            {{ errorMessage }}
        </NAlert>

        <NForm :class="$cn('form')" @submit.prevent="handleContinue">
            <NFormItem :label="t('Pages.CreateFormula.FormulaName')">
                <NInput
                    v-model:value="name"
                    :placeholder="t('Pages.CreateFormula.FormulaNamePlaceholder')"
                />
            </NFormItem>

            <NFormItem :label="t('Common.Expression')">
                <NInput
                    v-model:value="expression"
                    type="textarea"
                    :autosize="{ minRows: 4, maxRows: 10 }"
                    :placeholder="t('Pages.CreateFormula.ExpressionPlaceholder')"
                />
            </NFormItem>

            <NSpace>
                <NButton type="primary" attr-type="submit" :loading="loading">
                    {{ t('Common.Continue') }}
                </NButton>
                <NButton @click="router.push({ name: Route.ViewFormulas })">
                    {{ t('Common.Cancel') }}
                </NButton>
            </NSpace>
        </NForm>
    </NCard>
</template>

<style scoped>
.create-formula-page__alert {
    margin-top: 16px;
}

.create-formula-page__form {
    margin-top: 24px;
}
</style>
