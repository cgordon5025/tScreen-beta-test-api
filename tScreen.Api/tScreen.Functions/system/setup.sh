#!/bin/bash

SCRIPT_USER='tws_api_worker'
SCRIPT_GROUP='workers'

sudo addgroup $SCRIPT_GROUP &> /dev/null
id -u $SCRIPT_USER &> /dev/null || sudo adduser -q --disabled-password --gecos "" --ingroup $SCRIPT_GROUP $SCRIPT_USER
sudo mkdir /home/$SCRIPT_USER/{drop,triggered,.ssh}
sudo chown -R $SCRIPT_USER:$SCRIPT_GROUP /home/$SCRIPT_USER/{drop,triggered,.ssh}
sudo mkdir /var/log/$SCRIPT_GROUP