#!/bin/sh
current_dir="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

source $current_dir/env.sh

dist_dir=$current_dir/../dist/
proj_dir=$current_dir/../src/$APP_NAME

dotnet publish $proj_dir/$APP_NAME.csproj -o $dist_dir -c Release -r linux-x64

ssh root@$REMOTE_IP "sudo systemctl stop kestrel-webapp"
rsync -avz -e 'ssh' $dist_dir root@$REMOTE_IP:/var/www/webapp
ssh root@$REMOTE_IP "sudo systemctl start kestrel-webapp"
