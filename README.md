# tempdoc
[![.NET](https://img.shields.io/badge/--512BD4?logo=.net&logoColor=ffffff)](https://dotnet.microsoft.com/)
[![JavaScript](https://img.shields.io/badge/--F7DF1E?logo=javascript&logoColor=000)](https://www.javascript.com/)
[![Docker](https://badgen.net/badge/icon/docker?icon=docker&label)](https://https://docker.com/)
[![Generic badge](https://img.shields.io/badge/MongoDBGridFS-2.19.0-green.svg)](https://www.nuget.org/packages/MongoDB.Driver.GridFS)
[![Generic badge](https://img.shields.io/badge/Quartz-7.0.4-purple.svg)](https://www.nuget.org/packages/Quartz)
[![Generic badge](https://img.shields.io/badge/Grpc.Tools-2.52.0-blue.svg)](https://www.nuget.org/packages/Grpc.Tools)

Проект для практики работы с MongoDB GridFS и gRPC

Временное хранилище файлов

## Запуск
Необходимо: Docker

### Установка
```
git clone https://github.com/gfg7/tempdoc.git
```

Переменные среды gRPCServer:
```
MONGO_USERNAME= string
MONGO_PASSWORD= string
MONGO_DB= string
MONGO_PORT= int
INFO_COLLECTION= string
DEFAULT_MAX_COUNT= int
DEFAULT_TEMP_TIME= int //minutes 
CHUNK_SIZE= int
DEFAULT_MAX_FILE_SIZE= int?
CRON_FLUSH_EXPIRED= cron expression
CACHE_LIFETIME= int //minutes
SHOW_API= bool
```

Запуск через docker-compose:
```
docker-compose -f docker-compose.yml up -d --build
```
<details>

<summary>EN</summary>
Tutorial project for MongoDB & gRPC practice

Temporary cloud storage

Requirments: Docker

Installation:
```
git clone https://github.com/gfg7/tempdoc.git
```

Custom environmental variables of gRPCServer:
```
MONGO_USERNAME= string
MONGO_PASSWORD= string
MONGO_DB= string
MONGO_PORT= int
INFO_COLLECTION= string
DEFAULT_MAX_COUNT= int
DEFAULT_TEMP_TIME= int //minutes 
CHUNK_SIZE= int
DEFAULT_MAX_FILE_SIZE= int?
CRON_FLUSH_EXPIRED= cron expression
CACHE_LIFETIME= int //minutes
SHOW_API= bool
```

Start up via docker-compose file:
```
docker-compose -f docker-compose.yml up -d --build
```
</details>

[![ForTheBadge built-with-love](http://ForTheBadge.com/images/badges/built-with-love.svg)](https://GitHub.com/gfg7/)
