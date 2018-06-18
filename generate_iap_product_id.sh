#! /bin/bash
if [ ! -n "$1" ] ;then
    echo "you need to input a app bundle id!"
else
    echo 'Member IAP Product Id:'
    echo ''
    echo "1-month Premium:  $1.iap.member.2.2592000"
    echo "2-month Premium:  $1.iap.member.2.5184000"
    echo "3-month Premium:  $1.iap.member.2.7776000"
    echo "6-month Premium:  $1.iap.member.2.15552000"
    echo "12-month Premium: $1.iap.member.2.31104000"
    echo ''
    echo "1-month Advanced:  $1.iap.member.3.2592000"
    echo "2-month Advanced:  $1.iap.member.3.5184000"
    echo "3-month Advanced:  $1.iap.member.3.7776000"
    echo "6-month Advanced:  $1.iap.member.3.15552000"
    echo "12-month Advanced: $1.iap.member.3.31104000"
    echo ''
fi