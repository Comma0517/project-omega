const {src, dest, parallel, series} = require('gulp')
const {spawn, spawnSync} = require('child_process')
const {argv} = require('yargs')
const path = require('path')
const rimraf = require('rimraf')
const fs = require('fs')
const fsp = require('fs').promises
const which = require('which')

const clientAppPath = 'src/services/OmegaService.Web/client-app/'
const serverAppPath = 'src/Omega/'
const dbMigratorPath = 'src/libs/Omega.DbMigrator/'
const dockerDirPath = 'deploy/docker/'

const dockerProjectName = 'omega' // The docker project name option prevents warnings about unrelated orphaned containers
const dockerDepsProjectName = 'omega_deps'
const dockerDepsComposeName = 'docker-compose.deps.yml'
const dockerNetworkName = 'omega-net'

const spawnOptions = {
  shell: true,
  cwd: __dirname,
  stdio: ['ignore', 'inherit', 'inherit'],
}
const spawnOptionsWithInput = {...spawnOptions, stdio: 'inherit'}
const dockerSpawnOptions = {...spawnOptions, cwd: path.resolve(__dirname, dockerDirPath)}
const migratorSpawnOptions = {...spawnOptions, cwd: path.resolve(__dirname, dbMigratorPath)}
const migratorSpawnOptionsWithInput = {...migratorSpawnOptions, stdio: 'inherit'}

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

function getEnvDictionary(filePath) {
  let dict = {}
  fs.readFileSync(filePath).toString().split('\n').forEach(function (line) {
    if (line && line.indexOf('=') !== -1) {
      line = line.replace('\r', '').trim()
      let parts = line.split('=')
      dict[parts[0].trim()] = parts[1].trim()
    }
  })
  return dict
}

async function copyEnv(templateDir, templateFilename, envDir, envFilename, overrideAll = true) {
  await ensureEnvFile(templateDir, templateFilename, envDir, envFilename)

  let templateDict = getEnvDictionary(templateDir + templateFilename)
  let envDict = getEnvDictionary(envDir + envFilename)

  // Determine what keys are missing from .env that are in template
  let templateKeys = Object.keys(templateDict)
  let envKeys = Object.keys(envDict)
  let missingKeys = templateKeys.filter(k => !envKeys.includes(k))

  if (missingKeys.length > 0) {
    console.log(`Adding missing keys for ${envDir + envFilename}: `, missingKeys)
  }

  // Merge missing values with existing
  let newEnvDict = {}
  for (const [key, value] of Object.entries(overrideAll ? templateDict : envDict)) {
    newEnvDict[key] = value
  }
  for (const key of missingKeys) {
    newEnvDict[key] = templateDict[key]
  }

  // Sort
  let newDictEntries = Object.entries(newEnvDict)
  let newSortedEntries = newDictEntries.sort((a, b) => {
    if (a < b) {
      return -1
    }
    if (a > b) {
      return 1
    }
    return 0
  })

  // Write to .env file
  let newEnvFileContent = ''
  for (let kvp of newSortedEntries) {
    newEnvFileContent += `${kvp[0]}=${kvp[1]}\n`
  }
  await fsp.writeFile(envDir + envFilename, newEnvFileContent)
}

async function ensureEnvFile(templateDir, templateFilename, envDir, envFilename) {
  const envTemplatePath = `${templateDir}${templateFilename}`
  const envPath = `${envDir}${envFilename}`
  if (!fs.existsSync(envPath)) {
    console.log('Created new env file ' + envPath)
    await fsp.copyFile(envTemplatePath, envPath)
  }
}

async function syncEnvFiles() {
  const rootTemplateDir = './'
  const rootServerTemplateFilename = '.env.server.template'
  const rootClientTemplateFilename = '.env.client.template'
  const rootEnvDir = './'
  const rootServerEnvFilename = '.env.server'
  const rootClientEnvFilename = '.env.client'

  // Copy root .env.[category].template to .env
  await copyEnv(rootTemplateDir, rootServerTemplateFilename, rootEnvDir, rootServerEnvFilename, false)
  await copyEnv(rootTemplateDir, rootClientTemplateFilename, rootEnvDir, rootClientEnvFilename, false)

  // Copy root .env.[category] to subdirectory .env files
  await copyEnv(rootTemplateDir, rootServerEnvFilename, serverAppPath, '.env')
  await copyEnv(rootTemplateDir, rootServerEnvFilename, dockerDirPath, '.env')
  await copyEnv(rootTemplateDir, rootServerEnvFilename, dbMigratorPath, '.env')
  await copyEnv(rootTemplateDir, rootClientEnvFilename, clientAppPath, '.env')
}

