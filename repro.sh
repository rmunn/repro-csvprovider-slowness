#!/usr/bin/env bash

run() {
  if [ "$OS" != "Windows_NT" ]
  then
    mono "$@"
  else
    "$@"
  fi
}

./build.sh CopyBinaries

echo Now reproducing bug, should take about 10 minutes if bug is present...

run bin/ReproCsv/ReproCsv.exe
