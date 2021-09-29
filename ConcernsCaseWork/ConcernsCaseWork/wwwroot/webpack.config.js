const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const path = require('path');

module.exports = {
	mode: 'production',
	entry: ['./src/js/site.js', './src/css/site.scss'],
	plugins: [new MiniCssExtractPlugin({filename: 'site.css'})],
	module: {
		rules: [
			{
				test: /\.s[ac]ss$/i,
				use: [
					// Creates `style` nodes from JS strings
					MiniCssExtractPlugin.loader,
					// Translates CSS into CommonJS
					"css-loader",
					// Compiles Sass to CSS
					"sass-loader",
				],
			},
			{test: /\.css$/, use: ['style-loader', 'css-loader']},
			{test: /\.(jpe?g|png|gif|svg)$/i, use: 'file-loader'},
			{test: /\.(woff2?)$/i, use: 'file-loader'}
		]
	},
	output: {
		path: path.resolve(__dirname, 'dist/css'),
		filename: 'index.bundle.js',
	}
};