'use strict';
const fs = require('fs');
const path = require('path');

function walkSync(currentDirPath, callback) {
  // attrib: https://stackoverflow.com/questions/2727167/how-do-you-get-a-list-of-the-names-of-all-files-present-in-a-directory-in-node-j

  fs.readdirSync(currentDirPath).forEach(function (name) {
    var filePath = path.join(currentDirPath, name);
    var stat = fs.statSync(filePath);
    if (stat.isFile()) {
      callback(filePath, stat);
    } else if (stat.isDirectory()) {
      walkSync(filePath, callback);
    }
  });
}

let basePath = "C:\\coding\\generator-toucan\\generators\\app\\templates";

walkSync(basePath + "\\_src", (filePath, stat) => {
  let parts = filePath.split('\\');
  let fileName = parts[parts.length - 1];

  if (fileName.substring(1, 1) !== '_') {
    parts[parts.length - 1] = '_' + fileName;
    //fs.rename(filePath, parts.join('\\'))
  }
});
