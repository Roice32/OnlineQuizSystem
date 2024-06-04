/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      keyframes: {
        pulseColor: {
          '0%, 100%': { color: '#1c4e4f' },
          '50%': { color: '#3d7a7b' },
        }
      },
      animation: {
        pulseColor: 'pulseColor 2s cubic-bezier(0, 0, 0.2, 1) infinite',
      }
    }
  },
  plugins: [],
}