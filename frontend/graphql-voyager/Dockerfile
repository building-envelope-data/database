# syntax=docker/dockerfile:1.4
# The above line fixes the Dockerfile frontend used by BuildKit. For details
# see https://github.com/moby/buildkit/blob/master/frontend/dockerfile/docs/reference.md
# Available versions are listed on https://hub.docker.com/r/docker/dockerfile

# Inspired by https://mherman.org/blog/dockerizing-a-react-app/

# https://hub.docker.com/_/node
FROM node:18.13-bullseye-slim

ARG GROUP_ID
ARG USER_ID

# TODO directory node_modules is created on host machine when container is run
# and does not belong to current user. Avoid it being created at all or at
# least make it belong to the current user

#############
# As `root` #
#############

# For best practices on using `apt-get` see
# https://docs.docker.com/develop/develop-images/dockerfile_best-practices/#run
RUN \
  apt-get update && \
  apt-get install \
    --assume-yes \
    --no-install-recommends \
    dumb-init \
    wget && \
  rm \
    --recursive \
    --force \
    /var/lib/apt/lists/*

ENV HOME=/home/node
RUN \
  mkdir --parents ${HOME}/app && \
  chown \
    node:node \
    ${HOME}/app && \
  ln --symbolic ${HOME}/app /app

#############
# As `node` #
#############
USER node
WORKDIR /app

ENV PATH=/app/node_modules/.bin:$PATH
COPY \
  --chown=node:node \
  ./package.json ./
COPY \
  --chown=node:node \
  ./yarn.lock ./
RUN \
  yarn install \
    --pure-lockfile

ENTRYPOINT ["/usr/bin/dumb-init", "--"]
