export type FieldType = 'Integer' | 'Decimal';

export interface FieldConfiguration {
    label: string;
    type: FieldType;
}

export interface CreateFormulaResult {
    id: string;
    fields: string[];
}

export interface GetFormulaResult {
    id: string;
    name: string;
    fields: Record<string, FieldConfiguration>;
}

export interface ListFormulasResult {
    items: Array<{ id: string; name: string }>;
}

export interface EvaluateFormulaResult {
    key: string;
    result: number;
}

export interface SubmissionItem {
    id: string;
    result: number;
    userId: string;
}

export interface ListSubmissionsResult {
    items: SubmissionItem[];
}

export interface IntegerValue {
    $type: 'Integer';
    value: number;
}

export interface DecimalValue {
    $type: 'Decimal';
    value: number;
}

export type FormulaValue = IntegerValue | DecimalValue;

export type FormulaArguments = Record<string, FormulaValue>;
