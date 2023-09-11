<script lang="ts" setup>
    import { ref } from 'vue';

    const partyName = ref('');
    const partyPassword = ref('');

    function OnCreateSession() {
        window.location.href = "party/" + 1 + "/gm";
    }

</script>

<template>
    <div style="float: left; width: 30%;">
        <p></p>
    </div>
    <div style="float:left; width:40%;background:ghostwhite; height:500px; padding-top:100px;">
        <div class="alignedDiv">
            <div class="row">
                <div class="rowHeader">
                    <label class="rowLabel">Party:</label>
                </div>
                <div class="rowValue">
                    <input type="text" v-model="partyName" placeholder="party name" />
                </div>
            </div>
        </div>
        <div class="alignedDiv">
            <div class="row">
                <div class="rowHeader">
                    <label class="rowLabel">Password:</label>
                </div>
                <div class="rowValue">
                    <input type="text" id="partyPasswordInput" v-model="partyPassword" placeholder="password" />
                </div>
            </div>
        </div>
        <div style="float:none;">
            <div style="display: flex; justify-content: center; align-items: center;">
                <button class="centeredButton" id="CreateSessionButton" @click="OnCreateSession">Create session</button>
            </div>
        </div>
    </div>
    <div style="float: left; width: 30%;">
        <p></p>
    </div>
</template>

<style lang="css">
    .alignedDiv {
        display: flex;
        justify-content: center;
        align-items: center;
    }

    .row {
        width: 250px;
        height: 40px
    }

    .rowHeader {
        float: left;
        width: 30%
    }

    .rowValue {
        float: left;
        width: 70%
    }

    .centeredButton {
        display: flex;
        justify-content: center;
        align-items: center;
        left: 50%;
        height: 30px;
        margin: 5px;
        background-color: white;
    }

    .rowLabel {
        float: right;
        margin-right: 5px
    }
</style>