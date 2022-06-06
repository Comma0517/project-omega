const { src, dest, parallel, series } = require('gulp')
const { spawn } = require('child_process')
const argv = require('yargs').argv
const path = require('path')
const rimraf = require('rimraf')
const fs = require('fs')
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
  console.log('dockerBuiltServerPath: ', dockerBuiltServerPath)
  console.log('dockerBuiltClientPath: ', dockerBuiltClientPath)
  await Promise.all([
    new Promise(resolve => rimraf(dockerBuiltServerPath, resolve)),
    new Promise(resolve => rimraf(dockerBuiltClientPath, resolve))
  ])
  await Promise.all([fsp.mkdir(dockerBuiltServerPath), fsp.mkdir(dockerBuiltClientPath)])
  
  const builtServerPath = path.join(__dirname, 'Omega/bin/Release/net5.0/publish/')
  const builtClientPath = path.join(__dirname, 'Omega/client-app/build/')
  
  console.log('builtServerPath: ', builtServerPath)
  console.log('builtClientPath: ', builtClientPath)
  console.log('------')
  if (fs.existsSync(builtServerPath)) {
    console.log('server path exists')
  }
  if (fs.existsSync(builtClientPath)) {
    console.log('client path exists')
  }
  // return src('*', { base: builtServerPath }).pipe(dest('./docker/built_server_app'))
  // return src('*', { base: 'Omega/bin/Release/net5.0/publish/' }).pipe(dest('./docker/built_server_app'))
  // src('*', { base: builtClientPath }).pipe(dest(dockerBuiltClientPath))
}

const build = parallel(dotnetPublish, yarnBuild)

exports.build = build
exports.dockerBuild = dockerBuild
exports.dockerUp = dockerUp
exports.dockerDown = dockerDown
exports.dockerStop = dockerStop
exports.dockerBash = dockerBash
exports.dockerRecreate = series(dockerDown, dockerUp)
// exports.dockerRecreateFull = series(parallel(dockerDown, build), copyPublishedToDockerDir, dockerUp)
exports.dockerRecreateFull = copyPublishedToDockerDir
