import { type App, getCurrentInstance } from 'vue';
import { getFileNameWithoutExtension } from '@/shared/lib/utils/file.ts';
import kebabCase from 'lodash-es/kebabCase';

export type ClassNameModifiers = Record<string, string | number | boolean>;

function className(blockName: string, modifiers: ClassNameModifiers): string[];
function className(blockName: string, elementName?: string, modifiers?: ClassNameModifiers): string[];
function className(blockName: string, ...args: unknown[]): string[];

function className(blockName: string, ...args: unknown[]): string[] {
    blockName = kebabCase(blockName);

    const [arg1, arg2] = args;
    if (!arg1) {
        return [blockName];
    }

    let elementName: string | undefined;
    let modifiers: ClassNameModifiers | undefined;

    if (typeof arg1 === 'string') {
        // className(blockName: string, elementName: string | undefined, modifiers: ClassNameModifiers)
        elementName = kebabCase(arg1);
        modifiers = arg2 as ClassNameModifiers;
    } else {
        // className(blockName: string, modifiers: ClassNameModifiers)
        modifiers = arg1 as ClassNameModifiers;
    }

    const classNames: string[] = [];

    const baseName = elementName ? `${blockName}__${elementName}` : blockName;
    classNames.push(baseName);

    if (modifiers) {
        for (const [key, value] of Object.entries(modifiers)) {
            let className = baseName;
            const modifierName = kebabCase(key);

            if (typeof value === 'boolean') {
                if (!value) continue;
                className += `--${modifierName}`;
            } else {
                const formattedValue = typeof value === 'string' ? kebabCase(value) : value;
                className += `--${modifierName}-${formattedValue}`;
            }

            classNames.push(className);
        }
    }

    return classNames;
}

function getComponentName() {
    const fallbackComponentName = 'default-component';

    const instance = getCurrentInstance();
    if (!instance) {
        return fallbackComponentName;
    }

    const name = instance.type.name;
    if (!name?.length) {
        throw new Error(`Unable to determine component name for: ${getFileNameWithoutExtension(instance.type.__file)}`);
    }

    return kebabCase(name);
}

export const classNamePlugin = {
    install(app: App) {
        app.config.globalProperties.$cn = (...args: unknown[]) => {
            return className(getComponentName(), ...args);
        };
    },
};
