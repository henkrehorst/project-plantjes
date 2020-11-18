const path = require('path');

module.exports = {
    js: path.resolve(__dirname, './../src/js/index.js'),
    scss: path.resolve(__dirname, './../src/scss/main.scss'),
    dist: path.resolve(__dirname, '../../wwwroot'),
    src: path.resolve(__dirname, './../src')
}