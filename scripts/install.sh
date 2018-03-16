#!/bin/sh

DEPLOY_DIR=/var/www/webapp
APP_NAME=FoxValleyMeetup

# CDN_HOSTS="https://ssl.google-analytics.com https://assets.zendesk.com https://s-static.ak.facebook.com  https://connect.facebook.net https://fonts.googleapis.com https://themes.googleusercontent.com https://tautt.zendesk.com https://cdn.jsdelivr.net https://maxcdn.bootstrapcdn.com https://code.jquery.com https://cdnjs.cloudflare.com"

# create a new user
sudo useradd -m deploy
sudo usermod -aG sudo deploy

# install common libraries
sudo apt-get update
sudo apt-get -y upgrade
sudo apt-get install nano -y
sudo apt-get install git -y
sudo apt-get install sqlite3 -y
sudo apt-get install libsqlite3-dev -y
sudo apt-get install ufw -y
sudo apt-get install python-pip -y

# configure firewall
sudo ufw default deny incoming
sudo ufw default allow outgoing
sudo ufw allow ssh
sudo ufw --force enable

# upgrade pip
pip install --upgrade pip
pip install setuptools

# install nvm, nodejs v8, and some global libs
curl https://raw.githubusercontent.com/creationix/nvm/v0.33.8/install.sh | bash
export NVM_DIR="$HOME/.nvm"
[ -s "$NVM_DIR/nvm.sh" ] && \. "$NVM_DIR/nvm.sh"  # This loads nvm
[ -s "$NVM_DIR/bash_completion" ] && \. "$NVM_DIR/bash_completion"  # This loads nvm bash_completion
nvm install v8.9.4
nvm alias default v8.9.4
nvm use default
npm i -g rimraf dotenv pm2 forever nodemon

# install dotnet 2.1.4
sudo apt-get install libunwind8 -y
sudo apt-get install liblttng-ust0 -y
sudo apt-get install libcurl3 -y
sudo apt-get install libssl1.0.0 -y
sudo apt-get install libuuid1 -y
sudo apt-get install libkrb5 -y
sudo apt-get install zlib1g -y
sudo apt-get install libicu55 -y
sudo apt-get install curl -y
sudo apt-get install gettext -y
curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg
sudo mv microsoft.gpg /etc/apt/trusted.gpg.d/microsoft.gpg
sudo sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/microsoft-ubuntu-xenial-prod xenial main" > /etc/apt/sources.list.d/dotnetdev.list'
sudo apt-get update
# sudo apt-get install dotnet-sdk-2.0.3 -y
sudo apt-get install dotnet-sdk-2.1.4 -y

# install nginx, self-signed cert and reverse-proxy to localhost:5000
sudo apt-get install nginx -y
sudo systemctl start nginx
sudo systemctl enable nginx
sudo ufw allow 'Nginx HTTP'
sudo ufw allow 'Nginx HTTPS'
sudo ufw reload
pip install ngxtop

# create self-signed cert (valid for more than 5 years, why not), because we are running
# dhparam, this will take a while!!
publicip="$(dig +short myip.opendns.com @resolver1.opendns.com)"
sudo openssl req -x509 -nodes -days 2000 -newkey rsa:4096 -keyout /etc/ssl/private/nginx-selfsigned.key -out /etc/ssl/certs/nginx-selfsigned.crt -subj /C=US/ST=Illinois/L=Chicago/O=Startup/CN=$publicip
sudo openssl dhparam -out /etc/ssl/certs/dhparam.pem 4096 > /dev/null 2>&1
cat >/etc/nginx/snippets/self-signed.conf <<EOL
ssl_certificate /etc/ssl/certs/nginx-selfsigned.crt;
ssl_certificate_key /etc/ssl/private/nginx-selfsigned.key;
EOL

cat >/etc/nginx/snippets/ssl-params.conf <<EOL
# from https://cipherli.st/ and https://raymii.org/s/tutorials/Strong_SSL_Security_On_nginx.html
ssl_protocols TLSv1.2;
ssl_prefer_server_ciphers on;
ssl_ciphers "EECDH+AESGCM:EDH+AESGCM:AES256+EECDH:AES256+EDH";
ssl_ecdh_curve secp384r1;
ssl_session_cache shared:SSL:10m;
ssl_session_tickets off;
ssl_stapling on;
ssl_stapling_verify on;
resolver 8.8.8.8 8.8.4.4 valid=300s;
resolver_timeout 5s;
add_header Strict-Transport-Security "max-age=63072000; includeSubdomains";
add_header X-Frame-Options DENY;
add_header X-Content-Type-Options nosniff;
ssl_dhparam /etc/ssl/certs/dhparam.pem;
EOL

# configure nginx
# NOTE: if you plan to host multiple websites on the server, you'll want to create
#       a separate file /etc/nginx/sites-available and map a server_name property
#       on the server object to a domain, then symlink the file to sites-enabled
#       ln -s /etc/nginx/sites-available/newfilename /etc/nginx/sites-enabled/newfilename
#
#       the following is for 1 site deployed to this VM as the default site for
#       the given VM public ip
cat >/etc/nginx/sites-available/default <<EOL
server {
    listen 80 default_server;
    listen [::]:80 default_server;
    listen 443 ssl http2 default_server;
    listen [::]:443 ssl http2 default_server;
    server_tokens off;
    server_name $publicip;
    include snippets/self-signed.conf;
    include snippets/ssl-params.conf;
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \$host;
        proxy_cache_bypass \$http_upgrade;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
        proxy_buffering off;
        proxy_ignore_client_abort off;
        proxy_intercept_errors on;
        proxy_pass_request_headers on;

        proxy_hide_header X-Content-Type-Options;

        # The following are needed for a perfect security score
        # get a grade A in security at https://securityheaders.io
        add_header X-Frame-Options SAMEORIGIN;
        #add_header X-Content-Type-Options nosniff;
        add_header X-Content-Type-Options "" always;
        add_header X-XSS-Protection "1; mode=block";
        add_header Strict-Transport-Security "max-age=31536000; includeSubdomains; preload";
        # add_header Content-Security-Policy "default-src https: 'self' $CDN_HOSTS; script-src https: 'self' 'unsafe-inline' 'unsafe-eval' $CDN_HOSTS; img-src https: 'self' $CDN_HOSTS; style-src 'self' 'unsafe-inline' $CDN_HOSTS; font-src https: 'self' $CDN_HOSTS; frame-src $CDN_HOSTS; object-src 'none'";
        add_header Referrer-Policy "no-referrer";

        client_max_body_size 500m;
    }
}
EOL
sudo systemctl restart nginx
# or just reload: sudo service nginx reload

# Now create a default .net core app and deploy it with system.d
cd /home
mkdir app
cd app

dotnet new web -n $APP_NAME -o $APP_NAME
cd $APP_NAME
dotnet publish -c Release -o $DEPLOY_DIR

# create system.d service
cat >/etc/systemd/system/kestrel-webapp.service <<EOL
[Unit]
Description=WebApp Kestrel Service
[Service]
WorkingDirectory=$DEPLOY_DIR
ExecStart=/usr/bin/dotnet $DEPLOY_DIR/$APP_NAME.dll
Restart=always
RestartSec=10
SyslogIdentifier=dotnet-webapp
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
[Install]
WantedBy=multi-user.target
EOL

sudo systemctl enable kestrel-webapp.service
sudo systemctl start kestrel-webapp.service

# restart kestrel
# sudo systemctl try-restart kestrel-webapp
