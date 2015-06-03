var data = {};

exports.get = function(id, key) {
    if(!data[id]) {
        return;
    }

    return data[id][key];
}

exports.getAll = function(id) {
    return data[id];
}

exports.replace = function(id, metadata) {
    data[id] = metadata;
}

exports.set = function(id, key, value) {
    if(!data[id]) {
        data[id] = {};
    }

    data[id][key] = value;
}
