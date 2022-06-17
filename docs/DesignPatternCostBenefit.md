# Design Pattern Cost Benefit Analysis

This document is an attempt to document the costs and benefits of the Project Omega microservices monolith hybrid architectural design pattern.

## Design Pattern Costs

I feel strongly that the overall design pattern being used in Project Omega is viable and solves several big issues I see in current enterprise web development. However, it definitely does not solve all the problems, and in some cases simply shifts the complexity and cost from one area to another.

## Granularity of Deployment Cost

An example of this shifting of cost is how this pattern pushes the responsibility for efficient deployment into some non-trivial scripting. For true microservices this isn't a cost because microservices are completely separate and can be deployed separately without triggering building/deployment of all services.

For Project Omega to have similar granularity of deployment, there must be some script that programmatically determines which pseudo microservices have actually been affected by any given change request. If a shared library in the project changes that 3 out of 10 services reference, the script needs to examine those references to determine that only the images for those 3 services need to be built and deployed.

On the upside, this cost can be delayed until there are enough services where there is a benefit to building and deploying fewer than all services at the same time.

## Benefit of Direct Shared Library References

While added complexity to achieve granularity of deployment is definitely a cost, it does have the benefit of wrapping into it the idea that shared library versions will never be out of sync with what is actually deployed. For example, with traditional microservices let's say we have a Foo library on version 1.0 and someone fixes an important bug, increments the version to 1.1 and introduces the new version reference for several services but not all of them.

Then in another future change request another developer fixes a different bug in the Foo library, increments the version to 1.2 and references the new version in services that didn't get the 1.1 updated reference. What happened in this case is that code changes went out the door (both 1.1 and 1.2) that it's likely people weren't aware of or were prepared to test for 1.1 because they thought they were only deploying 1.2 changes. This is a very subtle but destructive practice that requires a lot of overhead to keep track of, especially for large development teams.

With a solution that has all the shared libraries in the same project the above scenario simply isn't possible. If a developer introduces a bug fix for a shared library than all services that reference that library will automatically get that change and any issues with it will be known immediately without getting blindsided in a future release.

Another cost here is that developers would perhaps be forced to make more fixes for an upgrade all at once that they normally wouldn't have to with traditional microservices, but the counter argument is that incremental changes shouldn't be done that way in the first place. If there is a breaking change, the developer should create an alternate new version of whatever the API method is, mark the old one as deprecated and only then do the upgrade incrementally.

The point is that we want to limit the number of ways developers can accidentally shoot themselves in the foot because of disconnected distributed problems that span multiple change requests and releases.

## Cost of Direct Shared Library References

The above benefit is significant, but at the cost of reducing the ease of utilizing the same shared libraries in other applications. The obvious counter argument to this is to pull out shared libraries that are actually good ideas to be shared and are generic enough, but leave the rest in the hybrid single solution. Also, it doesn't have to be either/or. You could include a shared library in a project until it is clear it would be useful to have separate for easier consumption of other applications, and only at that time pull it out into it's own project.

## Shared Data Access Logic Benefit

Having separate databases isn't a Project Omega idea, but a general microservices concept. On the surface there are obvious benefits to having separate databases. For example, we can scale specific parts of the application that have much heavier database usage by using an isolated microservice and database. Developers like this idea. I think that data analysts probably are not big fans. Not just because they can't easily to cross-database queries, but because developers fail to implement a common strategy in each microservice to allow extraction of data for analytics, in general. That's my experience anyway.

Having shared data access code isn't itself the solution, but I would argue that it will significantly increase the likelihood that a common strategy can be created and enforced if it's part of the project rather than having it as an external shared library or worse, duplicated logic between different services (or even worse, some services missing the functionality all together).

## Technology Lock

One reason people like the concept of microservices is because it doesn't lock you into one tech stack, or even the same version of a single tech stack, allowing gradual incremental updates. For example, even if you chose Python, you could have one service running python 2.x and another running 3.x and everyone's happy.

My opinion is maybe a little controversial on this topic, but I think it's worth talking about. If an important goal of the company is efficiency, then requiring developers that may have to work on 10 different services to not only know, but be proficient at 10 different tech stacks could not possibly be further from efficient. This is just a really horribly bad idea, in my humble opinion.

Developers love the idea they can try out something new without having to switch jobs. I like that too. But let's be honest - it's not only inefficient, but it disallows using shared libraries and makes it harder to enforce standard patterns across projects, and after a few months of thinking "Cool, I get to work on something different", the novelty is going to ware off and you may instead be thinking "How in the world was it implemented here? I can't remember. Why can't we do things the same in all projects so I don't have to waste all this time?".

I will admit that one reason locking into one tech stack is bad is because it does make it harder to upgrade versions. Because of that, some technologies are going to cause more pain than others. For example, moving from Java 1.8 to 9 or .NET Core 3 to .NET 5 isn't so bad, but maybe moving from Node 8 to 14 or Python 2.x to 3.x is incredibly difficult if the codebase is large. So that's something to consider.

I would also point out that if you really just wanted 2 or 3 tech stacks, you could have 2 or 3 Project Omega type projects so you have exactly one of each tech stack instead of many of each. That would allow you to "use the best tool for the job" (but let's be honest, it's mostly just to make developers happy), while still giving you the ability to standardize how you use that tech stack without developers going rogue with different patterns in every service.
