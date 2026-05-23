import type { ClassNameModifiers } from 'src/shared/lib/plugins';

declare module 'vue' {
    interface ComponentCustomProperties {
        $cn: {
            (modifiers: ClassNameModifiers): string[];
            (elementName?: string, modifiers?: ModifiersRecord): string[];
        };
    }
}
