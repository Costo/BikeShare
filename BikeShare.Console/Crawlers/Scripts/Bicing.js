var http = require('http'),
   vm = require('vm');

var markers = [];
var latlong;

http.get({ host: 'www.bicing.cat', path: '/localizaciones/localizaciones.php'}, function(res) {
   //console.log("statusCode: ", res.statusCode);
   //console.log("headers: ", res.headers);
   var data = [];
   res.on('data', function(d) {
      data.push(d);
   });
   res.on('end', function() {
      var pattern =/<script[^>]*>([\s\S]*?)<\/script>/ig, 
         m;
      while (m = pattern.exec(data.join('')) ) {
         if( m[1].indexOf( 'GBrowserIsCompatible' ) > -1 )
         {
            sandbox = {
               GBrowserIsCompatible: function(){ return true; },
               GLatLng: function(lat, long){ this.lat = lat; this.long = long; },
               GIcon: function(){},
               GSize: function(){},
               GPoint: function(){},
               GEvent: {
                  addListener: function(){
                     // execute handler immediately
                     arguments[2]();
                  }
               },
               GMarker: fakeGMarker,
               GMap2: function(){
                  this.addControl = function(){};
                  this.enableScrollWheelZoom = function(){};
                  this.setCenter = function(){};
                  this.addOverlay = function(){};
               },
               GLargeMapControl: function(){},
               GMapTypeControl: function(){},
               document: {
                  getElementById:function(){}
               },
               $: {
                  ajax: fakeAjax
               }
            };
            vm.runInNewContext(m[1], sandbox);
            break;
         }
      }
    
      sandbox.load();
      var strMarkers = [];
      for(var i in markers) {
         strMarkers[i] = JSON.stringify(markers[i]);
      }
      process.stdout.write('[' + strMarkers.join( ', ' ) + ']');
   });
}).on('error', function(e) {
   console.error(e);
});

function fakeGMarker(point) {
   markers.push(point);  
}

function fakeAjax(options)
{
   markers[markers.length - 1].data = options.data;
}