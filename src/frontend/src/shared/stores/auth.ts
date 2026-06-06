import { computed, ref } from 'vue';
import { defineStore } from 'pinia';
import { authApi } from '@/shared/api/authApi.ts';
import { formulasApi } from '@/shared/api/formulasApi.ts';
import type { ProblemDetails } from '@/shared/api/httpClient.types.ts';
import { isForbidden } from '@/shared/lib/errors/problemDetails.ts';
import { clearTokens, loadStoredTokens, saveTokens } from '@/shared/lib/auth/tokenStorage.ts';

const REFRESH_THRESHOLD_MS = 60_000;

export const useAuthStore = defineStore('auth', () => {
    const accessToken = ref<string | null>(loadStoredTokens().accessToken);
    const refreshToken = ref<string | null>(loadStoredTokens().refreshToken);
    const expiresAt = ref<number | null>(loadStoredTokens().expiresAt);
    const canManageFormulas = ref<boolean | null>(null);

    const isAuthenticated = computed(() => Boolean(accessToken.value));
    const isAdmin = computed(() => canManageFormulas.value === true);

    function resetAdminAccess() {
        canManageFormulas.value = null;
    }

    async function resolveAdminAccess() {
        if (!accessToken.value) {
            canManageFormulas.value = false;
            return;
        }

        if (canManageFormulas.value !== null) {
            return;
        }

        try {
            await formulasApi.list();
            canManageFormulas.value = true;
        } catch (error) {
            if (isForbidden(error)) {
                canManageFormulas.value = false;
                return;
            }

            throw error;
        }
    }

    function applyTokens(tokenResponse: { accessToken: string; refreshToken: string; expiresIn: number }) {
        accessToken.value = tokenResponse.accessToken;
        refreshToken.value = tokenResponse.refreshToken;
        expiresAt.value = saveTokens(
            tokenResponse.accessToken,
            tokenResponse.refreshToken,
            tokenResponse.expiresIn,
        );
        resetAdminAccess();
    }

    async function login(email: string, password: string) {
        const response = await authApi.login({ email, password });
        applyTokens(response);
        await resolveAdminAccess();
    }

    async function register(email: string, password: string) {
        await authApi.register({ email, password });
        await login(email, password);
    }

    async function refreshIfNeeded() {
        if (!refreshToken.value || !expiresAt.value) {
            return;
        }

        if (Date.now() < expiresAt.value - REFRESH_THRESHOLD_MS) {
            return;
        }

        const response = await authApi.refresh(refreshToken.value);
        applyTokens(response);
        await resolveAdminAccess();
    }

    function logout() {
        accessToken.value = null;
        refreshToken.value = null;
        expiresAt.value = null;
        canManageFormulas.value = null;
        clearTokens();
    }

    function getAccessToken() {
        return accessToken.value;
    }

    function getErrorMessage(error: unknown, fallback: string) {
        const problem = error as ProblemDetails;
        if (problem?.detail) {
            return problem.detail;
        }

        if (problem?.title) {
            return problem.title;
        }

        return fallback;
    }

    return {
        accessToken,
        refreshToken,
        expiresAt,
        canManageFormulas,
        isAuthenticated,
        isAdmin,
        login,
        register,
        refreshIfNeeded,
        resolveAdminAccess,
        logout,
        getAccessToken,
        getErrorMessage,
    };
});
