# tempdoc
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
