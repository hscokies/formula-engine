import type { ApiError, ProblemDetails } from '@/shared/api/httpClient.types.ts';

export function getHttpStatus(error: unknown): number | undefined {
    return (error as ApiError)?.status;
}

export function isForbidden(error: unknown): boolean {
    return getHttpStatus(error) === 403;
}

export function getProblemMessage(error: unknown, fallback: string): string {
    const problem = error as ProblemDetails;
    if (problem?.detail) {
        return problem.detail;
    }

    if (problem?.title) {
        return problem.title;
    }

    if (error instanceof Error && error.message) {
        return error.message;
    }

    return fallback;
}
