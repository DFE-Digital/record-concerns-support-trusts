const MiniCssExtractPlugin = require('mini-css-extract-plugin');

module.exports = {
	mode: 'production',
	entry: ['./src/css/site.scss'],
	optimization: {
		splitChunks: {
			cacheGroups: {
				styles: {
					name: "styles",
					type: "css/mini-extract",
					chunks: "all",
					enforce: true,
				},
			},
		},
	},
	plugins: [
		new MiniCssExtractPlugin({filename: 'site.css'}),
	],
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
			{ test: /\.css$/, use: ['style-loader', 'css-loader'] },
			{
				test: /\.(woff2?)$/i,
				use: [
					{
						loader: 'file-loader',
						options: {
							emitFile: false,
							name: '/assets/fonts/[name].[ext]'
						}
					}
				]
			},
			{
				test: /\.(jpe?g|png|gif|svg)$/i,
				use: [
					{
						loader: 'file-loader',
						options: {
							emitFile: false,
							name: '/assets/images/[name].[ext]'
						}
					}
				]
			},
		]
	},
};