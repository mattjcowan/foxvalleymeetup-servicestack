#!/bin/sh
current_dir="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

source $current_dir/env.sh

dist_dir=$current_dir/../dist/
proj_dir=$current_dir/../src/$APP_NAME

cd $proj_dir
export ASPNETCORE_ENVIRONMENT=Development # use `set` if on windows
dotnet watch run

