// noinspection JSCheckFunctionSignatures

const {src, dest, parallel, series} = require('gulp')
const {spawn} = require('child_process')
const {argv} = require('yargs')
const path = require('path')
const rimraf = require('rimraf')
const fsp = require('fs').promises
const {
  defaultSpawnOptions,
  waitForProcess,
  copyNewEnvValues,
  overwriteEnvFile,
  throwIfDockerNotRunning,
  dockerContainerIsRunning,
  bashIntoRunningDockerContainer
} = require('@mikeyt23/gulp-utils')

const clientAppPath = 'src/services/OmegaService.Web/client-app/'
const serverAppPath = 'src/Omega/'
const dbMigratorPath = 'src/libs/Omega.DbMigrator/'
const dockerDirPath = 'deploy/docker/'

const dockerProjectName = 'omega' // The docker project name option prevents warnings about unrelated orphaned containers
const dockerDepsProjectName = 'omega_deps'
const dockerDepsComposeName = 'docker-compose.deps.yml'

const spawnOptions = {...defaultSpawnOptions, cwd: __dirname}
const dockerSpawnOptions = {...spawnOptions, cwd: path.resolve(__dirname, dockerDirPath)}
const clientSpawnOptions = {...spawnOptions, cwd: path.resolve(__dirname, clientAppPath)}
const serverAppSpawnOptions = {...spawnOptions, cwd: path.resolve(__dirname, serverAppPath)}
const migratorSpawnOptions = {...spawnOptions, cwd: path.resolve(__dirname, dbMigratorPath)}
const migratorSpawnOptionsWithInput = {...migratorSpawnOptions, stdio: 'inherit'}

async function syncEnvFiles() {
  const rootServerEnv = './.env.server'
  const rootClientEnv = './.env.client'

  // Copy root .env.[category].template to .env
  await copyNewEnvValues(`${rootServerEnv}.template`, rootServerEnv)
  await copyNewEnvValues(`${rootClientEnv}.template`, rootClientEnv)
  

  // Copy root .env.[category] to subdirectory .env files
  await overwriteEnvFile(rootServerEnv, path.join(serverAppPath, '.env'))
  await overwriteEnvFile(rootServerEnv, path.join(dockerDirPath, '.env'))
  await overwriteEnvFile(rootServerEnv, path.join(dbMigratorPath, '.env'))
  await overwriteEnvFile(rootClientEnv, path.join(clientAppPath, '.env'))
}

async function deleteEnvFiles() {
  await Promise.all([
    fsp.unlink(path.join('./.env.server')),
    fsp.unlink(path.join('./.env.client')),
    fsp.unlink(path.join(serverAppPath, '.env')),
    fsp.unlink(path.join(dockerDirPath, '.env')),
    fsp.unlink(path.join(dbMigratorPath, '.env')),
    fsp.unlink(path.join(clientAppPath, '.env'))
  ])
}

async function yarnInstallClientApp() {
  return waitForProcess(spawn('yarn', ['install'], clientSpawnOptions))
}

async function yarnStartClient() {
  return waitForProcess(spawn('yarn', ['start'], clientSpawnOptions))
}

async function dotnetWatchRun() {
  return waitForProcess(spawn('dotnet', ['watch', 'run'], serverAppSpawnOptions))
}

async function dotnetPublish() {
  const args = ['publish', '-c', 'Release']
  return waitForProcess(spawn('dotnet', args, spawnOptions))
}

async function yarnBuildClientApp() {
  return waitForProcess(spawn('yarn', ['build'], clientSpawnOptions))
}

async function dockerBashRunningContainer() {
  await throwIfDockerNotRunning()
  await bashIntoRunningDockerContainer(`${dockerDepsProjectName}_${argv['imageName']}`)
}

// We're assuming that dependencies aren't up if the mssql container isn't running
async function throwIfDockerDepsNotUp() {
  const mssqlIsRunning = await dockerContainerIsRunning('omega_mssql')
  if (!mssqlIsRunning) {
    throw 'Docker dependencies are not running'
  }
}

async function dockerBuild() {
  await throwIfDockerNotRunning()
  return waitForProcess(spawn('docker-compose', ['--project-name', dockerProjectName, 'build', '--no-cache'], dockerSpawnOptions))
}

async function dockerUp() {
  await throwIfDockerNotRunning()
  return waitForProcess(spawn('docker-compose', ['--project-name', dockerProjectName, 'up'], dockerSpawnOptions))
}

async function dockerDown() {
  await throwIfDockerNotRunning()
  return waitForProcess(spawn('docker-compose', ['--project-name', dockerProjectName, 'down'], dockerSpawnOptions))
}

async function dockerStop() {
  await throwIfDockerNotRunning()
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
  await throwIfDockerNotRunning()
  const args = ['build', '-f', 'Dockerfile.standalone', '-t', 'omega_standalone:1.0', '.']
  return waitForProcess(spawn('docker', args, dockerSpawnOptions))
}

