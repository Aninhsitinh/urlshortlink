// axios.js
import axios from "axios";

const instance = axios.create({
    baseURL: "https://localhost:7176/api", // ✅ CHUẨN VỚI BACKEND ASP.NET
    withCredentials: false, // ✅ Không dùng cookie nên để false
});

// ✅ Tự động thêm JWT vào header
instance.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem("token");
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

export default instance;
