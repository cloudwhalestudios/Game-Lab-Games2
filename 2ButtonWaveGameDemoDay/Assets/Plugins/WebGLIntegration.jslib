mergeInto(LibraryManager.library, {

    Redirect: function(str_location) {
        window.location.href = encodeURI(Pointer_stringify(str_location));
    },

    RedirectWithParams: function(str_location, str_paramsJson) {
        var jsonObject = JSON.parse(Pointer_stringify(str_paramsJson));
        var location = encodeURI(Pointer_stringify(str_location) + '?');
        console.log("received location: " + location + " | received json: " + JSON.stringify(jsonObject, null, 4));

        var url = Object.keys(jsonObject).map(function(k) {
            return encodeURIComponent(k) + '=' + encodeURIComponent(jsonObject[k])
        }).join('&');

        console.log("Redirecting to " . location  + "?" . url);
        window.location.href = encodeURI(location + url);
    },

    Refresh: function() {
        location.reload();
    },

    GetParams: function () {
	var jsonString = "";
		if (location.search != "") {
			var search = location.search.substring(1);
			if (search != "") {
				var jsonObject = JSON.parse('{"' + search.replace(/&/g, '","').replace(/=/g,'":"') + '"}', function(key, value) { return key===""?value:decodeURIComponent(value) });
				var jsonString = JSON.stringify(jsonObject);
			}
		}
        console.log("Read parameters as json:" + jsonString);

        var bufferSize = lengthBytesUTF8(jsonString) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(jsonString, buffer, bufferSize);

        console.log("Parameter buffer:" + buffer);

        return buffer;
		
    },

    SetParams: function(str_paramsJson) {
        var jsonObject = JSON.parse(Pointer_stringify(str_paramsJson));
        var searchString = window.location.search = "";
        var questionmark = false;
        for(var key in jsonObject) {
            if (jsonObject.hasOwnProperty(key)) {
                // Attach to url
                if (!questionmark) {
                    searchString += "?";
                }
                else {
                    searchString += "&";
                }
                searchString += key + "=" + jsonObject[key];
            }
        }
        window.location.search = searchString;
    },

    Crash: function() {
        window.alert("This should totally be crashing right now.");
    },

    Hello: function () {
        window.alert("Hello, world!");
    },

    HelloString: function (str) {
        window.alert(Pointer_stringify(str));
    },

    PrintFloatArray: function (array, size) {
        for(var i = 0; i < size; i++)
            console.log(HEAPF32[(array >> 2) + i]);
    },

    AddNumbers: function (x, y) {
        return x + y;
    },

    StringReturnValueFunction: function () {
        var returnStr = "bla";
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },

    BindWebGLTexture: function (texture) {
        GLctx.bindTexture(GLctx.TEXTURE_2D, GL.textures[texture]);
    },

});