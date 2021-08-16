#!/bin/bash

if [ $# -ne 2 ]
then
	echo "Usage: $0 <src dir> <dst dir>"
	exit 1
fi

SRC_DIR=${1}
DST_DIR=${2}

if [ -d ${SRC_DIR} ]
then
	:
else
	echo "ERROR: can not find dir: ${SRC_DIR}"
	exit 1
fi

if [ -d ${DST_DIR} ]
then
	:
else
	echo "ERROR: can not find dir: ${DST_DIR}"
	exit 1
fi

if [ ${DST_DIR} = "/" ]
then
	echo "ERROR: can not set root dir: ${DST_DIR}"
	exit 1
fi

CURR_DIR=`pwd`

cd ${SRC_DIR}
find . -type d | grep -v "\.vs" > ${CURR_DIR}/dir.txt
find . -name "*.cs" 		>  ${CURR_DIR}/file.txt
find . -name "*.meta" 	>> ${CURR_DIR}/file.txt
cd ${CURR_DIR}

#copy dir
for i in `cat dir.txt`
do
	DST=${DST_DIR}/${i}
	if [ -d ${DST} ]
	then
		:
	else
		mkdir -p ${DST}
	fi
done

#copy files
for i in `cat file.txt`
do
	SRC_FILE=${SRC_DIR}/${i}
	DST_FILE=${DST_DIR}/${i}
	cp ${SRC_FILE} ${DST_FILE}
done

rm -f file.txt
rm -f dir.txt
