import { HTTP_METHOD, type HttpRequest } from '@/shared/api/httpClient.types.ts';
import { Route, router } from '@/app/providers/router';
import { ISO8601 } from '@/shared/api/httpClient.constants.ts';

export class HttpClient {
    private relativeUrl(path: string, query?: Record<string, unknown>) {
        if (!query) {
            return path;
        }

        // @ts-ignore
        const searchParams = new URLSearchParams(query);
        return `${path}?${searchParams}`;
    }

    private async parseJson(response: Response) {
        const content = await response.text();
        return JSON.parse(content, (_, value) => {
            if (typeof value === 'string' && ISO8601.test(value)) {
                return new Date(value);
            }

            return value;
        });
    }

    public async sendRequest(request: HttpRequest) {
        const response = await fetch(request.path, request.options);
        if (response.status === 401) {
            await router.replace({ name: Route.Login });
            throw new Error('Redirecting...');
        }

        return response;
    }

    public async get<TResult>(path: string, query?: Record<string, unknown>): Promise<TResult> {
        const response = await this.sendRequest({
            path: this.relativeUrl(path, query),
            options: {
                method: HTTP_METHOD.GET,
            },
        });

        const data = await this.parseJson(response);
        if (!response.ok) {
            throw data;
        }

        return data as TResult;
    }
}
