export enum HTTP_METHOD {
    GET = 'GET',
    POST = 'POST',
    PUT = 'PUT',
    PATCH = 'PATCH',
    DELETE = 'DELETE',
}

export enum CONTENT_TYPE {
    JSON = 'application/json',
}

export interface HttpRequest {
    path: string;
    options: RequestInit;
}

export interface ProblemDetails {
    traceId?: string;
    type?: string;
    title?: string;
    status?: number;
    detail?: string;
    errors?: Record<string, Error[]>;
}

export interface ApiError extends ProblemDetails {
    status: number;
}

interface Error {
    code: string;
    description: string;
}
