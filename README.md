# Project Omega

The last enterprise web architecture pattern you'll ever need. Until the next one.

## TL;DR

The goal is to optimize the developer experience by being able to:

- Develop locally as if it's a monolith
- Deploy as separate microservices
- Simulate the production environment locally using docker

## Demo

(TODO - video demo)

## Why

I want to prove that we don't have to sacrifice developer efficiency to get scalability. More discussion on pros and cons of monoliths and microservices here (TODO).

My impression is that many industry experts would have us believe that these are our main 3 options:

- Monolith
- Microservices
- "Hybrid" (monolith with some microservices)

I want to show that we don't have to pick any of these options. With a little creativity we can have a true "hybrid" that is both a monolith and a set of microservices. With my current strategy I don't think we can eliminate all the downsides of monolith and microservices, but we can get rid of many of the pain points of both.

## What It Is Not

- I'm not trying to create a framework (not yet at least...). I'm just putting together all the legos I have into a different configuration as an experiment.
- This is not meant to be a community project. I won't be taking pull requests. It's my own personal experiment that I intend to make frequent dramatic breaking changes to without notice. If this concept seems interesting to you and you have suggestions, feel free to fork the repo and try out your ideas yourself.

## Project Goals

- Create a pattern that will work for projects as small single developer hobby projects and also scale to dozens or even hundreds of developers working on large and complex enterprise web applications.
- Be able to develop locally as if it's a monolith: 
    - One repository. For all the same reasons companies choose a monorepo approach. 
    - Maximum 3 processes to run (client ui, server, docker dependencies with database, message queue, etc). We don't want pages of setup docs to get up and running.
- Be able to deploy as microservices.
- Be able to simulate a production environment with microservices running in docker containers.
- Extremely fast setup time. All dependencies other than Node and .NET should be included as docker dependencies (database, message queue, etc). New users should be able to install .NET, Node, clone the repo and then execute install and run commands.
- Extremely fast hot reload for both the client and the server in the development environment.
- Be able to develop and run the application on Windows, Linux and Mac.
- Be able to rapidly spin up a new service.

## Tech Stack

The tech stack is mostly irrelevant for the high level concept I'm attempting to prove, but for this project I'm going to be using:

- .NET 5 for services
- React front-end (basic create-react-app with typescript)
- Docker

## High Level Concepts

(TODO - diagram)

## Setup Instructions

Install pre-requisites:

- .NET 5
- Node
- Yarn
- Docker

Steps:

- Clone this repo.
- From repo root, run `yarn run installAll`
- From repo root
