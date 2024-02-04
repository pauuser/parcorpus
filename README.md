# Parallel corpus Backend

Parcorpus is a Web API for working with a parallel corpus of translated texts. With the use of Parcorpus, you can search for translations of a word along with its usage examples.

## What is a parallel corpus?

A parallel corpus is a bilingual corpus containing texts simultaneously both in its original language and in some other language [[Khosla, 2018](https://www.researchgate.net/publication/327195382_A_SURVEY_REPORT_ON_THE_EXISTING_METHODS_OF_BUILDING_A_PARALLEL_CORPUS)].

Today, the parallel corpus has become an integral part of the translation process. The translator needs resources that offer him options for interpreting the source text to confirm his own hypotheses. In practice, more than half of the time spent on translation is spent on helping materials such as dictionaries [[Rura, 2008](https://www.semanticscholar.org/paper/Designing-a-parallel-corpus-as-a-multifunctional-Rura-Vandeweghe/2c7f7cdcb5210fe6a4659edd1892c095eb7f0444)]. Thus, using a parallel corpus can significantly speed up the translator's work.

Contrary to most of the research suggesting storing the corpus as a set of XML files, this project proposes the **implementation of a parallel corpus using a relational data model.**

## Features

- Word search with its translation and usage examples
- Add new texts for aligning
- Manage your texts
- Navigate through your search history
- JWT-based authorization
- Nginx load balancing
- Database replication
- Producer-consumer pattern for aligning new texts

## Technologies

- C#, .NET 7.0, ASP.NET Core
- PostgreSQL / [bitnami](https://hub.docker.com/r/bitnami/postgresql/tags) image for replication
- [xUnit](https://xunit.net/), [Testcontainers](https://testcontainers.com/), [Respawn](https://github.com/jbogard/Respawn) and [FlueFlame](https://isbronny.github.io/FlueFlame/) for tests
- NGINX
- RabbitMQ

## Documentation

Documentation can be found in [docs folder](./docs/). It contains database schemes, UML diagrams, OpenAPI specification and some domain analysis in Russian.

![er-db](./docs/diagrams/er-base.svg)