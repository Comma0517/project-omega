const { src, dest, parallel, series } = require('gulp')
const { spawn } = require('child_process')
const argv = require('yargs').argv
const path = require('path')
const rimraf = require('rimraf')
const fs = require('fs')
const fsp = require('fs').promises

const clientAppPath = 'src/services/OmegaService.Web/client-app/'

// The project name option prevents warnings about unrelated orphaned containers
const dockerProjectName = 'omega';
const dockerDepsProjectName = 'omega_deps';

const spawnOptions = {
  shell: true,
  cwd: __dirname,
  stdio: ['ignore', 'inherit', 'inherit'],
}
const spawnOptionsWithInput = { ...spawnOptions, stdio: 'inherit' }
const dockerSpawnOptions = { ...spawnOptions, cwd: path.resolve(__dirname, 'deploy/docker') }
const migratorSpawnOptions = { ...spawnOptions, cwd: path.resolve(__dirname, 'src/libs/Omega.DbMigrator/') }
const migratorSpawnOptionsWithInput = { ...migratorSpawnOptions, stdio: 'inherit' }

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

async function ensureEnvFile(dir) {
  const envTemplatePath = `${dir}.env.template`
  const envFilePath = `${dir}.env`
  if (!fs.existsSync(envFilePath)) {
    await fsp.copyFile(envTemplatePath, envFilePath)
  }
}

async function yarnInstallClientApp() {
  const args = ['--cwd', clientAppPath, 'install']
  return waitForProcess(spawn('yarn', args, spawnOptions))
}

async function ensureClientAppEnvFile() {
  return ensureEnvFile(clientAppPath)
}

async function dotnetPublish() {
  const args = ['publish', '-c', 'Release']
  return waitForProcess(spawn('dotnet', args, spawnOptions))
}

async function yarnBuild() {
  const args = ['--cwd', 'src/services/OmegaService.Web/client-app', 'build']
  return waitForProcess(spawn('yarn', args, spawnOptions))
}

async function dockerBash() {
  const args = ['run', '--rm', '--entrypoint', '"bash"', '-it', `omega:${argv.imageName}`]
  return waitForProcess(spawn('docker', args, spawnOptionsWithInput))
}

async function dockerBuild() {
  return waitForProcess(spawn('docker-compose', ['--project-name', dockerProjectName, 'build', '--no-cache'], dockerSpawnOptions))
}

async function dockerUp() {
  return waitForProcess(spawn('docker-compose', ['--project-name', dockerProjectName, 'up'], dockerSpawnOptions))
}

async function dockerDown() {
  return waitForProcess(spawn('docker-compose', ['--project-name', dockerProjectName, 'down'], dockerSpawnOptions))
}

async function dockerStop() {
  return waitForProcess(spawn('docker-compose', ['--project-name', dockerProjectName, 'stop'], dockerSpawnOptions))
}

async function copyPublishedToDockerDir() {
  const dockerBuiltServerPath = path.join(__dirname, 'deploy/docker/built_server_app/')
  const dockerBuiltClientPath = path.join(__dirname, 'deploy/docker/built_client_app/')
  await Promise.all([
    new Promise(resolve => rimraf(dockerBuiltServerPath, resolve)),
    new Promise(resolve => rimraf(dockerBuiltClientPath, resolve))
  ])
  await Promise.all([fsp.mkdir(dockerBuiltServerPath), fsp.mkdir(dockerBuiltClientPath)])

  // For excluding a dir, see https://github.com/gulpjs/gulp/issues/165#issuecomment-32611271
  src(['src/Omega/bin/Release/net5.0/publish/**/*', '!**/client-app', '!**/client-app/**']).pipe(dest(dockerBuiltServerPath))
  src('src/services/OmegaService.Web/client-app/build/**/*').pipe(dest(dockerBuiltClientPath))
}

async function dockerStandaloneBuild() {
  const args = ['build', '-f', 'Dockerfile.standalone', '-t', 'omega_standalone:1.0', '.']
  return waitForProcess(spawn('docker', args, dockerSpawnOptions))
}

async function dockerStandaloneRun() {
  // Note that running docker as a gulp command disallows passing the -t command since that would try to configure the terminal
  const args = ['run', '-i', '--rm', '-p', '5000:80', 'omega_standalone:1.0']
  return waitForProcess(spawn('docker', args, dockerSpawnOptions))
}

async function copyDockerEnvFile() {
  return ensureEnvFile('./deploy/docker/')
}

async function dockerDepsUp() {
  return waitForProcess(spawn('docker-compose', ['--project-name', dockerDepsProjectName, '-f', 'docker-compose.deps.yml', 'up'], dockerSpawnOptions))
}

async function dockerDepsUpDetached() {
  return waitForProcess(spawn('docker-compose', ['--project-name', dockerDepsProjectName, '-f', 'docker-compose.deps.yml', 'up', '-d'], dockerSpawnOptions))
}

async function dockerDepsDown() {
  return waitForProcess(spawn('docker-compose', ['--project-name', dockerDepsProjectName, '-f', 'docker-compose.deps.yml', 'down'], dockerSpawnOptions))
}

async function dockerDepsStop() {
  return waitForProcess(spawn('docker-compose', ['--project-name', dockerDepsProjectName, '-f', 'docker-compose.deps.yml', 'stop'], dockerSpawnOptions))
}

async function deleteMigratorPublishDir() {
  const rootMigratorPublishPath = path.join(__dirname, 'src/libs/Omega.DbMigrator/publish/')
  await new Promise(resolve => rimraf(`${rootMigratorPublishPath}*`, resolve))
}

async function ensureDbMigratorEnvFile() {
  return ensureEnvFile('./src/libs/Omega.DbMigrator/')
}

async function publishMigrator() {
  return waitForProcess(spawn('dotnet', ['publish', '-o', 'publish'], migratorSpawnOptions))
}

async function runDbMigrator() {
  return waitForProcess(spawn('dotnet', ['publish/Omega.DbMigrator.dll'], migratorSpawnOptions))
}

async function dbDropAll() {
  return waitForProcess(spawn('dotnet', ['publish/Omega.DbMigrator.dll', 'dropAll'], migratorSpawnOptionsWithInput))
}

async function testDbMigrate() {
  return waitForProcess(spawn('dotnet', ['publish/Omega.DbMigrator.dll', 'testDbMigrate'], migratorSpawnOptionsWithInput))
}

const build = parallel(dotnetPublish, yarnBuild)

// Runs yarn install on client app while also copying client-app .env.template to .env.
// Add other initial setup tasks here when they're needed.
exports.initialInstall = parallel(yarnInstallClientApp, ensureClientAppEnvFile)

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

// Docker standalone build
exports.dockerStandaloneBuild = dockerStandaloneBuild
exports.dockerStandaloneRun = dockerStandaloneRun

// Docker dependency operations
exports.dockerDepsUp = series(copyDockerEnvFile, dockerDepsUp)
exports.dockerDepsUpDetached = series(copyDockerEnvFile, dockerDepsUpDetached)
exports.dockerDepsDown = dockerDepsDown
exports.dockerDepsStop = dockerDepsStop

// DB operations
exports.dbMigrate = series(parallel(ensureDbMigratorEnvFile, deleteMigratorPublishDir), publishMigrator, runDbMigrator)
exports.dbDropAll = series(parallel(ensureDbMigratorEnvFile, deleteMigratorPublishDir), publishMigrator, dbDropAll)
exports.testDbMigrate = series(parallel(ensureDbMigratorEnvFile, deleteMigratorPublishDir), publishMigrator, testDbMigrate)
