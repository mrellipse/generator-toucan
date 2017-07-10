'use strict';
const fs = require('fs');
const path = require('path');

function hasChildDirectories(currentDirPath) {
  let value = false;
  fs.readdirSync(currentDirPath).forEach(function (name) {
    var filePath = path.join(currentDirPath, name);
    var stat = fs.statSync(filePath);
    if (stat.isDirectory()) {
      value = true;
    }
  });
  return value;
}

function walkSync(currentDirPath, callback) {
  // attrib: https://stackoverflow.com/questions/2727167/how-do-you-get-a-list-of-the-names-of-all-files-present-in-a-directory-in-node-j

  fs.readdirSync(currentDirPath).forEach(function (name) {
    var filePath = path.join(currentDirPath, name);
    var stat = fs.statSync(filePath);
    if (stat.isDirectory()) {
      if (hasChildDirectories(filePath)) {
        walkSync(filePath, callback);
      }
      callback(filePath, stat);
    }
  });
}

let basePath = "C:\\coding\\generator-toucan\\generators\\app\\templates";

walkSync(basePath + "\\_src", (filePath, stat) => {
  let parts = filePath.split('\\');
  let fileName = parts[parts.length - 1];

  if (fileName.substring(1, 1) !== '_') {
    parts[parts.length - 1] = '_' + fileName;
    //fs.renameSync(filePath, parts.join('\\'))
  }
});
