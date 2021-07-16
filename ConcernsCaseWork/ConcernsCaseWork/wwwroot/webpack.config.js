const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CopyPlugin = require("copy-webpack-plugin");

module.exports = (env) => {
	const devMode = env.mode === 'development'
	return {
		mode: env.mode,
		entry: ['./src/js/site.js', './src/css/site.scss'],
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
			new CopyPlugin({
				patterns: [
					{
						from: path.join(__dirname, 'node_modules/govuk-frontend/govuk/assets'),
						to: path.join(__dirname, 'assets')
					},
					{
						from: path.resolve(__dirname, 'node_modules/@ministryofjustice/frontend/moj/assets'),
						to: path.resolve(__dirname, 'assets')
					},
				],
			})
		],
		module: {
			rules: [
				{
					test: /\.s[ac]ss$/i,
					use: [
						devMode ? 'style-loader' :
							// Creates `style` nodes from JS strings
							MiniCssExtractPlugin.loader,
						// Translates CSS into CommonJS
						"css-loader",
						// Compiles Sass to CSS
						{
							loader: "sass-loader",
							options: {
								// Prefer `dart-sass`
								implementation: require("sass"),
							},
						},
					],
				},
				{test: /\.css$/, use: ['style-loader', 'css-loader']},
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
		output: {
			path: path.resolve(__dirname, 'dist'),
			filename: 'site.js',
		}
	}
};