<template>
    <div class="dashboard">
        <h2>Dashboard</h2>

        <div class="input-group">
            <input v-model="url" placeholder="Enter a URL to shorten" />
            <button @click="shortenUrl">Shorten</button>
        </div>

        <div v-if="links.length > 0" class="links">
            <h3>Your shortened links:</h3>
            <ul>
                <li v-for="(item, index) in links" :key="index">
                    <a :href="item.originalUrl" target="_blank">
                        {{ item.originalUrl }}
                    </a>
                    →
                    <a :href="`${backendBase}/r/${item.code}`" target="_blank">
                        {{ `${backendHost}/r/${item.code}` }}
                    </a>
                </li>
            </ul>
        </div>

        <div v-else>
            <p>No links yet. Try shortening one!</p>
        </div>

        <button class="logout" @click="logout">Logout</button>
    </div>
</template>

<script setup>
    import { ref, onMounted } from "vue";
    import api from "../api/axios";            // ✅ dùng axios instance đã config
    import { useAuthStore } from "../store/authStore";
    import { useRouter } from "vue-router";

    const url = ref("");
    const links = ref([]);
    const auth = useAuthStore();
    const router = useRouter();

    // ✅ Backend URL (dùng cổng backend thật của bạn)
    const backendBase = "https://localhost:7176";
    const backendHost = "localhost:7176";

    // ✅ Rút gọn URL
    async function shortenUrl() {
        if (!url.value.trim()) {
            alert("Please enter a valid URL!");
            return;
        }

        try {
            const res = await api.post("/shorturl", { originalUrl: url.value });

            console.log("Shortened:", res.data);

            await loadLinks();
            url.value = "";
        } catch (err) {
            console.error("Shorten failed:", err.response?.data || err.message);
            alert(
                "Failed to shorten URL: " +
                (err.response?.data?.message ||
                    JSON.stringify(err.response?.data) ||
                    err.message)
            );
        }
    }

    // ✅ Tải danh sách URL
    async function loadLinks() {
        try {
            const res = await api.get("/shorturl");
            console.log("Links from backend:", res.data);

            links.value = res.data;
        } catch (err) {
            console.error("Load links failed:", err.response?.data || err.message);
        }
    }

    // ✅ Đăng xuất
    function logout() {
        auth.logout();
        router.push("/login");
    }

    onMounted(loadLinks);
</script>

<style>

    :root {
        --bg-color: #f5f7fa;
        --card-color: rgba(255, 255, 255, 0.18);
        --text-color: #1d1d1d;
        --input-bg: rgba(255, 255, 255, 0.5);
        --input-border: rgba(0,0,0,0.15);
        --button-bg: #42b883;
        --button-hover: #2a9d8f;
       
        --link-color: #FF4ECD;
    }

    body.theme-dark {
        --bg-color: #0e0e0e;
        --card-color: rgba(30, 30, 30, 0.35);
        --text-color: #e5e5e5;
        --input-bg: rgba(0,0,0,0.45);
        --input-border: rgba(255,255,255,0.15);
        --button-bg: #5acba2;
        --button-hover: #3fab88;
        --link-color: #0047ab;
    }

   

    .dashboard {
        width: 700px;
        margin: 60px auto;
        padding: 35px;
        border-radius: 22px;
        background: rgba(255, 255, 255, 0.18);
        backdrop-filter: blur(22px);
        -webkit-backdrop-filter: blur(22px);
        border: 1px solid rgba(255, 255, 255, 0.25);
        box-shadow: 0 12px 35px rgba(0,0,0,0.20);
        color: var(--text-color);
        display: flex;
        flex-direction: column;
        gap: 25px;
        transition: 0.3s ease;
    }

        .dashboard h2 {
            color: var(--text-color);
            font-size: 28px;
            text-align: center;
            font-weight: 700;
        }

   

    .input-group {
        display: flex;
        gap: 12px;
    }

    input {
        flex: 1;
        padding: 14px 16px;
        border-radius: 14px;
        border: 1px solid var(--input-border);
        background: var(--input-bg);
        color: var(--text-color);
        font-size: 15px;
        transition: 0.25s;
    }

        input:focus {
            outline: none;
            border-color: #42b883;
            box-shadow: 0 0 0 3px rgba(66,184,131,0.25);
            transform: translateY(-2px);
        }

    /* ============================= */
    /* ✅ BUTTON STYLE               */
    /* ============================= */

    button {
        padding: 14px 16px;
        border-radius: 14px;
        border: none;
        font-size: 15px;
        font-weight: 600;
        cursor: pointer;
        background-color: var(--button-bg);
        color: white;
        transition: 0.25s;
    }

        button:hover {
            background-color: var(--button-hover);
            transform: translateY(-3px);
        }

    .logout {
        background-color: #ff6b6b;
    }

        .logout:hover {
            background-color: #ff3a3a;
        }

    /* ============================= */
    /* ✅ LINKS – TABLE CARD STYLE   */
    /* ============================= */

    .links {
        margin-top: 20px;
    }

        .links h3 {
            font-size: 20px;
            font-weight: 600;
            color: var(--text-color);
            margin-bottom: 14px;
        }

    ul {
        padding: 0;
        list-style: none;
    }

    li {
        margin-bottom: 12px;
        padding: 15px 18px;
        border-radius: 16px;
        background: rgba(255,255,255,0.25);
        border: 1px solid var(--input-border);
        backdrop-filter: blur(15px);
        display: flex;
        flex-direction: column;
        gap: 6px;
        transition: 0.25s ease;
    }

        li:hover {
            transform: translateY(-4px);
            box-shadow: 0 6px 20px rgba(0,0,0,0.18);
        }

   

    a {
        text-decoration: none;
        font-weight: 600;
        color: var(--link-color);
        word-break: break-all;
        transition: color 0.25s ease;
    }

        a:hover {
            text-decoration: underline;
            color: var(--button-hover);
        }

    

    .dashboard p {
        text-align: center;
        font-size: 16px;
        color: var(--text-color);
    }

   
    @keyframes spin {
        to {
            transform: rotate(360deg);
        }
    }


</style>
