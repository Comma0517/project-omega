const { src, dest, parallel, series } = require('gulp')
const { spawn } = require('child_process')
const argv = require('yargs').argv
const path = require('path')
const rimraf = require('rimraf')
const fsp = require('fs').promises

const spawnOptions = {
  shell: true,
  cwd: __dirname,
  stdio: ['ignore', 'inherit', 'inherit'],
}
const spawnOptionsWithInput = { ...spawnOptions, stdio: 'inherit' }
const dockerSpawnOptions = { ...spawnOptions, cwd: path.resolve(__dirname, 'docker') }

function waitForProcess(childProcess) {
  return new Promise((resolve, reject) => {
    childProcess.once('exit', (returnCode) => {
      if (returnCode === 0) {
        resolve(returnCode)
      } else {
        reject(returnCode)
      }
    })
    childProcess.once('error', (err) => {
      reject(err)
    })
  })
}

async function dotnetPublish() {
  const args = ['publish', '-c', 'Release']
  return waitForProcess(spawn('dotnet', args, spawnOptions))
}

async function yarnBuild() {
  const args = ['--cwd', 'Omega/client-app', 'build']
  return waitForProcess(spawn('yarn', args, spawnOptions))
}

async function dockerBash() {
  const args = ['run', '--rm', '--entrypoint', '"bash"', '-it', `omega:${argv.imageName}`]
  return waitForProcess(spawn('docker', args, spawnOptionsWithInput))
}

async function dockerBuild() {
  return waitForProcess(spawn('docker-compose', ['build'], dockerSpawnOptions))
}

async function dockerUp() {
  return waitForProcess(spawn('docker-compose', ['up'], dockerSpawnOptions))
}

async function dockerDown() {
  return waitForProcess(spawn('docker-compose', ['down'], dockerSpawnOptions))
}

async function dockerStop() {
  return waitForProcess(spawn('docker-compose', ['stop'], dockerSpawnOptions))
}

async function copyPublishedToDockerDir() {
  const dockerBuiltServerPath = path.join(__dirname, 'docker/built_server_app/')
  const dockerBuiltClientPath = path.join(__dirname, 'docker/built_client_app/')
  await Promise.all([
    new Promise(resolve => rimraf(dockerBuiltServerPath, resolve)),
    new Promise(resolve => rimraf(dockerBuiltClientPath, resolve))
  ])
  await Promise.all([fsp.mkdir(dockerBuiltServerPath), fsp.mkdir(dockerBuiltClientPath)])

  // For excluding a dir, see https://github.com/gulpjs/gulp/issues/165#issuecomment-32611271
  src(['Omega/bin/Release/net5.0/publish/**/*', '!**/client-app', '!**/client-app/**']).pipe(dest(dockerBuiltServerPath))
  src('Omega/client-app/build/**/*').pipe(dest(dockerBuiltClientPath))
}

const build = parallel(dotnetPublish, yarnBuild)

// Run dotnet publish and yarn build in parallel.
exports.build = build

// Run docker-compose build. Can also just run dockerUp since it will build if the image doesn't exist.
exports.dockerBuild = dockerBuild

// Build the docker images/network if they don't exist and start containers.
exports.dockerUp = dockerUp

// Tear down the docker containers/network.
exports.dockerDown = dockerDown

// Sometimes ctrl-C doesn't stop the containers and they're left running - stop with this task
exports.dockerStop = dockerStop

// Run images with bash entrypoint to troubleshoot what files are ending up in the built images. Requires an arg --image-name to be passed. See package.json for examples.
exports.dockerBash = dockerBash

// Good for rapidly making docker-compose changes and testing them
exports.dockerRecreate = series(dockerDown, dockerUp)

// Good for testing app in docker after making source changes. Completely rebuilds the images before bringing containers up.
exports.dockerRecreateFull = series(parallel(dockerDown, build), copyPublishedToDockerDir, dockerBuild, dockerUp)