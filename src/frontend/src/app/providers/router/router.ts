import { createRouter, createWebHistory } from 'vue-router';
import { Route } from '@/app/providers/router/router.types.ts';
import {
    CreateFormulaPage,
    FormulaPage,
    LoginPage,
    RegisterPage,
    ViewFormulasPage,
    CalculationsPage,
    ConfigureFormula,
} from '@/pages';
import { i18n } from '@/app/providers/i18n';

export const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        {
            name: Route.Login,
            path: '/login',
            component: LoginPage,
            meta: {
                title: i18n.global.t('Pages.Login.Title'),
            },
        },
        {
            name: Route.Register,
            path: '/register',
            component: RegisterPage,
            meta: {
                title: i18n.global.t('Pages.Register.Title'),
            },
        },
        {
            name: Route.ViewFormulas,
            path: '/formulas',
            component: ViewFormulasPage,
            meta: {
                title: i18n.global.t('Pages.ViewFormulas.Title'),
            },
        },
        {
            name: Route.CreateFormula,
            path: '/formulas/create',
            component: CreateFormulaPage,
            meta: {
                title: i18n.global.t('Pages.CreateFormula.Title'),
            },
        },
        {
            name: Route.ConfigureFormula,
            path: '/formulas/configure',
            component: ConfigureFormula,
            meta: {
                title: i18n.global.t('Pages.ConfigureFormula.Title'),
            },
        },
        {
            name: Route.Formula,
            path: '/formulas/:id',
            component: FormulaPage,
            meta: {
                title: i18n.global.t('Pages.Formula.Title'),
            },
        },
        {
            name: Route.ViewCalculations,
            path: '/formulas/:id/calculations',
            component: CalculationsPage,
            meta: {
                title: i18n.global.t('Pages.ViewCalculations.Title'),
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

router.beforeEach((to, _, next) => {
    document.title = to.meta.title ?? i18n.global.t('Common.ProjectName');
    next();
});
