(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-0347a1ce"],{"1f57":function(t,e,o){var r;(function(e,o){t.exports=o()})(0,function(){function t(t){this.mode=o.MODE_8BIT_BYTE,this.data=t,this.parsedData=[];for(var e=0,r=this.data.length;e<r;e++){var i=[],a=this.data.charCodeAt(e);a>65536?(i[0]=240|(1835008&a)>>>18,i[1]=128|(258048&a)>>>12,i[2]=128|(4032&a)>>>6,i[3]=128|63&a):a>2048?(i[0]=224|(61440&a)>>>12,i[1]=128|(4032&a)>>>6,i[2]=128|63&a):a>128?(i[0]=192|(1984&a)>>>6,i[1]=128|63&a):i[0]=a,this.parsedData.push(i)}this.parsedData=Array.prototype.concat.apply([],this.parsedData),this.parsedData.length!=this.data.length&&(this.parsedData.unshift(191),this.parsedData.unshift(187),this.parsedData.unshift(239))}function e(t,e){this.typeNumber=t,this.errorCorrectLevel=e,this.modules=null,this.moduleCount=0,this.dataCache=null,this.dataList=[]}t.prototype={getLength:function(t){return this.parsedData.length},write:function(t){for(var e=0,o=this.parsedData.length;e<o;e++)t.put(this.parsedData[e],8)}},e.prototype={addData:function(e){var o=new t(e);this.dataList.push(o),this.dataCache=null},isDark:function(t,e){if(t<0||this.moduleCount<=t||e<0||this.moduleCount<=e)throw new Error(t+","+e);return this.modules[t][e]},getModuleCount:function(){return this.moduleCount},make:function(){this.makeImpl(!1,this.getBestMaskPattern())},makeImpl:function(t,o){this.moduleCount=4*this.typeNumber+17,this.modules=new Array(this.moduleCount);for(var r=0;r<this.moduleCount;r++){this.modules[r]=new Array(this.moduleCount);for(var i=0;i<this.moduleCount;i++)this.modules[r][i]=null}this.setupPositionProbePattern(0,0),this.setupPositionProbePattern(this.moduleCount-7,0),this.setupPositionProbePattern(0,this.moduleCount-7),this.setupPositionAdjustPattern(),this.setupTimingPattern(),this.setupTypeInfo(t,o),this.typeNumber>=7&&this.setupTypeNumber(t),null==this.dataCache&&(this.dataCache=e.createData(this.typeNumber,this.errorCorrectLevel,this.dataList)),this.mapData(this.dataCache,o)},setupPositionProbePattern:function(t,e){for(var o=-1;o<=7;o++)if(!(t+o<=-1||this.moduleCount<=t+o))for(var r=-1;r<=7;r++)e+r<=-1||this.moduleCount<=e+r||(this.modules[t+o][e+r]=0<=o&&o<=6&&(0==r||6==r)||0<=r&&r<=6&&(0==o||6==o)||2<=o&&o<=4&&2<=r&&r<=4)},getBestMaskPattern:function(){for(var t=0,e=0,o=0;o<8;o++){this.makeImpl(!0,o);var r=n.getLostPoint(this);(0==o||t>r)&&(t=r,e=o)}return e},createMovieClip:function(t,e,o){var r=t.createEmptyMovieClip(e,o),i=1;this.make();for(var a=0;a<this.modules.length;a++)for(var n=a*i,s=0;s<this.modules[a].length;s++){var h=s*i,l=this.modules[a][s];l&&(r.beginFill(0,100),r.moveTo(h,n),r.lineTo(h+i,n),r.lineTo(h+i,n+i),r.lineTo(h,n+i),r.endFill())}return r},setupTimingPattern:function(){for(var t=8;t<this.moduleCount-8;t++)null==this.modules[t][6]&&(this.modules[t][6]=t%2==0);for(var e=8;e<this.moduleCount-8;e++)null==this.modules[6][e]&&(this.modules[6][e]=e%2==0)},setupPositionAdjustPattern:function(){for(var t=n.getPatternPosition(this.typeNumber),e=0;e<t.length;e++)for(var o=0;o<t.length;o++){var r=t[e],i=t[o];if(null==this.modules[r][i])for(var a=-2;a<=2;a++)for(var s=-2;s<=2;s++)this.modules[r+a][i+s]=-2==a||2==a||-2==s||2==s||0==a&&0==s}},setupTypeNumber:function(t){for(var e=n.getBCHTypeNumber(this.typeNumber),o=0;o<18;o++){var r=!t&&1==(e>>o&1);this.modules[Math.floor(o/3)][o%3+this.moduleCount-8-3]=r}for(o=0;o<18;o++){r=!t&&1==(e>>o&1);this.modules[o%3+this.moduleCount-8-3][Math.floor(o/3)]=r}},setupTypeInfo:function(t,e){for(var o=this.errorCorrectLevel<<3|e,r=n.getBCHTypeInfo(o),i=0;i<15;i++){var a=!t&&1==(r>>i&1);i<6?this.modules[i][8]=a:i<8?this.modules[i+1][8]=a:this.modules[this.moduleCount-15+i][8]=a}for(i=0;i<15;i++){a=!t&&1==(r>>i&1);i<8?this.modules[8][this.moduleCount-i-1]=a:i<9?this.modules[8][15-i-1+1]=a:this.modules[8][15-i-1]=a}this.modules[this.moduleCount-8][8]=!t},mapData:function(t,e){for(var o=-1,r=this.moduleCount-1,i=7,a=0,s=this.moduleCount-1;s>0;s-=2){6==s&&s--;while(1){for(var h=0;h<2;h++)if(null==this.modules[r][s-h]){var l=!1;a<t.length&&(l=1==(t[a]>>>i&1));var u=n.getMask(e,r,s-h);u&&(l=!l),this.modules[r][s-h]=l,i--,-1==i&&(a++,i=7)}if(r+=o,r<0||this.moduleCount<=r){r-=o,o=-o;break}}}}},e.PAD0=236,e.PAD1=17,e.createData=function(t,o,r){for(var i=u.getRSBlocks(t,o),a=new d,s=0;s<r.length;s++){var h=r[s];a.put(h.mode,4),a.put(h.getLength(),n.getLengthInBits(h.mode,t)),h.write(a)}var l=0;for(s=0;s<i.length;s++)l+=i[s].dataCount;if(a.getLengthInBits()>8*l)throw new Error("code length overflow. ("+a.getLengthInBits()+">"+8*l+")");a.getLengthInBits()+4<=8*l&&a.put(0,4);while(a.getLengthInBits()%8!=0)a.putBit(!1);while(1){if(a.getLengthInBits()>=8*l)break;if(a.put(e.PAD0,8),a.getLengthInBits()>=8*l)break;a.put(e.PAD1,8)}return e.createBytes(a,i)},e.createBytes=function(t,e){for(var o=0,r=0,i=0,a=new Array(e.length),s=new Array(e.length),h=0;h<e.length;h++){var u=e[h].dataCount,d=e[h].totalCount-u;r=Math.max(r,u),i=Math.max(i,d),a[h]=new Array(u);for(var c=0;c<a[h].length;c++)a[h][c]=255&t.buffer[c+o];o+=u;var f=n.getErrorCorrectPolynomial(d),g=new l(a[h],f.getLength()-1),p=g.mod(f);s[h]=new Array(f.getLength()-1);for(c=0;c<s[h].length;c++){var m=c+p.getLength()-s[h].length;s[h][c]=m>=0?p.get(m):0}}var v=0;for(c=0;c<e.length;c++)v+=e[c].totalCount;var _=new Array(v),C=0;for(c=0;c<r;c++)for(h=0;h<e.length;h++)c<a[h].length&&(_[C++]=a[h][c]);for(c=0;c<i;c++)for(h=0;h<e.length;h++)c<s[h].length&&(_[C++]=s[h][c]);return _};for(var o={MODE_NUMBER:1,MODE_ALPHA_NUM:2,MODE_8BIT_BYTE:4,MODE_KANJI:8},i={L:1,M:0,Q:3,H:2},a={PATTERN000:0,PATTERN001:1,PATTERN010:2,PATTERN011:3,PATTERN100:4,PATTERN101:5,PATTERN110:6,PATTERN111:7},n={PATTERN_POSITION_TABLE:[[],[6,18],[6,22],[6,26],[6,30],[6,34],[6,22,38],[6,24,42],[6,26,46],[6,28,50],[6,30,54],[6,32,58],[6,34,62],[6,26,46,66],[6,26,48,70],[6,26,50,74],[6,30,54,78],[6,30,56,82],[6,30,58,86],[6,34,62,90],[6,28,50,72,94],[6,26,50,74,98],[6,30,54,78,102],[6,28,54,80,106],[6,32,58,84,110],[6,30,58,86,114],[6,34,62,90,118],[6,26,50,74,98,122],[6,30,54,78,102,126],[6,26,52,78,104,130],[6,30,56,82,108,134],[6,34,60,86,112,138],[6,30,58,86,114,142],[6,34,62,90,118,146],[6,30,54,78,102,126,150],[6,24,50,76,102,128,154],[6,28,54,80,106,132,158],[6,32,58,84,110,136,162],[6,26,54,82,110,138,166],[6,30,58,86,114,142,170]],G15:1335,G18:7973,G15_MASK:21522,getBCHTypeInfo:function(t){var e=t<<10;while(n.getBCHDigit(e)-n.getBCHDigit(n.G15)>=0)e^=n.G15<<n.getBCHDigit(e)-n.getBCHDigit(n.G15);return(t<<10|e)^n.G15_MASK},getBCHTypeNumber:function(t){var e=t<<12;while(n.getBCHDigit(e)-n.getBCHDigit(n.G18)>=0)e^=n.G18<<n.getBCHDigit(e)-n.getBCHDigit(n.G18);return t<<12|e},getBCHDigit:function(t){var e=0;while(0!=t)e++,t>>>=1;return e},getPatternPosition:function(t){return n.PATTERN_POSITION_TABLE[t-1]},getMask:function(t,e,o){switch(t){case a.PATTERN000:return(e+o)%2==0;case a.PATTERN001:return e%2==0;case a.PATTERN010:return o%3==0;case a.PATTERN011:return(e+o)%3==0;case a.PATTERN100:return(Math.floor(e/2)+Math.floor(o/3))%2==0;case a.PATTERN101:return e*o%2+e*o%3==0;case a.PATTERN110:return(e*o%2+e*o%3)%2==0;case a.PATTERN111:return(e*o%3+(e+o)%2)%2==0;default:throw new Error("bad maskPattern:"+t)}},getErrorCorrectPolynomial:function(t){for(var e=new l([1],0),o=0;o<t;o++)e=e.multiply(new l([1,s.gexp(o)],0));return e},getLengthInBits:function(t,e){if(1<=e&&e<10)switch(t){case o.MODE_NUMBER:return 10;case o.MODE_ALPHA_NUM:return 9;case o.MODE_8BIT_BYTE:return 8;case o.MODE_KANJI:return 8;default:throw new Error("mode:"+t)}else if(e<27)switch(t){case o.MODE_NUMBER:return 12;case o.MODE_ALPHA_NUM:return 11;case o.MODE_8BIT_BYTE:return 16;case o.MODE_KANJI:return 10;default:throw new Error("mode:"+t)}else{if(!(e<41))throw new Error("type:"+e);switch(t){case o.MODE_NUMBER:return 14;case o.MODE_ALPHA_NUM:return 13;case o.MODE_8BIT_BYTE:return 16;case o.MODE_KANJI:return 12;default:throw new Error("mode:"+t)}}},getLostPoint:function(t){for(var e=t.getModuleCount(),o=0,r=0;r<e;r++)for(var i=0;i<e;i++){for(var a=0,n=t.isDark(r,i),s=-1;s<=1;s++)if(!(r+s<0||e<=r+s))for(var h=-1;h<=1;h++)i+h<0||e<=i+h||0==s&&0==h||n==t.isDark(r+s,i+h)&&a++;a>5&&(o+=3+a-5)}for(r=0;r<e-1;r++)for(i=0;i<e-1;i++){var l=0;t.isDark(r,i)&&l++,t.isDark(r+1,i)&&l++,t.isDark(r,i+1)&&l++,t.isDark(r+1,i+1)&&l++,0!=l&&4!=l||(o+=3)}for(r=0;r<e;r++)for(i=0;i<e-6;i++)t.isDark(r,i)&&!t.isDark(r,i+1)&&t.isDark(r,i+2)&&t.isDark(r,i+3)&&t.isDark(r,i+4)&&!t.isDark(r,i+5)&&t.isDark(r,i+6)&&(o+=40);for(i=0;i<e;i++)for(r=0;r<e-6;r++)t.isDark(r,i)&&!t.isDark(r+1,i)&&t.isDark(r+2,i)&&t.isDark(r+3,i)&&t.isDark(r+4,i)&&!t.isDark(r+5,i)&&t.isDark(r+6,i)&&(o+=40);var u=0;for(i=0;i<e;i++)for(r=0;r<e;r++)t.isDark(r,i)&&u++;var d=Math.abs(100*u/e/e-50)/5;return o+=10*d,o}},s={glog:function(t){if(t<1)throw new Error("glog("+t+")");return s.LOG_TABLE[t]},gexp:function(t){while(t<0)t+=255;while(t>=256)t-=255;return s.EXP_TABLE[t]},EXP_TABLE:new Array(256),LOG_TABLE:new Array(256)},h=0;h<8;h++)s.EXP_TABLE[h]=1<<h;for(h=8;h<256;h++)s.EXP_TABLE[h]=s.EXP_TABLE[h-4]^s.EXP_TABLE[h-5]^s.EXP_TABLE[h-6]^s.EXP_TABLE[h-8];for(h=0;h<255;h++)s.LOG_TABLE[s.EXP_TABLE[h]]=h;function l(t,e){if(void 0==t.length)throw new Error(t.length+"/"+e);var o=0;while(o<t.length&&0==t[o])o++;this.num=new Array(t.length-o+e);for(var r=0;r<t.length-o;r++)this.num[r]=t[r+o]}function u(t,e){this.totalCount=t,this.dataCount=e}function d(){this.buffer=[],this.length=0}l.prototype={get:function(t){return this.num[t]},getLength:function(){return this.num.length},multiply:function(t){for(var e=new Array(this.getLength()+t.getLength()-1),o=0;o<this.getLength();o++)for(var r=0;r<t.getLength();r++)e[o+r]^=s.gexp(s.glog(this.get(o))+s.glog(t.get(r)));return new l(e,0)},mod:function(t){if(this.getLength()-t.getLength()<0)return this;for(var e=s.glog(this.get(0))-s.glog(t.get(0)),o=new Array(this.getLength()),r=0;r<this.getLength();r++)o[r]=this.get(r);for(r=0;r<t.getLength();r++)o[r]^=s.gexp(s.glog(t.get(r))+e);return new l(o,0).mod(t)}},u.RS_BLOCK_TABLE=[[1,26,19],[1,26,16],[1,26,13],[1,26,9],[1,44,34],[1,44,28],[1,44,22],[1,44,16],[1,70,55],[1,70,44],[2,35,17],[2,35,13],[1,100,80],[2,50,32],[2,50,24],[4,25,9],[1,134,108],[2,67,43],[2,33,15,2,34,16],[2,33,11,2,34,12],[2,86,68],[4,43,27],[4,43,19],[4,43,15],[2,98,78],[4,49,31],[2,32,14,4,33,15],[4,39,13,1,40,14],[2,121,97],[2,60,38,2,61,39],[4,40,18,2,41,19],[4,40,14,2,41,15],[2,146,116],[3,58,36,2,59,37],[4,36,16,4,37,17],[4,36,12,4,37,13],[2,86,68,2,87,69],[4,69,43,1,70,44],[6,43,19,2,44,20],[6,43,15,2,44,16],[4,101,81],[1,80,50,4,81,51],[4,50,22,4,51,23],[3,36,12,8,37,13],[2,116,92,2,117,93],[6,58,36,2,59,37],[4,46,20,6,47,21],[7,42,14,4,43,15],[4,133,107],[8,59,37,1,60,38],[8,44,20,4,45,21],[12,33,11,4,34,12],[3,145,115,1,146,116],[4,64,40,5,65,41],[11,36,16,5,37,17],[11,36,12,5,37,13],[5,109,87,1,110,88],[5,65,41,5,66,42],[5,54,24,7,55,25],[11,36,12],[5,122,98,1,123,99],[7,73,45,3,74,46],[15,43,19,2,44,20],[3,45,15,13,46,16],[1,135,107,5,136,108],[10,74,46,1,75,47],[1,50,22,15,51,23],[2,42,14,17,43,15],[5,150,120,1,151,121],[9,69,43,4,70,44],[17,50,22,1,51,23],[2,42,14,19,43,15],[3,141,113,4,142,114],[3,70,44,11,71,45],[17,47,21,4,48,22],[9,39,13,16,40,14],[3,135,107,5,136,108],[3,67,41,13,68,42],[15,54,24,5,55,25],[15,43,15,10,44,16],[4,144,116,4,145,117],[17,68,42],[17,50,22,6,51,23],[19,46,16,6,47,17],[2,139,111,7,140,112],[17,74,46],[7,54,24,16,55,25],[34,37,13],[4,151,121,5,152,122],[4,75,47,14,76,48],[11,54,24,14,55,25],[16,45,15,14,46,16],[6,147,117,4,148,118],[6,73,45,14,74,46],[11,54,24,16,55,25],[30,46,16,2,47,17],[8,132,106,4,133,107],[8,75,47,13,76,48],[7,54,24,22,55,25],[22,45,15,13,46,16],[10,142,114,2,143,115],[19,74,46,4,75,47],[28,50,22,6,51,23],[33,46,16,4,47,17],[8,152,122,4,153,123],[22,73,45,3,74,46],[8,53,23,26,54,24],[12,45,15,28,46,16],[3,147,117,10,148,118],[3,73,45,23,74,46],[4,54,24,31,55,25],[11,45,15,31,46,16],[7,146,116,7,147,117],[21,73,45,7,74,46],[1,53,23,37,54,24],[19,45,15,26,46,16],[5,145,115,10,146,116],[19,75,47,10,76,48],[15,54,24,25,55,25],[23,45,15,25,46,16],[13,145,115,3,146,116],[2,74,46,29,75,47],[42,54,24,1,55,25],[23,45,15,28,46,16],[17,145,115],[10,74,46,23,75,47],[10,54,24,35,55,25],[19,45,15,35,46,16],[17,145,115,1,146,116],[14,74,46,21,75,47],[29,54,24,19,55,25],[11,45,15,46,46,16],[13,145,115,6,146,116],[14,74,46,23,75,47],[44,54,24,7,55,25],[59,46,16,1,47,17],[12,151,121,7,152,122],[12,75,47,26,76,48],[39,54,24,14,55,25],[22,45,15,41,46,16],[6,151,121,14,152,122],[6,75,47,34,76,48],[46,54,24,10,55,25],[2,45,15,64,46,16],[17,152,122,4,153,123],[29,74,46,14,75,47],[49,54,24,10,55,25],[24,45,15,46,46,16],[4,152,122,18,153,123],[13,74,46,32,75,47],[48,54,24,14,55,25],[42,45,15,32,46,16],[20,147,117,4,148,118],[40,75,47,7,76,48],[43,54,24,22,55,25],[10,45,15,67,46,16],[19,148,118,6,149,119],[18,75,47,31,76,48],[34,54,24,34,55,25],[20,45,15,61,46,16]],u.getRSBlocks=function(t,e){var o=u.getRsBlockTable(t,e);if(void 0==o)throw new Error("bad rs block @ typeNumber:"+t+"/errorCorrectLevel:"+e);for(var r=o.length/3,i=[],a=0;a<r;a++)for(var n=o[3*a+0],s=o[3*a+1],h=o[3*a+2],l=0;l<n;l++)i.push(new u(s,h));return i},u.getRsBlockTable=function(t,e){switch(e){case i.L:return u.RS_BLOCK_TABLE[4*(t-1)+0];case i.M:return u.RS_BLOCK_TABLE[4*(t-1)+1];case i.Q:return u.RS_BLOCK_TABLE[4*(t-1)+2];case i.H:return u.RS_BLOCK_TABLE[4*(t-1)+3];default:return}},d.prototype={get:function(t){var e=Math.floor(t/8);return 1==(this.buffer[e]>>>7-t%8&1)},put:function(t,e){for(var o=0;o<e;o++)this.putBit(1==(t>>>e-o-1&1))},getLengthInBits:function(){return this.length},putBit:function(t){var e=Math.floor(this.length/8);this.buffer.length<=e&&this.buffer.push(0),t&&(this.buffer[e]|=128>>>this.length%8),this.length++}};var c=[[17,14,11,7],[32,26,20,14],[53,42,32,24],[78,62,46,34],[106,84,60,44],[134,106,74,58],[154,122,86,64],[192,152,108,84],[230,180,130,98],[271,213,151,119],[321,251,177,137],[367,287,203,155],[425,331,241,177],[458,362,258,194],[520,412,292,220],[586,450,322,250],[644,504,364,280],[718,560,394,310],[792,624,442,338],[858,666,482,382],[929,711,509,403],[1003,779,565,439],[1091,857,611,461],[1171,911,661,511],[1273,997,715,535],[1367,1059,751,593],[1465,1125,805,625],[1528,1190,868,658],[1628,1264,908,698],[1732,1370,982,742],[1840,1452,1030,790],[1952,1538,1112,842],[2068,1628,1168,898],[2188,1722,1228,958],[2303,1809,1283,983],[2431,1911,1351,1051],[2563,1989,1423,1093],[2699,2099,1499,1139],[2809,2213,1579,1219],[2953,2331,1663,1273]];function f(){return"undefined"!=typeof CanvasRenderingContext2D}function g(){var t=!1,e=navigator.userAgent;if(/android/i.test(e)){t=!0;var o=e.toString().match(/android ([0-9]\.[0-9])/i);o&&o[1]&&(t=parseFloat(o[1]))}return t}var p=function(){var t=function(t,e){this._el=t,this._htOption=e};return t.prototype.draw=function(t){var e=this._htOption,o=this._el,r=t.getModuleCount();Math.floor(e.width/r),Math.floor(e.height/r);function i(t,e){var o=document.createElementNS("http://www.w3.org/2000/svg",t);for(var r in e)e.hasOwnProperty(r)&&o.setAttribute(r,e[r]);return o}this.clear();var a=i("svg",{viewBox:"0 0 "+String(r)+" "+String(r),width:"100%",height:"100%",fill:e.colorLight});a.setAttributeNS("http://www.w3.org/2000/xmlns/","xmlns:xlink","http://www.w3.org/1999/xlink"),o.appendChild(a),a.appendChild(i("rect",{fill:e.colorLight,width:"100%",height:"100%"})),a.appendChild(i("rect",{fill:e.colorDark,width:"1",height:"1",id:"template"}));for(var n=0;n<r;n++)for(var s=0;s<r;s++)if(t.isDark(n,s)){var h=i("use",{x:String(s),y:String(n)});h.setAttributeNS("http://www.w3.org/1999/xlink","href","#template"),a.appendChild(h)}},t.prototype.clear=function(){while(this._el.hasChildNodes())this._el.removeChild(this._el.lastChild)},t}(),m="svg"===document.documentElement.tagName.toLowerCase(),v=m?p:f()?function(){function t(){this._elImage.src=this._elCanvas.toDataURL("image/png"),this._elImage.style.display="block",this._elCanvas.style.display="none"}if(this._android&&this._android<=2.1){var e=1/window.devicePixelRatio,o=CanvasRenderingContext2D.prototype.drawImage;CanvasRenderingContext2D.prototype.drawImage=function(t,r,i,a,n,s,h,l,u){if("nodeName"in t&&/img/i.test(t.nodeName))for(var d=arguments.length-1;d>=1;d--)arguments[d]=arguments[d]*e;else"undefined"==typeof l&&(arguments[1]*=e,arguments[2]*=e,arguments[3]*=e,arguments[4]*=e);o.apply(this,arguments)}}function r(t,e){var o=this;if(o._fFail=e,o._fSuccess=t,null===o._bSupportDataURI){var r=document.createElement("img"),i=function(){o._bSupportDataURI=!1,o._fFail&&o._fFail.call(o)},a=function(){o._bSupportDataURI=!0,o._fSuccess&&o._fSuccess.call(o)};return r.onabort=i,r.onerror=i,r.onload=a,void(r.src="data:image/gif;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==")}!0===o._bSupportDataURI&&o._fSuccess?o._fSuccess.call(o):!1===o._bSupportDataURI&&o._fFail&&o._fFail.call(o)}var i=function(t,e){this._bIsPainted=!1,this._android=g(),this._htOption=e,this._elCanvas=document.createElement("canvas"),this._elCanvas.width=e.width,this._elCanvas.height=e.height,t.appendChild(this._elCanvas),this._el=t,this._oContext=this._elCanvas.getContext("2d"),this._bIsPainted=!1,this._elImage=document.createElement("img"),this._elImage.alt="Scan me!",this._elImage.style.display="none",this._el.appendChild(this._elImage),this._bSupportDataURI=null};return i.prototype.draw=function(t){var e=this._elImage,o=this._oContext,r=this._htOption,i=t.getModuleCount(),a=r.width/i,n=r.height/i,s=Math.round(a),h=Math.round(n);e.style.display="none",this.clear();for(var l=0;l<i;l++)for(var u=0;u<i;u++){var d=t.isDark(l,u),c=u*a,f=l*n;o.strokeStyle=d?r.colorDark:r.colorLight,o.lineWidth=1,o.fillStyle=d?r.colorDark:r.colorLight,o.fillRect(c,f,a,n),o.strokeRect(Math.floor(c)+.5,Math.floor(f)+.5,s,h),o.strokeRect(Math.ceil(c)-.5,Math.ceil(f)-.5,s,h)}this._bIsPainted=!0},i.prototype.makeImage=function(){this._bIsPainted&&r.call(this,t)},i.prototype.isPainted=function(){return this._bIsPainted},i.prototype.clear=function(){this._oContext.clearRect(0,0,this._elCanvas.width,this._elCanvas.height),this._bIsPainted=!1},i.prototype.round=function(t){return t?Math.floor(1e3*t)/1e3:t},i}():function(){var t=function(t,e){this._el=t,this._htOption=e};return t.prototype.draw=function(t){for(var e=this._htOption,o=this._el,r=t.getModuleCount(),i=Math.floor(e.width/r),a=Math.floor(e.height/r),n=['<table style="border:0;border-collapse:collapse;">'],s=0;s<r;s++){n.push("<tr>");for(var h=0;h<r;h++)n.push('<td style="border:0;border-collapse:collapse;padding:0;margin:0;width:'+i+"px;height:"+a+"px;background-color:"+(t.isDark(s,h)?e.colorDark:e.colorLight)+';"></td>');n.push("</tr>")}n.push("</table>"),o.innerHTML=n.join("");var l=o.childNodes[0],u=(e.width-l.offsetWidth)/2,d=(e.height-l.offsetHeight)/2;u>0&&d>0&&(l.style.margin=d+"px "+u+"px")},t.prototype.clear=function(){this._el.innerHTML=""},t}();function _(t,e){for(var o=1,r=C(t),a=0,n=c.length;a<=n;a++){var s=0;switch(e){case i.L:s=c[a][0];break;case i.M:s=c[a][1];break;case i.Q:s=c[a][2];break;case i.H:s=c[a][3];break}if(r<=s)break;o++}if(o>c.length)throw new Error("Too long data");return o}function C(t){var e=encodeURI(t).toString().replace(/\%[0-9a-fA-F]{2}/g,"a");return e.length+(e.length!=t?3:0)}return r=function(t,e){if(this._htOption={width:256,height:256,typeNumber:4,colorDark:"#000000",colorLight:"#ffffff",correctLevel:i.H},"string"===typeof e&&(e={text:e}),e)for(var o in e)this._htOption[o]=e[o];"string"==typeof t&&(t=document.getElementById(t)),this._htOption.useSVG&&(v=p),this._android=g(),this._el=t,this._oQRCode=null,this._oDrawing=new v(this._el,this._htOption),this._htOption.text&&this.makeCode(this._htOption.text)},r.prototype.makeCode=function(t){this._oQRCode=new e(_(t,this._htOption.correctLevel),this._htOption.correctLevel),this._oQRCode.addData(t),this._oQRCode.make(),this._el.title=t,this._oDrawing.draw(this._oQRCode),this.makeImage()},r.prototype.makeImage=function(){"function"==typeof this._oDrawing.makeImage&&(!this._android||this._android>=3)&&this._oDrawing.makeImage()},r.prototype.clear=function(){this._oDrawing.clear()},r.CorrectLevel=i,r})},"412f":function(t,e,o){"use strict";var r=o("c7b3"),i=o.n(r);i.a},c7b3:function(t,e,o){},ef02:function(t,e,o){"use strict";o.r(e);var r=function(){var t=this,e=t.$createElement,o=t._self._c||e;return o("div",{staticClass:"false-user-list"},[o("div",{staticClass:"password-manager"},[o("div",{staticClass:"false-user-header"},[o("span",{staticClass:"false-user-title"},[t._v("说明：")]),t.h5DomainDesc?o("p",{staticClass:"false-user-poptip"},[o("span",{domProps:{textContent:t._s(t.h5DomainDesc)}})]):t._e()]),o("div",{staticClass:"account-password"},[o("div",{staticClass:"domain-left"},[o("div",{staticClass:"password-item"},[o("label",[t._v("请输入你的域名：")]),o("input",{directives:[{name:"model",rawName:"v-model",value:t.h5Domain,expression:"h5Domain"}],attrs:{placeholder:"请输入你的域名"},domProps:{value:t.h5Domain},on:{input:function(e){e.target.composing||(t.h5Domain=e.target.value)}}})]),o("div",{staticClass:"password-item"},[o("label",[t._v("默认安全码（可空）：")]),o("input",{directives:[{name:"model",rawName:"v-model",value:t.safeCode,expression:"safeCode"}],attrs:{placeholder:"请输入你的默认安全码"},domProps:{value:t.safeCode},on:{input:function(e){e.target.composing||(t.safeCode=e.target.value)}}})]),o("div",{staticClass:"password-item"},[o("label",[t._v("默认房间号（可空）：")]),o("input",{directives:[{name:"model",rawName:"v-model",value:t.roomCode,expression:"roomCode"}],attrs:{placeholder:"请输入你的默认房间号"},domProps:{value:t.roomCode},on:{input:function(e){e.target.composing||(t.roomCode=e.target.value)}}})]),o("div",{staticClass:"update-password"},[o("span",{on:{click:t.handleUpdateH5Domain}},[t._v("生成h5网址/二维码")])]),t.h5DomainUrl?o("div",{staticClass:"show-url",on:{click:t.handleCopy}},[o("span",[t._v(t._s(t.h5DomainUrl))])]):t._e()]),o("div",{staticClass:"domain-right"},[t.h5Domain?[t._m(0),o("div",{staticClass:"update-password"},[o("span",{on:{click:t.handleSaveH5Qrcode}},[t._v("保存二维码")])])]:t._e()],2)])]),o("a",{ref:"saveImg",staticStyle:{display:"none"},attrs:{download:""}})])},i=[function(){var t=this,e=t.$createElement,o=t._self._c||e;return o("div",{staticClass:"h5-domain-box"},[o("div",{attrs:{id:"h5-domain-qr"}})])}],a=o("1f57"),n=o.n(a),s={data:function(){return{h5Domain:"",h5DomainDesc:"",h5DomainUrl:"",safeCode:"",roomCode:"",qrcode:null,isLoading:!1}},mounted:function(){var t=this;t.handlegetH5Domain()},methods:{handleUpdateH5Domain:function(){var t=this;t.h5Domain?t.isLoading||(t.isLoading=!0,t.$axios({url:"/api/Setup/UpdateDomain",method:"post",params:{url:t.h5Domain,seurityNo:t.safeCode,roomNum:t.roomCode}}).then(function(e){t.isLoading=!1,100==e.data.Status?(t.$Message.success(e.data.Message),t.handlegetH5Domain()):t.$Message.error(e.data.Message)})):t.$Message.error("请输入你的域名")},handlegetH5Domain:function(){var t=this;return new Promise(function(e,o){t.$axios({url:"/api/Setup/GetDomainInfo"}).then(function(o){t.h5Domain=o.data.Model.H5DomainUrl,t.safeCode=o.data.Model.SetupSeurityNo,t.roomCode=o.data.Model.SetupRoomNum,t.h5DomainDesc=o.data.Model.H5DomainDescription;var r=(t.safeCode?"s0=".concat(t.safeCode):"")+(t.roomCode?"&s1=".concat(t.roomCode):"");t.h5DomainUrl=t.h5Domain?"http://".concat(t.h5Domain,"/m/index.html#/login?").concat(r):"",t.$nextTick(function(){t.h5Domain&&(t.qrcode?t.qrcode.makeCode(t.h5DomainUrl):t.qrcode=new n.a(document.getElementById("h5-domain-qr"),{width:132,height:132,text:t.h5DomainUrl,colorDark:"#000",colorLight:"#fff"}),e())})})})},handleSaveH5Qrcode:function(){this.$refs["saveImg"].href=this.qrcode._oDrawing._elImage.src,this.$refs["saveImg"].href&&this.$refs["saveImg"].click()},handleCopy:function(){var t=this,e=document.createElement("textarea");e.style.position="absolute",e.style.left="-9999px",e.style.top="-9999px",e.setAttribute("readonly",""),e.value=t.h5DomainUrl,document.body.appendChild(e),e.select(),document.execCommand("Copy")?t.$Message.success("复制成功"):t.$Message.error("复制失败")}}},h=s,l=(o("412f"),o("6691")),u=Object(l["a"])(h,r,i,!1,null,"874a08a8",null);e["default"]=u.exports}}]);
//# sourceMappingURL=chunk-0347a1ce.1588759372124.js.map