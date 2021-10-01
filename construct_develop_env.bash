#!/bin/bash

set -x

if [ $# -ne 3 ]
then
	echo "Usage: $0 {DIR_FULLPATH OF HAKONIWA_ROS_SAMPLE} {ros1|ros2} {UNITY PROJECT NAME}"
	exit 1
fi
path=$1/$2/unity/$3

echo "$(cd "$(dirname "$0")" || exit; pwd)"/UnityScripts/Hakoniwa

mkdir -p "${HOME}/backup.hakoniwa-Unity-Package"
mv "$path/Assets/Scripts/Hakoniwa" "${HOME}/backup.hakoniwa-Unity-Package"
mv "$path/Assets/Scripts/Hakoniwa.meta" "${HOME}/backup.hakoniwa-Unity-Package"
ln -s "$(cd "$(dirname "$0")" || exit; pwd)"/UnityScripts/Hakoniwa "$path"/Assets/Scripts/Hakoniwa
