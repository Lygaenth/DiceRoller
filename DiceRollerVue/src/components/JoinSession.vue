<script lang="ts" setup>
    import { ref } from 'vue'

    const userName = ref('');
    const partyId = ref('');
    const partyPassword = ref('');

    function OnJoinSession() {
        alert("attempting to join session " + partyId.value + " as " + userName.value + "(" + partyPassword.value + ")");
    }

</script>

<template>
    <div class="rowHeader">
        <p></p>
    </div>
    <div style="float:left; width:40%;background:ghostwhite; height:500px; padding-top:100px;">
        <div class="alignedDiv">
            <div class="row">
                <div class="rowHeader">
                    <label class="rowLabel">User:</label>
                </div>
                <div class="rowValue">
                    <input type="text" v-model="userName" placeholder="user name" />
                </div>
            </div>
        </div>
        <div class="alignedDiv">
            <div class="row">
                <div class="rowHeader">
                    <label class="rowLabel">Party ID:</label>
                </div>
                <div class="rowValue">
                    <input type="text" v-model="partyId" placeholder="party id" />
                </div>
            </div>
        </div>
        <div class="alignedDiv">
            <div class="row">
                <div class="rowHeader">
                    <label class="rowLabel">User:</label>
                </div>
                <div class="rowValue">
                    <input type="text" v-model="partyPassword" placeholder="password" />
                </div>
            </div>
        </div>
        <div class="alignedDiv">
            <button class="centeredButton" @click="OnJoinSession">Join session</button>
        </div>
        <div class="rowHeader">
            <p></p>
        </div>
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