import { createApp } from 'vue';
import { createPinia } from 'pinia';
import App from '@/app/ui/app.vue';
import { i18n } from '@/app/providers/i18n';
import { router } from '@/app/providers/router';
import { classNamePlugin } from '@/app/plugins';
import { setAccessTokenGetter } from '@/shared/api/httpClient.ts';
import { useAuthStore } from '@/shared/stores/auth.ts';

const app = createApp(App);
const pinia = createPinia();

app.use(pinia);
app.use(i18n);
app.use(classNamePlugin);
app.use(router);

const authStore = useAuthStore(pinia);
setAccessTokenGetter(() => authStore.getAccessToken());

app.mount('#app');
