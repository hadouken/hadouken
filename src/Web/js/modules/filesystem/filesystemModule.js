angular.module('hadouken.filesystem', [
    'hadouken.jsonrpc'
])
.directive('directoryBrowser', [ '$document', 'jsonrpc', function($document, jsonrpc) {
    var browser = '<div class="directoryBrowser">' + 
                    '<div class="panel panel-default">' +
                      '<div class="panel-body">' +
                        '<i class="fa fa-spin fa-spinner drive-loader"></i>' +
                        '<select class="form-control drive-list"></select>' +
                      '</div>' +
                      '<div class="list-group">' +
                      '</div>' + 
                    '</div>' +
                  '</div>';

    var parentItem = '<a href="javascript: void(0);" class="list-group-item"><i class="fa fa-chevron-up"></i></a>';
    var item = '<a href="javascript: void(0);" class="list-group-item"><i class="fa fa-folder"></i> <span class="text"></span></a>';

    var originalDirectory = '';

    function bytes(bytes, precision) {
        if (isNaN(parseFloat(bytes)) || !isFinite(bytes) || bytes <= 0) return '-';
        if (typeof precision === 'undefined') precision = 1;
        var units = ['bytes', 'kB', 'MB', 'GB', 'TB', 'PB'],
            number = Math.floor(Math.log(bytes) / Math.log(1024));
        return (bytes / Math.pow(1024, Math.floor(number))).toFixed(precision) +  ' ' + units[number];
    }

    function $anyParentHasClass(element, classname) {
        if (element.className && element.className.split(' ').indexOf(classname)>=0) return true;
        return element.parentNode && $anyParentHasClass(element.parentNode, classname);
    }

    function getDirectories(selected, cb) {
        jsonrpc.request('fileSystem.getDirectories', {
            params: [selected],
            success: function(d) {
                cb(d.result);
            }
        });
    }

    function getDrives(cb) {
        jsonrpc.request('fileSystem.getDrives', {
            params: [],
            success: function(d) {
                cb(d.result);
            }
        });
    }

    function getParent(selected, cb) {
        jsonrpc.request('fileSystem.getParent', {
            params: [selected],
            success: function(d) {
                cb(d.result);
            }
        });
    }

    // Go up one level
    function up($scope, element, currentDirectory) {
        getParent(currentDirectory, function(parent) {
            getDirectories(parent, function(directories) {
                render($scope, element, parent, directories);
            });
        });
    }

    function down($scope, element, currentDirectory) {
        getDirectories(currentDirectory, function(directories) {
            render($scope, element, currentDirectory, directories);
        });
    }

    function render($scope, element, currentDirectory, directories) {
        element.find('.list-group-item').remove();

        var parent = angular.element(parentItem);
        parent.bind('click', function(e) {
            e.preventDefault();
            up($scope, element, currentDirectory);
        });

        element.find('.list-group').append(parent);

        for(var i = 0; i < directories.length; i++) {

            (function(directory) {
                var el = angular.element(item);
                var parts = directory.split('/');

                el.find('.text').text(parts[parts.length - 1]);
                el.bind('click', function(e) {
                    e.preventDefault();
                    down($scope, element, directory);
                });

                element.find('.list-group').append(el);
            })(directories[i]);
        }

        $scope.directory = currentDirectory.replace(/\//g, '\\');

        if($scope.directory.split('\\').filter(function(e){return e;}).length <= 1) {
            parent.hide();
        }
    }

    return {
        scope: {
            directory: '='
        },
        link: function($scope, element, attrs) {
            $document.bind('click', function(e) {
                if(e.target === element[0]
                    || $anyParentHasClass(e.target, 'directoryBrowser')) {
                    return;
                }
                
                element.parent().find('.directoryBrowser').remove();
            });

            element.bind('click', function() {
                originalDirectory = $scope.directory;

                var container = angular.element(browser);
                var driveList = container.find('.drive-list');

                container.width(element.outerWidth());
                driveList.hide();

                element.after(container);

                getDrives(function(drives) {
                    for(var i = 0; i < drives.length; i++) {
                        var drive = drives[i];
                        var opt = angular.element('<option></option>');
                        
                        opt.val(drive.Name);
                        opt.text(drive.Name + ' (' + bytes(drive.TotalFreeSpace) + ' free)');

                        driveList.append(opt);
                    }

                    container.find('.drive-loader').hide();
                    driveList.show();
                    
                    // On drive change
                    driveList.bind('change', function(e) {
                        var drive = driveList.val();

                        getDirectories(drive, function(result) {
                            render($scope, container, drive, result);
                        });
                    });
                });

                getDirectories(originalDirectory, function(result) {
                    render($scope, container, originalDirectory, result);
                });
            });
        }
    }
}]);