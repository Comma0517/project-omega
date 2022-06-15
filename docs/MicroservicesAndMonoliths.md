# Microservices and Monoliths

This document is meant to gather existing info about microservices and monoliths and add some of my opinions. I'm focusing on what I consider the most important aspects rather than attempting to say everything there is to say on the subject. This is a work in progress.

## Monolith

Small/medium sized application with small number of developers:

| Pros | Cons |
|------|------|
| Fast onboarding for new devs. | Cannot horizontally scale selectively. |
| Easy to run locally. | May be difficult to scale at the DB level depending on site traffic and data requirements. |
| Easy to build and deploy. | Upgrading referenced third party libraries is costly because the entire application must be updated at the same time unless there is an added abstraction layer on top. |
| Easy to upgrade large portions of the application simultaneously. | |
| Easy to scale horizontally (not selectively). | |
| Single PRs rather than multiple PRs for multiple microservices + UI. You might say that for microservices you should only ever modify one service at a time, but that's not been my experience in the real world, for various reasons, good and bad... | |

Notable differences for large and complex application with many developers:

| Pros | Cons |
|------|------|
| Still reasonably fast onboarding for new devs. | Build performance can get unbearably slow the larger the application becomes. |
| Still conceptually easy to build deploy (but could be very slow). | Developers start stepping on each other's toes, modifying the same code concurrently. |
| | Inability to selectively scale horizontally could eventually become very expensive or could even end up a complete deal breaker for using the monolith approach. |
| | Due to difficulty of upgrading any aspect of the application, the shelf-life of the application is going to be shorter. Eventually the cost of maintenance and adding new features will warrant starting over again. Few companies plan on getting into this scenario, but experience says it is still very likely to happen anyway with a monolith approach. |
| | Inability to change overall software patterns without massive expensive re-writes which companies simply can't afford. This often results in simply "living with what we have". This prevents development teams from fixing inefficiencies, :fire: **which compounds over time** :fire:. |

## Microservices

Small application with small number of developers:

| Pros | Cons |
|------|------|
| Can selectively scale parts of the application. | The small number of developers will be slowed down by the complexity of build/deployment unless there is a dedicated devops team. |
| Can update individual parts of the application without affecting the whole. | Difficult to manage infrastructure for the amount of people responsible. |
| Can experiment with new tech for each microservice (good for devs, but maybe not for the employer paying for this). | Making changes to how the microservices work or upgrading common functionality used in all services requires extra work to make the same changes in many places. |
| Can make updates/upgrades incrementally because of the service separation. | Versioning hell. Have to juggle shared library versions and API versions. |
| | Possibly lots of duplicated boilerplate code between services. |
| | Writing sane logging requires significantly more effort than for a monolith. |
| | Onboarding new developers can become difficult. |
| | Scripts must be created and maintained to manage setting things up and running the application locally. |

In addition to points above, for medium/large sized applications with many developers:

| Pros | Cons |
|------|------|
| Big benefit is to allow teams to focus on their own services that they're responsible for without being affected by technical debt or other issues from other parts of the application. | With a big enough application, there may end up being no one at the company that actually knows how the whole application functions. In some sense this is ideal (single responsibility), but in the real world someone has to maintain the plumbing that makes them all work together, and when something goes wrong, getting multiple teams of people to coordinate is not easy. |
| | A large monolith might still be able to run on one developer machine (though maybe with bad performance). A big application divided into microservices is going to quickly hit a limit of what's possible to run locally.<br /><br />Some companies mitigate this by running "dev" servers with some of the services or DBs/queues, etc. This is an absolute nightmare to work with as a developer. Working remotely probably requires using a crumby VPN with horrible latency. If someone accidentally takes down these servers, many devs are out of commission. Understanding how local services are interacting with remote services starts to become a large majority of total time spent on any given task. <br /><br /> In my opinion, **this is a big fat deal breaker** if you're not setup so you only ever have to develop services in isolation. Companies that don't have the resources to do this correctly need some other option - microservices is the absolute worst thing they could do to themselves. |

## "Hybrid"

If you google "monolith microservices hybrid", the most common thing you'll get is advice to peel off parts of your monolith into microservices, while keeping your monolith intact. This seems good on the surface. If you have the developer resources and you're just looking to fix a scale issue, this could be your ideal solution.

If you're trying to mitigate issues other than scaling and don't have sufficient developer resources, you've probably done exactly the opposite of your intention. You may have solved one or two issues, but you now have the downsides of both a monolith and microservices, and have to maintain them both.

## Conclusion

My opinion is that there are some very severe issues with both monolith and microservices for **large complex applications** with a **small/medium number of developers**. Project Omega is an attempt to experiment with a hybrid solution that can work for this specific scenario, but that can also be used for single dev small projects or enormous projects with hundreds of developers. 
