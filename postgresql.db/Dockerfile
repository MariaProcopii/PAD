# Use the official PostgreSQL image as the base image
FROM postgres:15

# Copy the custom script to initialize additional databases
COPY ./init-db.sh /docker-entrypoint-initdb.d/

# Make sure the script has executable permissions
RUN chmod +x /docker-entrypoint-initdb.d/init-db.sh
