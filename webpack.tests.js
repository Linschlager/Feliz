var path = require("path");
var webpack = require("webpack");

module.exports = function (evn, argv) {
    var mode = argv.mode || "development";
    var isProduction = mode === "production";
    console.log("Webpack mode: " + mode);

    return {
        mode: mode,
        devtool: isProduction ? false : "eval-source-map",
        plugins: [
            // fix "process is not defined" error:
            new webpack.DefinePlugin({
                'process.env': {},
            }),
        ],
        entry: './tests/Tests.fs.js',
        output: {
            filename: 'tests.js',
            path: path.join(__dirname, './public-tests'),
        }
    };
}