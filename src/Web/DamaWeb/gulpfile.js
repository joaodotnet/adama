/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. https://go.microsoft.com/fwlink/?LinkId=518007
*/
'use strict';

var gulp = require('gulp');
var sass = require('gulp-sass');

gulp.task('sass', function () {
    return gulp.src('wwwroot/lib/slick-1.8.0/slick/*.scss')
        .pipe(sass().on('error', sass.logError))
        .pipe(gulp.dest(function (file) {
            return file.base;
        }));
});

gulp.task('sass:watch', function () {
    gulp.watch('wwwroot/lib/slick-1.8.0/slick/*.scss', ['sass']);
});