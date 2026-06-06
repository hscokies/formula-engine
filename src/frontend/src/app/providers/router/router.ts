import { createRouter, createWebHistory } from 'vue-router';
import { Route } from '@/app/providers/router/router.types.ts';
import {
    ConfigureFormulaPage,
    CreateFormulaPage,
    FormulaPage,
    LoginPage,
    RegisterPage,
    ViewFormulasPage,
    CalculationsPage,
} from '@/pages';
import { i18n } from '@/app/providers/i18n';
import { useAuthStore } from '@/shared/stores/auth.ts';

const publicRoutes = new Set<Route>([Route.Login, Route.Register]);

export const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        {
            name: Route.Login,
            path: '/login',
            component: LoginPage,
            meta: {
                title: i18n.global.t('Pages.Login.Title'),
                guestOnly: true,
            },
        },
        {
            name: Route.Register,
            path: '/register',
            component: RegisterPage,
            meta: {
                title: i18n.global.t('Pages.Register.Title'),
                guestOnly: true,
            },
        },
        {
            name: Route.ViewFormulas,
            path: '/formulas',
            component: ViewFormulasPage,
            meta: {
                title: i18n.global.t('Pages.ViewFormulas.Title'),
                requiresAuth: true,
            },
        },
        {
            name: Route.CreateFormula,
            path: '/formulas/create',
            component: CreateFormulaPage,
            meta: {
                title: i18n.global.t('Pages.CreateFormula.Title'),
                requiresAuth: true,
                requiresAdmin: true,
            },
        },
        {
            name: Route.ConfigureFormula,
            path: '/formulas/:id/configure',
            component: ConfigureFormulaPage,
            meta: {
                title: i18n.global.t('Pages.ConfigureFormula.Title'),
                requiresAuth: true,
                requiresAdmin: true,
            },
        },
        {
            name: Route.ViewCalculations,
            path: '/formulas/:id/calculations',
            component: CalculationsPage,
            meta: {
                title: i18n.global.t('Pages.ViewCalculations.Title'),
                requiresAuth: true,
            },
        },
        {
            name: Route.Formula,
            path: '/formulas/:id',
            component: FormulaPage,
            meta: {
                title: i18n.global.t('Pages.Formula.Title'),
                requiresAuth: true,
            },
        },
        {
            path: '/',
            redirect: {
                name: Route.ViewFormulas,
            },
        },
        {
            path: '/:pathMatch(.*)*',
            redirect: {
                name: Route.ViewFormulas,
            },
        },
    ],
});

router.beforeEach(async (to, _, next) => {
    document.title = to.meta.title ?? i18n.global.t('Common.ProjectName');

    const authStore = useAuthStore();
    await authStore.refreshIfNeeded();

    const routeName = to.name as Route;
    const isPublicRoute = publicRoutes.has(routeName);

    if (to.meta.guestOnly && authStore.isAuthenticated) {
        next({ name: Route.ViewFormulas });
        return;
    }

    if (!isPublicRoute && to.meta.requiresAuth && !authStore.isAuthenticated) {
        next({ name: Route.Login, query: { redirect: to.fullPath } });
        return;
    }

    if (!isPublicRoute && authStore.isAuthenticated) {
        await authStore.resolveAdminAccess();
    }

    if (to.meta.requiresAdmin && !authStore.isAdmin) {
        next({ name: Route.ViewFormulas });
        return;
    }

    next();
});
