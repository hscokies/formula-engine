import type { ClassNameModifiers } from '@/app/plugins/classNamePlugin.ts';

declare module 'vue' {
    interface ComponentCustomProperties {
        $cn: {
            (modifiers: ClassNameModifiers): string[];
            (elementName?: string, modifiers?: ClassNameModifiers): string[];
        };
    }
}