async function dockerStandaloneRun() {
  await throwIfDockerNotRunning()
  // Note that running docker as a gulp command disallows passing the -t command since that would try to configure the terminal
  const args = ['run', '-i', '--rm', '-p', '5000:80', 'omega_standalone:1.0']
  return waitForProcess(spawn('docker', args, dockerSpawnOptions))
}

async function dockerDepsUp() {
  await throwIfDockerNotRunning()
  return waitForProcess(spawn('docker-compose', ['--project-name', dockerDepsProjectName, '-f', dockerDepsComposeName, 'up'], dockerSpawnOptions))
}

async function dockerDepsUpDetached() {
  await throwIfDockerNotRunning()
  return waitForProcess(spawn('docker-compose', ['--project-name', dockerDepsProjectName, '-f', dockerDepsComposeName, 'up', '-d'], dockerSpawnOptions))
}

async function dockerDepsDown() {
  await throwIfDockerNotRunning()
  return waitForProcess(spawn('docker-compose', ['--project-name', dockerDepsProjectName, '-f', dockerDepsComposeName, 'down'], dockerSpawnOptions))
}

async function dockerDepsStop() {
  await throwIfDockerNotRunning()
  return waitForProcess(spawn('docker-compose', ['--project-name', dockerDepsProjectName, '-f', dockerDepsComposeName, 'stop'], dockerSpawnOptions))
}

async function deleteMigratorPublishDir() {
  const rootMigratorPublishPath = path.join(__dirname, 'src/libs/Omega.DbMigrator/publish/')
  await new Promise(resolve => rimraf(`${rootMigratorPublishPath}*`, resolve))
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

const build = parallel(dotnetPublish, yarnBuildClientApp)

// Runs yarn install on client app while also copying client-app .env.template to .env.
// Add other initial setup tasks here when they're needed.
exports.initialInstall = parallel(yarnInstallClientApp, syncEnvFiles)

// Run client app
exports.startClient = series(syncEnvFiles, yarnStartClient)

// Run dotnet server app
exports.startServer = series(syncEnvFiles, dotnetWatchRun)

// Run dotnet publish and yarn build in parallel.
exports.build = build

// Run docker-compose build. Can also just run dockerUp since it will build if the image doesn't exist.
exports.dockerBuild = series(dockerBuild)

// Build the docker images/network if they don't exist and start containers.
exports.dockerUp = series(dockerUp)

// Tear down the docker containers/network.
exports.dockerDown = dockerDown

// Sometimes ctrl-C doesn't stop the containers and they're left running - stop with this task
exports.dockerStop = dockerStop

// Run images with bash entrypoint to troubleshoot what files are ending up in the built images. Requires an arg --image-name to be passed. See package.json for examples.
exports.dockerBash = dockerBashRunningContainer

// Good for rapidly making docker-compose changes and testing them
exports.dockerRecreate = series(throwIfDockerDepsNotUp, dockerDown, dockerUp)

// Good for testing app in docker after making source changes. Completely rebuilds the images before bringing containers up.
exports.dockerRecreateFull = series(throwIfDockerDepsNotUp, parallel(dockerDown, build), copyPublishedToDockerDir, dockerBuild, dockerUp)
exports.dockerRecreateServer = series(throwIfDockerDepsNotUp, parallel(dockerDown, dotnetPublish), copyPublishedToDockerDir, dockerBuild, dockerUp)

// Docker standalone build
exports.dockerStandaloneBuild = dockerStandaloneBuild
exports.dockerStandaloneRun = series(throwIfDockerDepsNotUp, dockerStandaloneRun)

// Docker dependency operations
exports.dockerDepsUp = series(syncEnvFiles, dockerDepsUp)
exports.dockerDepsUpDetached = series(syncEnvFiles, dockerDepsUpDetached)
exports.dockerDepsDown = dockerDepsDown
exports.dockerDepsStop = dockerDepsStop

// DB operations
exports.dbMigrate = series(throwIfDockerDepsNotUp, parallel(syncEnvFiles, deleteMigratorPublishDir), publishMigrator, runDbMigrator)
exports.dbDropAll = series(throwIfDockerDepsNotUp, parallel(syncEnvFiles, deleteMigratorPublishDir), publishMigrator, dbDropAll)
exports.testDbMigrate = series(throwIfDockerDepsNotUp, parallel(syncEnvFiles, deleteMigratorPublishDir), publishMigrator, testDbMigrate)

// Create .env files if they don't exist and adds any new key value pairs. Also changes existing values in subdirectory .env files if non-template values have changed.
exports.syncEnvFiles = syncEnvFiles
exports.deleteEnvFiles = deleteEnvFiles
