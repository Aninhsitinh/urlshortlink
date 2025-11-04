import { defineStore } from "pinia";
import axios from "../api/axios";

export const useAuthStore = defineStore("auth", {
    state: () => ({
        user: null,
        token: localStorage.getItem("token") || null,
        refreshToken: localStorage.getItem("refreshToken") || null,
    }),

    actions: {
        // ✅ REGISTER
        async register(email, password) {
            try {
                const res = await axios.post("/auth/register", {
                    email,
                    password,
                });

                if (!res.data.success) {
                    throw new Error(res.data.message || "Register failed");
                }

                this.token = res.data.accessToken;
                this.refreshToken = res.data.refreshToken;

                localStorage.setItem("token", this.token);
                localStorage.setItem("refreshToken", this.refreshToken);

                this.user = { email };

                console.log("✅ Register success:", res.data);
                return true;
            } catch (err) {
                console.error("❌ Register error:", err.response?.data || err.message);

                alert(
                    "Register failed: " +
                    (err.response?.data?.message ||
                        JSON.stringify(err.response?.data) ||
                        err.message)
                );

                throw err;
            }
        },

        // ✅ LOGIN
        async login(email, password) {
            try {
                const res = await axios.post("/auth/login", {
                    email,
                    password,
                });

                if (!res.data.success) {
                    throw new Error(res.data.message || "Login failed");
                }

                this.token = res.data.accessToken;
                this.refreshToken = res.data.refreshToken;

                localStorage.setItem("token", this.token);
                localStorage.setItem("refreshToken", this.refreshToken);

                this.user = { email };

                console.log("✅ Login success:", res.data);
                return true;
            } catch (err) {
                console.error("❌ Login error:", err.response?.data || err.message);

                alert(
                    "Login failed: " +
                    (err.response?.data?.message ||
                        JSON.stringify(err.response?.data) ||
                        err.message)
                );

                throw err;
            }
        },

        // ✅ LOGOUT
        logout() {
            this.token = null;
            this.refreshToken = null;
            this.user = null;

            localStorage.removeItem("token");
            localStorage.removeItem("refreshToken");

            console.log("✅ Logged out");
        },
    },
});
