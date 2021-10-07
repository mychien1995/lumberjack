const path = require('path');
const { VueLoaderPlugin } = require('vue-loader');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const HtmlWebpackHarddiskPlugin = require('html-webpack-harddisk-plugin');

module.exports = {
    entry: './src/main.js',
    mode: 'development',
    output: {
        path: path.resolve(__dirname, './dist'),
        publicPath: '/'
    },
    module: {
        rules: [{
            test: /\.vue$/,
            include: /src/,
            loader: 'vue-loader'
        }, {
            test: /\.css$/,
            use: ["vue-style-loader", "css-loader"]
        }, {
            test: /\.scss$/,
            use: ["style-loader", "css-loader", "sass-loader"]
        }]
    },
    resolve: {
        extensions: ['.js', '.vue', '.json'],
        alias: {
            '@': path.resolve('src'),
            '@components': path.resolve('src/components'),
            '@services': path.resolve('src/services'),
        }
    },
    plugins: [
        new VueLoaderPlugin(),
        new HtmlWebpackPlugin({
            template: path.resolve(__dirname, "public", "index.html"),
            favicon: "./public/favicon.ico",
            inject: true,
            alwaysWriteToDisk: true
        }),
        new HtmlWebpackHarddiskPlugin()
    ]
}