async function yarnInstallClientApp() {
  const args = ['--cwd', clientAppPath, 'install']
  return waitForProcess(spawn('yarn', args, spawnOptions))
}

async function yarnStartClient() {
  const args = ['--cwd', clientAppPath, 'start']
  return waitForProcess(spawn('yarn', args, spawnOptions))
}

async function dotnetWatchRun() {
  const args = ['watch', '-p', serverAppPath, 'run']
  return waitForProcess(spawn('dotnet', args, spawnOptions))
}

async function dotnetPublish() {
  const args = ['publish', '-c', 'Release']
  return waitForProcess(spawn('dotnet', args, spawnOptions))
}

async function yarnBuild() {
  const args = ['--cwd', 'src/services/OmegaService.Web/client-app', 'build']
  return waitForProcess(spawn('yarn', args, spawnOptions))
}

async function ensureDockerIsRunning() {
  if (!which.sync('docker')) {
    throw Error('docker was not found')
  }

  let childProcess = spawnSync('docker', ['info'], {encoding: 'utf8'})
  if (childProcess.error) {
    throw childProcess.error
  }
  if (!childProcess.stdout || childProcess.stdout.includes('ERROR: error during connect')) {
    throw Error('docker is not running')
  }
}

async function dockerBash() {
  await ensureDockerIsRunning()
  const args = ['run', '--rm', '--entrypoint', '"bash"', '-it', `omega_${argv['imageName']}:1.0`]
  return waitForProcess(spawn('docker', args, spawnOptionsWithInput))
}

// We're assuming that deps isn't up by the lack of the network created in docker-compose.deps.yml
async function throwIfDockerDepsNotUp() {
  await ensureDockerIsRunning()
  let childProcess = spawnSync('docker', ['network', 'ls'], {encoding: 'utf8'})
  if (childProcess.error) {
    throw childProcess.error
  }
  console.log('Docker networks:')
  console.log(childProcess.stdout)
  if (!childProcess.stdout || !childProcess.stdout.includes(dockerNetworkName)) {
    console.error(`ERROR: docker network ${dockerNetworkName} not found`)
    throw `docker network ${dockerNetworkName} not found, create using "yarn run dockerDepsUp" or "yarn run dockerDepsUpDetached"`
  } else {
    console.log(`docker network ${dockerNetworkName} found`)
  }
}

async function dockerBuild() {
  await ensureDockerIsRunning()
  return waitForProcess(spawn('docker-compose', ['--project-name', dockerProjectName, 'build', '--no-cache'], dockerSpawnOptions))
}

async function dockerUp() {
  await ensureDockerIsRunning()
  return waitForProcess(spawn('docker-compose', ['--project-name', dockerProjectName, 'up'], dockerSpawnOptions))
}

async function dockerDown() {
  await ensureDockerIsRunning()
  return waitForProcess(spawn('docker-compose', ['--project-name', dockerProjectName, 'down'], dockerSpawnOptions))
}

async function dockerStop() {
  await ensureDockerIsRunning()
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
  await ensureDockerIsRunning()
  const args = ['build', '-f', 'Dockerfile.standalone', '-t', 'omega_standalone:1.0', '.']
  return waitForProcess(spawn('docker', args, dockerSpawnOptions))
}

async function dockerStandaloneRun() {
  await ensureDockerIsRunning()
  // Note that running docker as a gulp command disallows passing the -t command since that would try to configure the terminal
  const args = ['run', '-i', '--rm', '-p', '5000:80', 'omega_standalone:1.0']
  return waitForProcess(spawn('docker', args, dockerSpawnOptions))
}

async function dockerDepsUp() {
  await ensureDockerIsRunning()
  return waitForProcess(spawn('docker-compose', ['--project-name', dockerDepsProjectName, '-f', dockerDepsComposeName, 'up'], dockerSpawnOptions))
}

async function dockerDepsUpDetached() {
  await ensureDockerIsRunning()
  return waitForProcess(spawn('docker-compose', ['--project-name', dockerDepsProjectName, '-f', dockerDepsComposeName, 'up', '-d'], dockerSpawnOptions))
}

async function dockerDepsDown() {
  await ensureDockerIsRunning()
  return waitForProcess(spawn('docker-compose', ['--project-name', dockerDepsProjectName, '-f', dockerDepsComposeName, 'down'], dockerSpawnOptions))
}

async function dockerDepsStop() {
  await ensureDockerIsRunning()
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

const build = parallel(dotnetPublish, yarnBuild)

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
exports.dockerBash = dockerBash

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
