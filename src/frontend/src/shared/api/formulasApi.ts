import { httpClient } from '@/shared/api/httpClient.ts';
import type {
    CreateFormulaResult,
    EvaluateFormulaResult,
    FieldConfiguration,
    FormulaArguments,
    GetFormulaResult,
    ListFormulasResult,
    ListSubmissionsResult,
} from '@/shared/api/formulas.types.ts';

export const formulasApi = {
    create(name: string, expression: string) {
        return httpClient.post<CreateFormulaResult>('/api/formulas/create', { name, expression });
    },

    configure(id: string, name: string, fields: Record<string, FieldConfiguration>) {
        return httpClient.post<void>(`/api/formulas/${id}/configure`, { name, fields });
    },

    list(search?: string) {
        return httpClient.get<ListFormulasResult>('/api/formulas', search ? { search } : undefined);
    },

    get(id: string) {
        return httpClient.get<GetFormulaResult>(`/api/formulas/${id}`);
    },

    listSubmissions(formulaId: string) {
        return httpClient.get<ListSubmissionsResult>(`/api/formulas/${formulaId}/submissions`);
    },

    evaluate(id: string, arguments_: FormulaArguments) {
        return httpClient.post<EvaluateFormulaResult>(`/api/formulas/${id}/evaluate`, arguments_);
    },

    submit(formulaId: string, evaluationId: string, arguments_: FormulaArguments) {
        return httpClient.postNoContent(
            `/api/formulas/${formulaId}/evaluations/${evaluationId}/submit`,
            arguments_,
        );
    },
};
