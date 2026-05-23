export default {
    allowEmptyInput: true,
    defaultSeverity: 'warning',
    extends: ['stylelint-config-standard-scss', 'stylelint-config-recommended-vue/scss'],
    rules: {
        'at-rule-empty-line-before': null,
        'declaration-empty-line-before': null,
        'no-descending-specificity': null,
        'selector-class-pattern': null,
        'selector-pseudo-element-no-unknown': [
            true,
            {
                ignorePseudoElements: ['deep'],
            },
        ],

        'scss/dollar-variable-empty-line-before': [
            'always',
            {
                except: ['first-nested', 'after-comment', 'after-dollar-variable'],
                ignore: ['after-comment', 'inside-single-line-block', 'after-dollar-variable'],
            },
        ],
        'scss/dollar-variable-pattern': [
            // __VAR__ and kebab-case
            /^(__[A-Z]+__)$|^(-?[a-z][\da-z]*)(-[\da-z]+)*$/,
            {
                message: 'Expected variable to be kebab-case',
            },
        ],
    },
};
