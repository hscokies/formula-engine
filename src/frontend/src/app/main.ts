import { createApp } from 'vue';
import App from '@/app/ui/app.vue';
import { i18n } from '@/app/providers/i18n';
import { router } from '@/app/providers/router';
import { classNamePlugin } from '@/app/plugins';

createApp(App).use(i18n).use(classNamePlugin).use(router).mount('#app');
