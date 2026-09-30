window.themeStorage = {
    get: () => localStorage.getItem('cinema-theme'),
    set: (value) => localStorage.setItem('cinema-theme', value)
};