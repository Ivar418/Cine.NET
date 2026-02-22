FROM mysql:8.4

# Copy init scripts from repo
COPY ansible/roles/webapi/files/db-init/init/ /docker-entrypoint-initdb.d/

RUN chmod -R 755 /docker-entrypoint-initdb.d