const paths = require('./paths');
const miniCssExtractPlugin = require('mini-css-extract-plugin');
const extraWatchWebpackPlugin = require('extra-watch-webpack-plugin');

module.exports = {
    entry: [paths.js,paths.scss],
    output : {
        path: paths.dist,
        filename: 'main.js'
    },
    plugins: [new miniCssExtractPlugin({
        filename: '[name].css'
    }),
        new extraWatchWebpackPlugin({
            dirs: [paths.src, paths.tail]
        })
    ],
    module: {
        rules: [
            {
                test: /\.scss$/i,
                exclude: /node_modules/,
                use: [
                    {
                        loader: miniCssExtractPlugin.loader
                    },
                    {
                        loader: 'css-loader',
                        options: {
                            importLoaders: 1,
                        }
                    },
                    {
                      loader: 'sass-loader'  
                    },
                    {
                        loader: 'postcss-loader',
                        options: {
                            postcssOptions: {
                                plugins:[
                                    'postcss-import',
                                    'tailwindcss',
                                    'postcss-nested',
                                    'autoprefixer'
                                ]
                            }
                        }
                    }
                ]
            },
        ],
    },
}

