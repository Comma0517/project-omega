const { src, dest, task, parallel, series, watch } = require('gulp')
const { spawn } = require('child_process')

const spawnOptions = {
  shell: true,
  cwd: __dirname,
  stdio: ['ignore', 'inherit', 'inherit'],
}

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
  const args = ['publish']
  return waitForProcess(spawn('dotnet', args, spawnOptions))
}

async function yarnBuild() {
  const args = ['--cwd', 'Omega/client-app', 'build']
  return waitForProcess(spawn('yarn', args, spawnOptions))
}

exports.build = parallel(dotnetPublish, yarnBuild)
