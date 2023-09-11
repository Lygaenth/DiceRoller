<template>
    <footer>
        <p>Copyright &copy; {{year}} </p>
    </footer>
</template>

<script lang="ts">
    import { computed } from 'vue';

    export default {
        setup() {
            const year = computed(() => new Date().getFullYear())
            return { year };
        }
    };

</script>