/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    colors: {
      'dark-green': '#022c22',
      'medium-green': '#4E6C50',
      'brown': '#AA8B56',
      'white': '#F0EBCE',
      'gray': '#eaeaea',
      'red': '#ea0808',
      'blue': '#346beb'

    },
    extend: {},
    container: {
      center: true,
    },
  },
  plugins: [],
}

