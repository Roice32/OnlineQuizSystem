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
        },
        blingBling: {
          '0%, 100%': { textShadow: '0 0 10px #fff, 0 0 20px #fff, 0 0 30px #fff, 0 0 40px #fff, 0 0 50px #fff, 0 0 60px #fff, 0 0 70px #fff' },
          '50%': { textShadow: '0 0 20px #fff, 0 0 40px #fff, 0 0 60px #fff, 0 0 80px #fff, 0 0 100px #fff, 0 0 120px #fff, 0 0 140px #fff' },
        },
      },
      animation: {
        pulseColor: 'pulseColor 2s cubic-bezier(0, 0, 0.2, 1) infinite',
        blingBling: 'blingBling 1s linear infinite',
      },
    },
  },
  plugins: [],
}