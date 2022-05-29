#!/bin/bash 

docker run -it --rm -p 5000:80 omega:1.0

# Debug container
# docker run --entrypoint "sh" -it omega:1.0
