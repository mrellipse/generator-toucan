'use strict';
const Generator = require('yeoman-generator');
const chalk = require('chalk');
const yosay = require('yosay');
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

let validate = {

  alphanumeric: function (value) {
    if (!value || value.match(/[^\d\w]/)) {
      return chalk.red('Must be alphanumeric')
    } else {
      return true;
    }
  },
  required: function (value) {
    return value && value.length > 0 ? true : "Value is required";
  },

  positiveInteger: function (value) {
    if (!isNaN(parseInt(value)) && parseInt(value) > 0) {
      return true;
    } else {
      return "Must be a number and greater than zero"
    }
  }

};

module.exports = class extends Generator {

  constructor(name, config) {
    super(name, config);
  }

  prompting() {

    this.log(yosay('Yo Toucan'));

    const prompts = [{
      type: 'input',
      name: 'name',
      message: 'Your application name',
      default: this.appname,
      validate: validate.required
    },
    {
      type: 'input',
      name: 'assemblyName',
      message: 'Your assembly name (one short alphanumeric word)',
      default: this.appname,
      validate: validate.alphanumeric
    },
    {
      type: 'input',
      name: 'port',
      message: 'Local port for Kestrel http server',
      default: 5000,
      validate: validate.positiveInteger
    },
    {
      type: 'list',
      name: 'dbProvider',
      message: 'Database provider',
      choices: [{
        name: 'pgsql',
        value: 'npgsql'
      }, {
        name: 'mssql',
        value: 'mssql'
      }],
      validate: validate.required
    },
    {
      type: 'input',
      name: 'dbHost',
      message: 'Host name',
      default: function (answers) {
        return answers.dbProvider === 'npgsql' ? '127.0.0.1' : '.';
      },
      validate: validate.required
    },
    {
      when: function (answers) {
        return answers.dbProvider === 'npgsql';
      },
      type: 'input',
      name: 'dbPort',
      message: 'Port',
      default: function (answers) {
        return 5432;
      },
      validate: validate.positiveInteger
    },
    {
      type: 'input',
      name: 'dbName',
      message: 'Database Name',
      default: function (answers) {
        let name = answers.assemblyName.substring(0, 1).toUpperCase() + answers.assemblyName.substring(1)
        return name + 'Dev';
      },
      validate: validate.alphanumeric
    }, {
      when: function (answers) {
        return answers.dbProvider === 'mssql' && !answers.dbTrustedConnection;
      },
      type: 'confirm',
      name: 'dbTrustedConnection',
      message: 'Use trusted connection ?',
      default: true
    }, {
      when: function (answers) {
        return (answers.dbProvider === 'npgsql' && !answers.dbUser) || (answers.dbProvider === 'mssql' && !answers.dbTrustedConnection && !answers.dbUser);
      },
      type: 'input',
      name: 'dbUser',
      message: 'Username',
      validate: validate.required
    }, {
      when: function (answers) {
        return (answers.dbProvider === 'npgsql' && !answers.dbPassword) || (answers.dbProvider === 'mssql' && !answers.dbTrustedConnection && !answers.dbPassword);
      },
      type: 'input',
      name: 'dbPassword',
      message: 'Password',
      validate: validate.required
    }];

    return this.prompt(prompts).then(answers => {

      if (!answers.assemblyName)
        answers.assemblyName = answers.name;

      answers.assemblyName = answers.assemblyName.substring(0, 1).toUpperCase() + answers.assemblyName.substring(1)

      let authSegment = null;

      if (answers.dbProvider === 'npgsql') {
        authSegment = 'Username=' + answers.dbUser + ';Password=' + answers.dbPassword;
      } else if (answers.dbProvider === 'mssql' && !answers.dbTrustedConnection) {
        authSegment = 'User Id=' + answers.dbUser + ';Password=' + answers.dbPassword;
      } else {
        authSegment = 'Trusted_Connection=True';
      }

      answers.dbProviderString = answers.dbProvider === 'npgsql' ?
        'Host=' + answers.dbHost + ';Port=' + answers.dbPort + ';Database=' + answers.dbName + ';' + authSegment + ';' :
        'Server=' + answers.dbHost + ';Database=' + answers.dbName + ';' + authSegment + ';'

      answers.dbContextName = answers.dbProvider === 'npgsql' ? 'NpgSqlContext' : 'MsSqlContext';
      answers.runPostInstall = false;

      this.log(yosay('Make it so number one! Send a priority away team down to the surface.'));

      this.props = answers;
    });
  }

  writing() {

    this.writeRoot();
    this.writeSource();
  }

  writeRoot() {
    this.fs.copy(
      this.templatePath('_.gitignore'),
      this.destinationPath('.gitignore')
    );

    this.fs.copy(
      this.templatePath('_global.json'),
      this.destinationPath('global.json')
    );

    this.fs.copyTpl(
      this.templatePath('_package.json'),
      this.destinationPath('package.json'),
      this.props
    );

    this.fs.copyTpl(
      this.templatePath('_README.md'),
      this.destinationPath('README.md'),
      this.props
    );
  }

  writeSource() {
    let _this = this;
    let basePath = this.templatePath();

    let staticResourcePaths = [

    ];

    let ignoreResourcePaths = [];

    let ignoreExtensions = [];

    let ignoreNpgSql = [
      '_src\\_data\\_Context\\_NpgSqlContextFactory.cs',
      '_src\\_data\\_Context\\_NpgSqlContext.cs',
      '_src\\_data\\_npgsql.json'
    ];

    let ignoreMsSql = [
      '_src\\_data\\_Context\\_MsSqlContextFactory.cs',
      '_src\\_data\\_Context\\_MsSqlContext.cs',
      '_src\\_data\\_mssql.json'
    ];

    if (this.props.dbProvider == 'npgsql') {
      ignoreResourcePaths = ignoreResourcePaths.concat(ignoreMsSql);
    }

    if (this.props.dbProvider == 'mssql') {
      ignoreResourcePaths = ignoreResourcePaths.concat(ignoreNpgSql);
    }

    let staticResourceExtensions = [
      '.png',
      '.ttf',
      '.ejs'
    ];

    let isTargetResource = (filePath) => {
      let value = true;

      ignoreResourcePaths.forEach((o) => {
        if (value && filePath.indexOf(o) > -1) {
          value = false;
        }
      });

      if (value) {
        ignoreExtensions.forEach((o) => {
          if (value && filePath.endsWith(o)) {
            value = false;
          }
        });
      }

      return value;
    }

    let isStaticResource = (filePath) => {
      let value = false;

      staticResourcePaths.forEach((o) => {
        if (!value && filePath.indexOf(o) > -1) {
          value = true;
        }
      });

      if (!value) {
        staticResourceExtensions.forEach((o) => {
          if (!value && filePath.endsWith(o)) {
            value = true;
          }
        });
      }

      return value;
    }

    let fn = (filePath, stat) => {

      let relPath = filePath.split(/\\_/).join('\\').replace(basePath, '').substring(1);

      if (isTargetResource(filePath)) {

        if (isStaticResource(filePath)) {
          _this.fs.copy(
            _this.templatePath(filePath),
            _this.destinationPath(relPath)
          );
        }
        else {
          _this.fs.copyTpl(
            _this.templatePath(filePath),
            _this.destinationPath(relPath),
            _this.props
          );
        }
      }
    };

    walkSync(this.templatePath('_.vscode'), fn);
    walkSync(this.templatePath('_build'), fn);
    walkSync(this.templatePath('_test'), fn);
    walkSync(this.templatePath('_src'), fn);
  }

  install() {
    let _this = this;
    this.installDependencies({
      bower: false,
      npm: true,
      callback: function () {
        _this.log(yosay('Away team assembled! To start shuttle run \'npm start\''));
      },
      skipMessage: true
    });
  }
};
