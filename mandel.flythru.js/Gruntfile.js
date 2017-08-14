module.exports = function (grunt) {

    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        babel: {
            options: {
                sourceMap: true,
                presets: ['es2015']
            },
            dist: {
                files: {
                    'build/mandel.flythru.js': 'js/*.js'
                }
            }
        },
        browserify: {
            target: {
                src: ['js/**/*.js'],
                dest: 'build/mandel.flythru.js',
                options: {
                    browserifyOptions: { debug: false },
                    transform: [["babelify", { "presets": ["es2015"] }]]
                }
            }
        },
        uglify: {
            production_target: {
                files: {
                    'build/mandel.flythru.min.js': ['build/mandel.flythru.js']
                }
            }
        }
    });


    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-babel');
    grunt.loadNpmTasks('grunt-browserify');

    grunt.registerTask('default', ['browserify']);

};