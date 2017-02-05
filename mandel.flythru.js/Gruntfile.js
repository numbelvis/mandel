module.exports = function (grunt) {

    // Project configuration.
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        uglify: {
            production_target: {
                files: {

                    'build/mandel.flythru.min.js': ['js/main.js', 'js/communication-modes/*.js', 'js/computation-modes/*.js']
                }
            },
            debug_target: {
                options: {
                    mangle: false,
                    beautify: true
                },
                files: {

                    'build/mandel.flythru.js': ['js/main.js', 'js/communication-modes/*.js', 'js/computation-modes/*.js']
                }
            }
        }
    });

    // Load the plugin that provides the "uglify" task.
    grunt.loadNpmTasks('grunt-contrib-uglify');

    // Default task(s).
    grunt.registerTask('default', ['uglify']);

};