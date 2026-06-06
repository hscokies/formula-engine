import type { Router } from 'vue-router';
import { Route } from '@/app/providers/router';

export function getFormulaUrl(router: Router, formulaId: string) {
    const path = router.resolve({ name: Route.Formula, params: { id: formulaId } }).href;
    return new URL(path, window.location.origin).toString();
}

export async function copyFormulaLink(router: Router, formulaId: string) {
    const url = getFormulaUrl(router, formulaId);
    await navigator.clipboard.writeText(url);
    return url;
}
