import { createI18n } from 'vue-i18n';
import { getFileNameWithoutExtension } from '@/shared/lib/utils/file.ts';
import { Locale } from './i18n.types.ts';

function loadLocalizationFiles(files: Record<string, unknown>) {
    return Object.fromEntries(
        Object.entries(files).map(([path, contents]) => [getFileNameWithoutExtension(path), contents])
    );
}

export const i18n = createI18n({
    legacy: false,
    locale: Locale.en,
    fallbackLocale: Locale.en,
    messages: loadLocalizationFiles(import.meta.glob('@/shared/i18n/*.json', { eager: true, import: 'default' })),
});

console.log(
    'i18n messages:',
    JSON.stringify(loadLocalizationFiles(import.meta.glob('@/shared/i18n/*.json', { eager: true, import: 'default' })))
);
