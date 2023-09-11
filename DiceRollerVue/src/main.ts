import { createApp } from 'vue';
import App from './App.vue';
import { createRouter, createWebHistory } from 'vue-router'

import AppHome from './components/AppHome.vue';
import PartyPlayer from './components/PartyPlayer.vue';


const Home = { template: '<app-home/>' };
const PartyPage = { template: '<party-page/>' };
const About = { template: '<div><p>ABOUT</p></div>'}

const routes = [
    { path: '/', component: AppHome },
    { path: '/party', component: PartyPage },
    { path: '/about', component: About }
]

export const router = createRouter({
    history: createWebHistory(),
    routes // short for `routes: routes`
})

const app = createApp(App);

app.use(router);

app.mount('#app')
