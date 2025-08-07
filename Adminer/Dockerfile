FROM adminer:latest

USER root

# Install dependencies on Alpine
RUN apk --no-cache add \
      bash \
      libaio \
      libnsl \
      libc6-compat \
      unzip \
      zip \
      build-base \
      autoconf \
      libzip-dev \
      zlib-dev \
      curl \
  && ln -s /lib/libc.so.6 /usr/lib/libresolv.so.2

# Copy and run zip2instantclient.sh script
COPY oracle-instantclient/ /tmp/oracle
WORKDIR /tmp/oracle
RUN chmod +x zip2instantclient.sh \
  && bash zip2instantclient.sh 

# Move Instant Client and clean up
RUN mkdir -p /opt/oracle \
  && mv instantclient /opt/oracle/instantclient \
  && rm -rf /tmp/oracle

# Configure the library path
RUN mkdir -p /etc/ld.so.conf.d \
  && printf "/opt/oracle/instantclient\n" > /etc/ld.so.conf.d/oracle-instantclient.conf

# Build and enable OCI8 and zip extensions
RUN printf "instantclient,/opt/oracle/instantclient" | pecl install oci8 \
 && docker-php-ext-enable oci8 \
 && docker-php-ext-install zip

# Switch back to non-root user
USER adminer
