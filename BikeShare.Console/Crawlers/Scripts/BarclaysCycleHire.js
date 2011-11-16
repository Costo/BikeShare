var http = require('http'),
   https = require('https'),
   vm = require('vm');

var markers = [];

https.get({ host: 'web.barclayscyclehire.tfl.gov.uk', path: '/maps'}, function(res) {
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
         if( m[1].indexOf( 'genateScript' ) > -1 )
         {
            sandbox = {
               map: {},
               imageInstalled: {},
               ShowInfoBulle: function() {},
               google: {
                  maps: {
                     LatLng: function() {},
                     Marker: Marker
                  }
               }
            };
            vm.runInNewContext(m[1], sandbox);
            break;
         }
      }
    
      sandbox.genateScript();
      process.stdout.write('[' + markers.join(', ') + ']');
   });
}).on('error', function(e) {
   console.error(e);
});

function Marker(data) {
   markers.push(JSON.stringify(data));
}