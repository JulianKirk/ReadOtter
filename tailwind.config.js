/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./ReadOtter/**/*.razor",
    "./ReadOtter/**/*.html",
    "./ReadOtter.Shared/**/*.razor",
  ],
  theme: {
    extend: {
      colors: {
        "surface-base": "#191919",
        "surface-card": "#242424",
        "surface-raised": "#2e2e2e",
        border: "#333333",
        "text-primary": "#e5e5e5",
        "text-secondary": "#888888",
        "text-tertiary": "#666666",
        accent: "#d4a04a",
        "accent-hover": "#c08d35",
        "accent-muted": "#d4a04a26",
        danger: "#dc2626",
        "danger-hover": "#b91c1c",
      },
      fontFamily: {
        sans: ["Inter", "system-ui", "sans-serif"],
        reading: ["Literata", "Georgia", "serif"],
      },
    },
  },
  plugins: [],
};
