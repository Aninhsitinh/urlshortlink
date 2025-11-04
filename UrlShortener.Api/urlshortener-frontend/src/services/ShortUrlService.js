import axios from "../api/axios";

// ✅ Tạo URL rút gọn
export async function shortenUrl(originalUrl) {
    try {
        const res = await axios.post("/shorturl", { originalUrl });

        // Backend trả:
        // {
        //   shortUrl: "...",
        //   originalUrl: "...",
        //   createdAt: "..."
        // }

        return res.data;
    } catch (err) {
        console.error("❌ Shorten failed:", err.response?.data || err.message);
        alert(
            "Failed to shorten URL: " +
            (err.response?.data?.message ||
                err.response?.data ||
                err.message)
        );
        throw err;
    }
}

// ✅ Lấy danh sách URL của user
export async function getMyUrls() {
    try {
        const res = await axios.get("/shorturl");
        return res.data;
    } catch (err) {
        console.error("❌ Get URLs failed:", err.response?.data || err.message);
        alert(
            "Failed to load your URLs: " +
            (err.response?.data?.message ||
                err.response?.data ||
                err.message)
        );
        throw err;
    }
}
