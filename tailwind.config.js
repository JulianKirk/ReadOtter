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
        "surface-base": "var(--color-surface-base)",
        "surface-card": "var(--color-surface-card)",
        "surface-raised": "var(--color-surface-raised)",
        border: "var(--color-border)",
        "text-primary": "var(--color-text-primary)",
        "text-secondary": "var(--color-text-secondary)",
        "text-tertiary": "var(--color-text-tertiary)",
        accent: "var(--color-accent)",
        "accent-hover": "var(--color-accent-hover)",
        "accent-muted": "var(--color-accent-muted)",
        danger: "var(--color-danger)",
        "danger-hover": "var(--color-danger-hover)",
      },
      fontFamily: {
        sans: ["Inter", "system-ui", "sans-serif"],
        reading: ["Literata", "Georgia", "serif"],
      },
    },
  },
  plugins: [],
};
