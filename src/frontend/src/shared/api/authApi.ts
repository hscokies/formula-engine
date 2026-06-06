import { httpClient } from '@/shared/api/httpClient.ts';
import type { LoginRequest, RegisterRequest, TokenResponse } from '@/shared/api/auth.types.ts';

export const authApi = {
    login(request: LoginRequest) {
        return httpClient.post<TokenResponse>('/api/users/login', request, { useCookies: false });
    },

    register(request: RegisterRequest) {
        return httpClient.post<void>('/api/users/register', request);
    },

    refresh(refreshToken: string) {
        return httpClient.post<TokenResponse>('/api/users/refresh', { refreshToken });
    },
};
