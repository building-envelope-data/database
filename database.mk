#!/usr/bin/env -S make --file
SELF := $(lastword $(MAKEFILE_LIST))

include ./.env

SHELL := /usr/bin/env bash
.SHELLFLAGS := -o errexit -o errtrace -o nounset -o pipefail -c
MAKEFLAGS += --warn-undefined-variables

COMPOSE_BAKE=true

dump_archive_name = postgresql_dumpall.gz

# Taken from https://www.client9.com/self-documenting-makefiles/
help : ## Print this help
	@awk -F ':.*?## ' '/^[^\t].+?:.*?##/ {\
		printf "\033[36m%-30s\033[0m %s\n", $$1, $$NF \
	}' $(MAKEFILE_LIST)
.PHONY : help
.DEFAULT_GOAL := help

psql : ## Enter PostgreSQL interactive terminal in the `database` container
	docker compose up \
		--no-build \
		--no-recreate \
		--wait \
		database
	docker compose exec \
		database \
		psql \
		--username="${POSTGRES_USER}" \
		--dbname="${POSTGRES_DATABASE_NAME}"
.PHONY : psql

remove-volume : ## Remove data and files volumes
	docker volume rm \
		"${NAME}_${ENVIRONMENT}_data"
	docker volume rm \
		"${NAME}_${ENVIRONMENT}_files"
.PHONY : remove-volume

create : ## Create database with name `${POSTGRES_DATABASE_NAME}`
	docker compose up \
		--no-build \
		--no-recreate \
		--wait \
		database
	docker compose exec \
		--no-tty \
		database \
		createdb \
			--username="${POSTGRES_USER}" \
			"${POSTGRES_DATABASE_NAME}"
.PHONY : create

drop : ## Drop database with name `${POSTGRES_DATABASE_NAME}`
	docker compose up \
		--no-build \
		--no-recreate \
		--wait \
		database
	docker compose exec \
		--no-tty \
		database \
		dropdb \
			--username="${POSTGRES_USER}" \
			"${POSTGRES_DATABASE_NAME}"
.PHONY : drop

sql : ## Run the SQL script in the file `${SCRIPT}` in the database service, for example, `make sql SCRIPT=./my.sql ` (note that after database schema changes it is necessary to restart the backend service for the object-relational mapper Npgsql to work seamlessly, for example, by restarting the backend service with `./docker.mk restart SERVICE=backend`)
	docker compose up \
		--no-build \
		--no-recreate \
		--wait \
		database
	cat "${SCRIPT}" \
	| docker compose exec \
		--no-tty \
		database \
		psql \
			--echo-all \
			--no-psqlrc \
			--set=ON_ERROR_STOP=on \
			--file=- \
			--username="${POSTGRES_USER}" \
			--dbname="${POSTGRES_DATABASE_NAME}"
.PHONY : sql

migrate : SCRIPT = ./backend/src/Migrations/migrate.sql
migrate : ## Migrate database by running the idempotent SQL script ./backend/src/Migrations/migrate.sql
	$(MAKE) --file="${SELF}" sql SCRIPT="${SCRIPT}"
	docker compose restart \
		--no-deps \
		backend
.PHONY : migrate

