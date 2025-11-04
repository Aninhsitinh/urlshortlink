import { defineStore } from "pinia";
import axios from "../api/axios";

export const useAuthStore = defineStore("auth", {
    state: () => ({
        user: null,
        token: localStorage.getItem("token") || null,
    }),

    actions: {
        async register(email, password) {
            try {
                const res = await axios.post("/auth/register", { email, password });
                this.token = res.data.accessToken;
                localStorage.setItem("token", this.token);
                this.user = { email };
                return true;
            } catch (err) {
                console.error("Register error:", err);
                throw err;
            }
        },

        async login(email, password) {
            try {
                const res = await axios.post("/auth/login", { email, password });
                this.token = res.data.accessToken;
                localStorage.setItem("token", this.token);
                this.user = { email };
                return true;
            } catch (err) {
                console.error("Login error:", err);
                throw err;
            }
        },

        logout() {
            this.token = null;
            this.user = null;
            localStorage.removeItem("token");
        }
    }
});
