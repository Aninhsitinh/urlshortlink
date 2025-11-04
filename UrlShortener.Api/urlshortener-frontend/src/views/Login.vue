<template>
    <div class="form-container">
        <h2>Login</h2>
        <button class="theme-btn" @click="toggleTheme">
            {{ theme === 'theme-light' ?  "🌙" : "☀️" }}
        </button>


        <input v-model="email"
               type="email"
               placeholder="Email" />

        <input v-model="password"
               type="password"
               placeholder="Password" />

        <button @click="onLogin">Login</button>

        <p class="muted">
            Don't have an account?
            <router-link to="/register">Register</router-link>
        </p>
    </div>
</template>

<script setup>
    import { ref } from "vue";
    import { useRouter } from "vue-router";
    import { useAuthStore } from "../store/authStore";

    const email = ref("");
    const password = ref("");
    const router = useRouter();
    const auth = useAuthStore();
    const theme = ref(localStorage.getItem("theme") || "theme-light");

    async function onLogin() {
        if (!email.value || !password.value) {
            alert("Please fill in both fields.");
            return;
        }

        try {
            console.log("Trying to login:", email.value);
            await auth.login(email.value, password.value);

            alert("Login successful!");

            console.log("User:", auth.user);

            router.push("/dashboard");
        } catch (err) {
            console.error("Login failed:", err.response?.data || err.message);

            alert(
                "Login failed: " +
                (err.response?.data?.message ||
                    JSON.stringify(err.response?.data) ||
                    err.message)
            );
        }
    }
    function toggleTheme() {
        document.body.classList.toggle("theme-dark");

        const btn = document.querySelector(".theme-btn");
        btn.textContent = document.body.classList.contains("theme-dark") ? "🌙" : "☀️";
    }

</script>

<style>
    :root {
    --bg-color: #f5f7fa;
    --card-color: rgba(255, 255, 255, 0.42);
    --text-color: #1d1d1d;
    --input-bg: rgba(255, 255, 255, 0.55);
    --input-border: rgba(150, 150, 150, 0.35);
    --button-bg: #42b883;
    --button-hover: #2a9d8f;
    --muted-text: #555;
    --link-color: #2c7df0;
}

body.theme-dark {
    --bg-color: #000000;
    --card-color: #111111;
    --text-color: #f0f0f0;
    --input-bg: #1a1a1a;
    --input-border: #333;
    --button-bg: #4ecf9c;
    --button-hover: #36b783;
    --muted-text: #e9e9e9;
    --link-color: #7ddfff;
}

body {
    background: var(--bg-color);
    background: var(--bg-color);
    min-height: 100vh;
    display: flex;
    align-items: center;
    justify-content: center;
    transition: background 0.4s ease;
}

.muted {
    color: var(--muted-text);
    font-size: 14px;
    text-align: center;
}

.theme-btn {
    position: absolute;
    top: 14px;
    right: 14px;
    width: 38px;
    height: 38px;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 20px;
    background: rgba(255,255,255,0.22);
    border: 1px solid var(--input-border);
    border-radius: 12px;
    cursor: pointer;
    backdrop-filter: blur(12px);
    transition: 0.25s ease;
    color: var(--text-color);
}

.theme-btn:hover {
    background: rgba(255,255,255,0.32);
    transform: translateY(-2px);
}

.form-container {
    width: 360px;
    padding: 32px;
    border-radius: 22px;
    background: var(--card-color);
    backdrop-filter: blur(18px);
    border: 1px solid rgba(255,255,255,0.28);
    box-shadow: 0 12px 34px rgba(0,0,0,0.4);
    display: flex;
    flex-direction: column;
    gap: 18px;
    animation: fadeIn 0.6s ease;
    transition: 0.35s ease;
}

.form-container h2 {
    text-align: center;
    color: var(--text-color);
    font-weight: 600;
    font-size: 26px;
}

input {
    padding: 12px 14px;
    font-size: 15px;
    border: 1px solid var(--input-border);
    border-radius: 14px;
    background: var(--input-bg);
    color: var(--text-color);
    transition: 0.25s;
}

input:hover {
    background: rgba(255,255,255,0.65);
}

input:focus {
    border-color: #42b883;
    outline: none;
    transform: translateY(-2px);
    box-shadow: 0 0 0 3px rgba(66,184,131,0.25);
}

button {
    padding: 12px;
    border-radius: 14px;
    border: none;
    font-size: 15px;
    font-weight: 600;
    cursor: pointer;
    background-color: var(--button-bg);
    color: white;
    transition: 0.25s ease;
}

button:hover {
    background-color: var(--button-hover);
    transform: translateY(-2px);
}

a {
    color: var(--link-color);
    font-weight: 600;
}

a:hover {
    text-decoration: underline;
}

@keyframes fadeIn {
    from { opacity: 0; transform: translateY(10px); }
    to { opacity: 1; transform: translateY(0); }
}

</style>
