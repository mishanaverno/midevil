#!/bin/bash

type=$1
name=$2
if [[ -z $name ]]; then
echo "No name Specified"
exit 0;
fi

PATH="Resources/Data/Equip/"
ARMOR=${PATH}"Templates/Armor.json"
WEAPON=${PATH}"Templates/Weapon.json"

case "$1" in
# armor
-a) echo "Creating Armor"
FROM=$ARMOR
TO=${PATH}"Armor/Body/"${name}".json"
;;
-h) echo "Creating Helm"
FROM=$ARMOR
TO=${PATH}"Armor/Head/"${name}".json"
;;
-s) echo "Creating Shield"
FROM=$ARMOR
TO=${PATH}"Armor/Shield/"${name}".json"
;;
# weapon 
-ohm) echo "Creating One Hand Melee"
FROM=$ARMOR
TO=${PATH}"Weapon/OneHandMelee/"${name}".json"
;;
*) 
echo "$1 is not an option"
echo "USAGE sh script [-a,h,s,ohm] name"
;;
esac
echo "Template: "$FROM
echo "New data JSON: "$TO
/usr/bin/cp ${FROM} ${TO}