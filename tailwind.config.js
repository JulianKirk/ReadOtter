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
        "surface-base": "#FBF7F2",
        "surface-card": "#EDE4D8",
        "surface-raised": "#E0D4C4",
        border: "#C4B5A0",
        "text-primary": "#2C1E10",
        "text-secondary": "#5C4A38",
        "text-tertiary": "#8A7560",
        accent: "#7A4E2D",
        "accent-hover": "#5E3A1F",
        "accent-muted": "#7A4E2D1A",
        danger: "#B83220",
        "danger-hover": "#952818",
      },
      fontFamily: {
        sans: ["Inter", "system-ui", "sans-serif"],
        reading: ["Literata", "Georgia", "serif"],
      },
    },
  },
  plugins: [],
};
