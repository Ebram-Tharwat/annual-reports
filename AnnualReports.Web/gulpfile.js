var ts = require('gulp-typescript');
var gulp = require('gulp');
var clean = require('gulp-clean');

var destPath = './libs/';

// Delete the dist directory
gulp.task('clean', function () {
    return gulp.src(destPath)
        .pipe(clean());
});

gulp.task("scriptsNStyles", function () {
    gulp.src([
            'core-js/client/*.js',
            'systemjs/dist/*.js',
            'reflect-metadata/*.js',
            'rxjs/**',
            'zone.js/dist/*.js',
            '@angular/**/bundles/*.js',
            'bootstrap/dist/js/*.js',
            'primeng/*.js',
            'primeng/**/components/**/*.js'
    ], {
        cwd: "node_modules/**"
    })
        .pipe(gulp.dest("./libs"));
});

var tsProject = ts.createProject('tsScripts/tsconfig.json', {
    typescript: require('typescript')
});

gulp.task('ts', function (done) {
    //var tsResult = tsProject.src()
    var tsResult = gulp.src([
            'tsScripts/*.ts',
            'tsScripts/**/*.ts'
    ])
        .pipe(tsProject(), undefined, ts.reporter.fullReporter());
    return tsResult.js.pipe(gulp.dest('./Scripts/app/dist'));
});

gulp.task('copy-templates', function () {
    gulp.src('tsScripts/**/*.html')    
    .pipe(gulp.dest('./Scripts/app/dist'));
});

gulp.task('watch', ['watch.ts', 'watch.html']);

gulp.task('watch.ts', ['ts'], function () {
    return gulp.watch(['tsScripts/*.ts', 'tsScripts/**/*.ts'], ['ts']);
});

gulp.task('watch.html', ['copy-templates'], function () {
    return gulp.watch(['tsScripts/**/*.html'], ['copy-templates']);
});

gulp.task('default', ['scriptsNStyles', 'watch']);