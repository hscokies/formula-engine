<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { NAlert, NButton, NCard, NForm, NFormItem, NInput, NSpace, NText } from 'naive-ui';
import { Route } from '@/app/providers/router';
import { formulasApi } from '@/shared/api/formulasApi.ts';
import { getProblemMessage } from '@/shared/lib/errors/problemDetails.ts';

defineOptions({ name: 'CreateFormulaPage' });

const router = useRouter();

const expression = ref('');
const name = ref('');
const loading = ref(false);
const errorMessage = ref<string | null>(null);

async function handleContinue() {
    const trimmedExpression = expression.value.trim();
    const trimmedName = name.value.trim();

    if (!trimmedExpression) {
        errorMessage.value = 'Enter a formula expression.';
        return;
    }

    if (!trimmedName) {
        errorMessage.value = 'Enter a formula name.';
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
        errorMessage.value = getProblemMessage(error, 'Unable to create formula.');
    } finally {
        loading.value = false;
    }
}
</script>

<template>
    <NCard title="Create formula">
        <NText depth="3">
            Enter a mathematical expression and name. Use variable names for fields, for example
            <code>price * quantity</code> or <code>sqrt(a^2 + b^2)</code>.
        </NText>

        <NAlert v-if="errorMessage" type="error" class="create-formula__alert">
            {{ errorMessage }}
        </NAlert>

        <NForm class="create-formula__form" @submit.prevent="handleContinue">
            <NFormItem label="Formula name">
                <NInput v-model:value="name" placeholder="Sales tax calculator" />
            </NFormItem>

            <NFormItem label="Expression">
                <NInput
                    v-model:value="expression"
                    type="textarea"
                    :autosize="{ minRows: 4, maxRows: 10 }"
                    placeholder="price * quantity"
                />
            </NFormItem>

            <NSpace>
                <NButton type="primary" attr-type="submit" :loading="loading">Continue</NButton>
                <NButton @click="router.push({ name: Route.ViewFormulas })">Cancel</NButton>
            </NSpace>
        </NForm>
    </NCard>
</template>

<style scoped>
.create-formula__alert {
    margin-top: 16px;
}

.create-formula__form {
    margin-top: 24px;
}
</style>
