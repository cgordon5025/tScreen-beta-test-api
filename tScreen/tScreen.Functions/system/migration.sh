#!/bin/bash

SCRIPT_USER='tws_api_worker'
CURRENT_ARCHIVE=$(find /home/tws_api_worker/drop/*.zip | head -1)
unzip -o -j "$CURRENT_ARCHIVE" -d /home/$SCRIPT_USER/triggered/TwsApi.Webjobs

# Remove files 14 days or older
find /home/$SCRIPT_USER/drop/*.zip -mtime +14 -delete