# Backup with `pg_dump`: https://www.postgresql.org/docs/current/backup-dump.html
# Command `pg_dump`: https://www.postgresql.org/docs/current/app-pgdump.html
# Backup files with `tar` and `gzip` as suggested in https://docs.docker.com/storage/volumes/#backup-restore-or-migrate-data-volumes
# We could have used `docker cp` as explained in https://docs.docker.com/engine/reference/commandline/cp/
backup : DIR = ./backup
backup : CONTENT_ADDRESSABLE_STORAGE = ./.content-addressable-storage
backup : ## Backup database and related data to directory with absolute path `${DIR}` storing files across multiple backups content-addressed in the directory `${CONTENT_ADDRESSABLE_STORAGE}`, for example, `./database.mk backup DIR=/app/data/backups/$(date +"%Y-%m-%d_%H_%M_%S") CONTENT_ADDRESSABLE_STORAGE=/app/data/backups/.content-addressable-storage`
	mkdir --parents \
		"${DIR}/files" \
		"${CONTENT_ADDRESSABLE_STORAGE}"
	docker compose up \
		--no-build \
		--no-recreate \
		--wait \
		database
	docker compose exec \
		--no-tty \
		database \
		pg_dump \
			--clean \
			--if-exists \
			--username="${POSTGRES_USER}" \
			--dbname="${POSTGRES_DATABASE_NAME}" \
		| gzip \
		> "${DIR}/${dump_archive_name}"
	docker compose run \
		--rm \
		--no-deps \
		--no-TTY \
		--volume "${DIR}":/backup \
		--volume "${CONTENT_ADDRESSABLE_STORAGE}":/.content-addressable-storage \
		backend \
		bash -o errexit -o errtrace -o nounset -o pipefail -c ' \
			if [[ "${ENVIRONMENT}" == "development" ]]; then \
				cd /app/src/files ; \
			else \
				cd /app/files ; \
			fi ; \
			find . -type f -printf "%f\n" | while read -r original_file_name; do \
				echo "Backing up file: $${original_file_name}" ; \
				hash=$$(sha256sum ./"$${original_file_name}" | cut --delimiter=" " --fields=1) ; \
				content_addressed_file_path="/.content-addressable-storage/$${hash}" ; \
				if [[ ! -f "$${content_addressed_file_path}" ]]; then \
						cp "$${original_file_name}" "$${content_addressed_file_path}" ; \
						echo "Stored new content-addressed file: $${content_addressed_file_path}" ; \
				else \
						echo "File already exists in content-addressable storage: $${content_addressed_file_path}" ; \
				fi ; \
				ln --symbolic "$${content_addressed_file_path}" /backup/files/"$${original_file_name}" ; \
			done \
		'
.PHONY : backup

restore : DIR = ./backup
restore : CONTENT_ADDRESSABLE_STORAGE = ./.content-addressable-storage
restore : ## Restore database and related data from directory with absolute path `${DIR}` and content-addressable storage directory `${CONTENT_ADDRESSABLE_STORAGE}` (dropping and recreating the database and clearing related files before to start cleanly), for example, `./database.mk restore DIR=/app/data/backups/2021-04-22_15_43_35 CONTENT_ADDRESSABLE_STORAGE=/app/data/backups/.content-addressable-storage`
	docker compose stop \
		backend
	docker compose up \
		--no-build \
		--no-recreate \
		--wait \
		database
	-docker compose exec \
		--no-tty \
		database \
		dropdb \
			--username="${POSTGRES_USER}" \
			"${POSTGRES_DATABASE_NAME}"
	docker compose exec \
		--no-tty \
		database \
		createdb \
			--username="${POSTGRES_USER}" \
			"${POSTGRES_DATABASE_NAME}"
	gunzip --stdout "${DIR}/${dump_archive_name}" \
	| docker compose exec \
		--no-tty \
		database \
		psql \
			--echo-all \
			--no-psqlrc \
			--set=ON_ERROR_STOP=on \
			--file=- \
			--username="${POSTGRES_USER}" \
			--dbname="${POSTGRES_DATABASE_NAME}"
	docker compose run \
		--rm \
		--no-deps \
		--no-TTY \
		--volume "${DIR}":/backup \
		--volume "${CONTENT_ADDRESSABLE_STORAGE}":/.content-addressable-storage \
		backend \
		bash -o errexit -o errtrace -o nounset -o pipefail -c ' \
			if [[ "${ENVIRONMENT}" == "development" ]]; then \
				cd /app/src/files ; \
			else \
				cd /app/files ; \
			fi ; \
			rm \
				--recursive \
				--force \
				--dir \
				* ; \
			find /backup/files -type l -printf "%f\n" | while read -r backup_symlink_name; do \
				echo "Restoring file: $${backup_symlink_name}" from content-addressable storage $$(readlink --canonicalize /backup/files/$${backup_symlink_name}) ; \
				cp /backup/files/"$${backup_symlink_name}" ./"$${backup_symlink_name}" ; \
			done \
		'
	docker compose start \
		backend
.PHONY : restore
