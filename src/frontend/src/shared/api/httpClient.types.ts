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
    traceId: string;
    type: URL;
    title: string;
    status: number;
    detail: string;
    errors: Record<string, Error[]>;
}

interface Error {
    code: string;
    description: string;
}
