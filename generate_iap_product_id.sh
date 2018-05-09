#! /bin/bash
if [ ! -n "$1" ] ;then
    echo "you need to input a app bundle id!"
else
    echo 'Member IAP Product Id:'
    echo ''
    echo "Premium 1 month:  $1.iap.member.2.2592000"
    echo "Premium 2 month:  $1.iap.member.2.5184000"
    echo "Premium 3 month:  $1.iap.member.2.7776000"
    echo "Premium 6 month:  $1.iap.member.2.15552000"
    echo "Premium 12 month: $1.iap.member.2.31104000"
    echo ''
    echo "Advanced 1 month:  $1.iap.member.3.2592000"
    echo "Advanced 2 month:  $1.iap.member.3.5184000"
    echo "Advanced 3 month:  $1.iap.member.3.7776000"
    echo "Advanced 6 month:  $1.iap.member.3.15552000"
    echo "Advanced 12 month: $1.iap.member.3.31104000"
    echo ''
fi