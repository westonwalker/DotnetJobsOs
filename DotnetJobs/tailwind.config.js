module.exports = {
    content: ["Pages/**/*.cshtml"],
    theme: {
        extend: {},
    },
    daisyui: {
        themes: ["winter"],
    },
    plugins: [require("@tailwindcss/typography"), require("daisyui")],
}

