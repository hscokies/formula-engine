export function getFileNameWithoutExtension(fullPath?: string) {
    return fullPath
        ?.split(/[/\\]/)
        .pop()
        ?.replace(/\.\w+$/, '');
}
