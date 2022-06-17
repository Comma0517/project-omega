# Design Pattern Variations

The high level pattern introduced in this project has been implemented with some specific details that aren't necessarily part of the overall vision. There are variations of several aspects of the pattern that can be used based on the number of developers, the type of product, the frequency of updates, the type of deployment, etc.

## UI Consideration

I have a UI being served from the API gateway because it's convenient for testing and demos, but it isn't strictly part of the pattern. An alternative would be to keep the UI project and the services project separate. I happen to like the UI being part of the whole because often user stories tend to include some UI element and I'd prefer not to have to manage multiple PRs. Ideally user stories would not include both service changes and UI changes, but getting a company to agree to that depends heavily on people's opinions which are sometimes hard to change. For instance there may be issues with QA being unable/unwilling to do API testing, so for the story to be testable they need a UI (unfortunate but it happens).

Also, having the UI included makes the lower end scale for personal hobby projects very convenient.

For larger projects we would want to ensure that built images for the services do not contain the UI app, as that would unnecessarily increase the size of those images dramatically more than additional services. That's fairly simple to do (see my example Dockerfiles), but should be called out here just in case that's a deal breaker in someone's mind.

## Service "Clumps"

There is no theoretically limit to the upper end scale of the hybrid design pattern being introduced by Project Omega.

For extremely large projects with many development teams, it may make a lot of sense to still have separate projects. But rather than having hundreds of them, you could divide up the larger whole into projects that have "clumps" of related services that follow our hybrid pattern.

The most important consideration when choosing this route would be to determine where those boundaries are. Business domain boundaries might be a good option, but you could divide up the whole based on what teams work on the individual parts.

Using these clumps would dramatically reduce the overhead of managing hundreds of microservices and make teams much more efficient.

## Databases and Microservices

One of the advantages of traditional microservices is the ability for each service to have it's own database. Databases are hard to scale. Cost and complexity go up dramatically with size and traffic no matter what you do. Replication, backup, and other DB considerations all have the potential to become complex. The microservices way to attempt to mitigate this is by extracting portions of the application that have more DB needs than other parts so that the service can have it's own database.

Project Omega, like microservices, allows for each service to have it's own database. As with other features in this architecture, individual services can operate differently locally compared to when they are deployed. So we could have one DB locally running in docker that all services use, but when deployed, each microservice gets its own DB connection string/different DB.

A variation to using all separate databases for each service could be to only use separate databases for services that may get advantages from having a separate DB. This is a bigger discussion, but it's worth noting that this variation is possible.

## Deployment Options

One of the main points of the project is to allow deployment as microservices, but my overall approach actually isn't opinionated at all about deployment configuration. During initial phase of an application, you could treat it as single application until you get to a point where there are good service candidates to deploy as microservices.

I've seen a similar pattern to the one I've introduced called "Neomonolith" (see [https://inconshreveable.com/10-07-2015/the-neomonolith/](https://inconshreveable.com/10-07-2015/the-neomonolith/)). It has a similar concept about including multiple services in a single image/instance, but unlike the Project Omega approach the services talk to other services only on the same instance via loopback. Rather than be opinionated about deployment, the goal of Project Omega is to be completely oblivious to how you actually decide to deploy your application. You could mimic exactly how Neomonolith works by having each service talk to others on the same instance over loopback, or you could separate them out into true microservices, or you could deploy as a traditional monolith, or you could mix and match. To the developer it simply doesn't matter. You could work on an app for a year, change your mind about how that works and change it overnight with very little effort via some configuration changes.
