
<script lang="ts" setup>
    import { ref } from 'vue'
    import CreateSession from './CreateSession.vue'
    import JoinSession from './JoinSession.vue'

    function SwitchToJoin() {
        ShowCreateDiv(false);
    }

    function SwitchToCreate() {
        ShowCreateDiv(true);
    }

    function ShowCreateDiv(show: boolean) {
        (document.getElementById("divCreateSession") as HTMLDivElement).style.visibility = show ? "visible" : "hidden";
        (document.getElementById("divCreateSession") as HTMLDivElement).style.display = show ? "block" : "none";
        (document.getElementById("divJoinSession") as HTMLDivElement).style.visibility = show ? "hidden" : "visible";
        (document.getElementById("divJoinSession") as HTMLDivElement).style.display = show ? "none" : "block";
    }

</script>

<template>
<div style="background: ghostwhite;">
    <div>
        <div style="display: flex; justify-content: center; align-items: center;">
            <button class="centeredButton" id="btnCreateSession" @click="SwitchToCreate">Create session</button>
            <button class="centeredButton" id="btnJoinsession" @click="SwitchToJoin">Join session</button>
        </div>
    </div>
</div>
<div class="hiddenDiv" id="divCreateSession">
    <create-session/>
</div>
<div class="hiddenDiv" id="divJoinSession">
    <join-session/>
</div>
</template>



<style scoped>
    .hiddenDiv {
        visibility: hidden;
        display: none;
    }

    .centeredButton {
        display: flex;
        justify-content: center;
        align-items: center;
        left: 50%;
        height:30px;
        margin:5px;
        background-color:white;
    }
</style>
