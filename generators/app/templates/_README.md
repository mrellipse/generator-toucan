# <%=name%>

This is an SPA built using .Net Core 2.0, TypeScript, Vue & Vuex.

## Features
* localization
* validation
* global application state/event bus
* multiple optimized entry points
* token-based authentication
* support for local or external authentication providers
* HMR support for development

## Getting Started

These instructions will get a copy of the project up and running on your local machine for development purposes.

### Prerequisites


### Installing


#### Project Dependencies
Update and build the .NET Core projects by switching to to ./src/server and running

```DOS
dotnet build
```

Update and build the TypeScript UI project by switching to ./src/ui and running

```DOS
npm install
npm install webpack -g
npm install typings -g
typings install
webpack --config webpack.config.js
```
#### Configuration

The first step in configuration is to decide what backing database to use, and then invoke scaffolding tools to generate initial data migrations.

##### Startup
Run the project by switching to ./src/server and running

```DOS
dotnet run -p server.csproj -c Development
```
You should now be able to load the site at [http://localhost:<%=port%>/](http://localhost:<%=port%>/) 


## Overview


## Authors

* 

## License

This project is licensed under the ***

## Acknowledgments

* [mrellipse](https://github.com/mrellipse) for [Toucan](https://github.com/mrellipse/toucan)

