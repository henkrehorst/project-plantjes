const paths = require('./paths');

module.exports = {
    entry: paths.js,
    output : {
        path: paths.dist,
        filename: 'main.js'
    },
    module: {
        rules: [
            {
                test: /\.s[ac]ss$/i,
                exclude: /node_modules/,
                use: ['style-loader', 'postcss-loader', 'sass-loader', 'css-loader'],
            },
        ],
    },
}

