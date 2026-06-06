import {
    HTTP_METHOD,
    CONTENT_TYPE,
    type ApiError,
    type HttpRequest,
} from '@/shared/api/httpClient.types.ts';
import { Route, router } from '@/app/providers/router';
import { ISO8601 } from '@/shared/api/httpClient.constants.ts';

type AccessTokenGetter = () => string | null;

let accessTokenGetter: AccessTokenGetter = () => null;

export function setAccessTokenGetter(getter: AccessTokenGetter) {
    accessTokenGetter = getter;
}

export class HttpClient {
    private relativeUrl(path: string, query?: Record<string, unknown>) {
        if (!query) {
            return path;
        }

        const searchParams = new URLSearchParams();
        for (const [key, value] of Object.entries(query)) {
            if (value !== undefined && value !== null) {
                searchParams.set(key, String(value));
            }
        }

        const queryString = searchParams.toString();
        return queryString ? `${path}?${queryString}` : path;
    }

    private buildHeaders(hasBody: boolean): HeadersInit {
        const headers: Record<string, string> = {};
        const token = accessTokenGetter();

        if (token) {
            headers.Authorization = `Bearer ${token}`;
        }

        if (hasBody) {
            headers['Content-Type'] = CONTENT_TYPE.JSON;
        }

        return headers;
    }

    private async parseJson(response: Response) {
        const content = await response.text();
        if (!content) {
            return null;
        }

        return JSON.parse(content, (_, value) => {
            if (typeof value === 'string' && ISO8601.test(value)) {
                return new Date(value);
            }

            return value;
        });
    }

    public async sendRequest(request: HttpRequest) {
        const response = await fetch(request.path, request.options);
        if (response.status === 401 && router.currentRoute.value.name !== Route.Login) {
            await router.replace({ name: Route.Login });
            throw new Error('Redirecting...');
        }

        return response;
    }

    private async request<TResult>(
        method: HTTP_METHOD,
        path: string,
        options?: { query?: Record<string, unknown>; body?: unknown },
    ): Promise<TResult> {
        const hasBody = options?.body !== undefined;
        const response = await this.sendRequest({
            path: this.relativeUrl(path, options?.query),
            options: {
                method,
                headers: this.buildHeaders(hasBody),
                body: hasBody ? JSON.stringify(options.body) : undefined,
            },
        });

        const data = await this.parseJson(response);
        if (!response.ok) {
            throw { ...(data ?? {}), status: response.status } as ApiError;
        }

        return data as TResult;
    }

    public get<TResult>(path: string, query?: Record<string, unknown>): Promise<TResult> {
        return this.request(HTTP_METHOD.GET, path, { query });
    }

    public post<TResult>(path: string, body?: unknown, query?: Record<string, unknown>): Promise<TResult> {
        return this.request(HTTP_METHOD.POST, path, { body, query });
    }

    public async postNoContent(path: string, body?: unknown): Promise<void> {
        const response = await this.sendRequest({
            path,
            options: {
                method: HTTP_METHOD.POST,
                headers: this.buildHeaders(body !== undefined),
                body: body !== undefined ? JSON.stringify(body) : undefined,
            },
        });

        if (!response.ok) {
            const data = await this.parseJson(response);
            throw { ...(data ?? {}), status: response.status } as ApiError;
        }
    }
}

export const httpClient = new HttpClient();
