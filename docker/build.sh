#!/bin/bash 

# Building outside the container so making docker changes and rebuilding isn't slow

rm -rf deploy
mkdir deploy
cp -r ../Omega/bin/Release/net5.0/publish/ ./deploy/

docker build -t omega:1.0  .
