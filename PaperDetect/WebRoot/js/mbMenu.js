/*
 * ******************************************************************************
 *  jquery.mb.components
 *  file: mbMenu.js
 *
 *  Copyright (c) 2001-2014. Matteo Bicocchi (Pupunzi);
 *  Open lab srl, Firenze - Italy
 *  email: matteo@open-lab.com
 *  site: 	http://pupunzi.com
 *  blog:	http://pupunzi.open-lab.com
 * 	http://open-lab.com
 *
 *  Licences: MIT, GPL
 *  http://www.opensource.org/licenses/mit-license.php
 *  http://www.gnu.org/licenses/gpl.html
 *
 *  last modified: 27/01/14 20.06
 *  *****************************************************************************
 */

// to get the element that is fireing a contextMenu event you have $.mbMenu.lastContextMenuEl that returns an object.

/*Browser detection patch*/
if (!jQuery.browser) {
	jQuery.browser = {}, jQuery.browser.mozilla = !1, jQuery.browser.webkit = !1, jQuery.browser.opera = !1, jQuery.browser.safari = !1, jQuery.browser.chrome = !1, jQuery.browser.msie = !1;
	var nAgt = navigator.userAgent;
	jQuery.browser.ua = nAgt, jQuery.browser.name = navigator.appName, jQuery.browser.fullVersion = "" + parseFloat(navigator.appVersion), jQuery.browser.majorVersion = parseInt(navigator.appVersion, 10);
	var nameOffset, verOffset, ix;
	if (-1 != (verOffset = nAgt.indexOf("Opera")))jQuery.browser.opera = !0, jQuery.browser.name = "Opera", jQuery.browser.fullVersion = nAgt.substring(verOffset + 6), -1 != (verOffset = nAgt.indexOf("Version")) && (jQuery.browser.fullVersion = nAgt.substring(verOffset + 8)); else if (-1 != (verOffset = nAgt.indexOf("MSIE")))jQuery.browser.msie = !0, jQuery.browser.name = "Microsoft Internet Explorer", jQuery.browser.fullVersion = nAgt.substring(verOffset + 5); else if (-1 != nAgt.indexOf("Trident")) {
		jQuery.browser.msie = !0, jQuery.browser.name = "Microsoft Internet Explorer";
		var start = nAgt.indexOf("rv:") + 3, end = start + 4;
		jQuery.browser.fullVersion = nAgt.substring(start, end)
	} else-1 != (verOffset = nAgt.indexOf("Chrome")) ? (jQuery.browser.webkit = !0, jQuery.browser.chrome = !0, jQuery.browser.name = "Chrome", jQuery.browser.fullVersion = nAgt.substring(verOffset + 7)) : -1 != (verOffset = nAgt.indexOf("Safari")) ? (jQuery.browser.webkit = !0, jQuery.browser.safari = !0, jQuery.browser.name = "Safari", jQuery.browser.fullVersion = nAgt.substring(verOffset + 7), -1 != (verOffset = nAgt.indexOf("Version")) && (jQuery.browser.fullVersion = nAgt.substring(verOffset + 8))) : -1 != (verOffset = nAgt.indexOf("AppleWebkit")) ? (jQuery.browser.webkit = !0, jQuery.browser.name = "Safari", jQuery.browser.fullVersion = nAgt.substring(verOffset + 7), -1 != (verOffset = nAgt.indexOf("Version")) && (jQuery.browser.fullVersion = nAgt.substring(verOffset + 8))) : -1 != (verOffset = nAgt.indexOf("Firefox")) ? (jQuery.browser.mozilla = !0, jQuery.browser.name = "Firefox", jQuery.browser.fullVersion = nAgt.substring(verOffset + 8)) : (nameOffset = nAgt.lastIndexOf(" ") + 1) < (verOffset = nAgt.lastIndexOf("/")) && (jQuery.browser.name = nAgt.substring(nameOffset, verOffset), jQuery.browser.fullVersion = nAgt.substring(verOffset + 1), jQuery.browser.name.toLowerCase() == jQuery.browser.name.toUpperCase() && (jQuery.browser.name = navigator.appName));
	-1 != (ix = jQuery.browser.fullVersion.indexOf(";")) && (jQuery.browser.fullVersion = jQuery.browser.fullVersion.substring(0, ix)), -1 != (ix = jQuery.browser.fullVersion.indexOf(" ")) && (jQuery.browser.fullVersion = jQuery.browser.fullVersion.substring(0, ix)), jQuery.browser.majorVersion = parseInt("" + jQuery.browser.fullVersion, 10), isNaN(jQuery.browser.majorVersion) && (jQuery.browser.fullVersion = "" + parseFloat(navigator.appVersion), jQuery.browser.majorVersion = parseInt(navigator.appVersion, 10)), jQuery.browser.version = jQuery.browser.majorVersion
}


(function($) {
	$.mbMenu = {
		name:"mbMenu",
		author:"Matteo Bicocchi",
		version:"2.9.7",
		actualMenuOpener:false,
		options: {
			template:"yourMenuVoiceTemplate",// the url that returns the menu voices via ajax. the data passed in the request is the "menu" attribute value as "menuId"
			additionalData:"",
			menuSelector:".menuContainer",
			menuWidth:400,
			openOnRight:false,
			containment:"window",
			iconPath:"ico/",
			hasImages:true,
			fadeInTime:100,
			fadeOutTime:200,
			menuTop:0,
			menuLeft:0,
			submenuTop:0,
			submenuLeft:4,
			opacity:1,
			ajaxAlwaysReload:false,
			menuFetcher: function(op, m, callback){
				$.ajax({
					type: "GET",
					url: op.options.template,
					cache: false,
					async: false,
					data:"menuId="+m+(op.options.additionalData!=""?"&"+op.options.additionalData:""),
					dataType:"html",
					success: callback
				});
			},

			cloner: function(op, m, source) {
				return source.clone(true);
			},
			openOnClick:true,
			closeOnMouseOut:false,
			closeAfter:500,
			minZindex:"auto", // or number
			hoverIntent:0, //if you use jquery.hoverIntent.js set this to time in milliseconds; 0= false;
			submenuHoverIntent:200, //if you use jquery.hoverIntent.js set this to time in milliseconds; 0= false;
			onContextualMenu:function(){} //it pass 'o' (the menu you clicked on) and 'e' (the event)
		},

		buildMenu : function (options){
			return this.each (function ()
			{
				var thisMenu =this;
				thisMenu.id = !this.id ? "menu_"+Math.floor (Math.random () * 1000): this.id;
				this.options = {};
				$.extend (this.options, $.mbMenu.options);
				$.extend (this.options, options);

				$(".mbmenu").hide();
				thisMenu.clicked = false;
				thisMenu.rootMenu=false;
				thisMenu.actualOpenedMenu=false;
				thisMenu.menuvoice=false;
				var root=$(this);
				var openOnClick=this.options.openOnClick;
				var closeOnMouseOut=this.options.closeOnMouseOut;

				//build roots
				$(root).each(function(){

					/*
					 *using metadata plugin you can add attribute writing them inside the class attr with a JSON sintax
					 * for ex: class="rootVoice {menu:'menu_2'}"
					 */
					if ($.metadata){

						thisMenu.menuvoice=$(this).find(".rootVoice");
						$(thisMenu.menuvoice).each(function(){
							if ($(this).metadata().menu) $(this).attr("menu",$(this).metadata().menu);
							if ($(this).metadata().disabled) $(this).attr("isDisable",$(this).metadata().disabled);
						});
					}

					thisMenu.menuvoice=$(this).find("[menu]").add($(this).filter("[menu]"));
					thisMenu.menuvoice.filter("[isDisable]").addClass("disabled");

					$(thisMenu.menuvoice).css("white-space","nowrap");

					if(openOnClick){
						$(thisMenu.menuvoice).on("click",function(){
							$(document).off("click.closeMbMenu");
							if (!$(this).attr("isOpen")){
								$(this).buildMbMenu(thisMenu,$(this).attr("menu"));
								$(this).attr("isOpen","true");
							}else{
								$(this).removeMbMenu(thisMenu,true);
								$(this).addClass("selected");
							}

							//empty
							if($(this).attr("menu")=="empty"){
								if(thisMenu.actualOpenedMenu){
									$("[isOpen]").removeAttr("isOpen");
								}
								$(this).removeMbMenu(thisMenu);
							}
							$(document).off("click.closeMbMenu");
						});
					}

					$(thisMenu.menuvoice).mb_hover(
							this.options.hoverIntent,
							function(){
								if(!$(this).attr("isOpen"))
									$("[isOpen]").removeAttr("isOpen");
								$(document).off("click.closeMbMenu");
								if (closeOnMouseOut) clearTimeout($.mbMenu.deleteOnMouseOut);
								if (!openOnClick) $(thisMenu).find(".selected").removeClass("selected");
								if(thisMenu.actualOpenedMenu){ $(thisMenu.actualOpenedMenu).removeClass("selected");}
								$(this).addClass("selected");
								if((thisMenu.clicked || !openOnClick) && !$(this).attr("isOpen")){
									$(this).removeMbMenu(thisMenu);
									$(this).buildMbMenu(thisMenu,$(this).attr("menu"));
									if ($(this).attr("menu")=="empty"){
										$(this).removeMbMenu(thisMenu);
									}
									$(this).attr("isOpen","true");
								}
							},
							function(){
								if (closeOnMouseOut)
									$.mbMenu.deleteOnMouseOut= setTimeout(function(){
										$(this).removeMbMenu(thisMenu,true);
										$(document).off("click.closeMbMenu");
									},$(root)[0].options.closeAfter);

								if ($(this).attr("menu")=="empty"){
									$(this).removeClass("selected");
								}
								if(!thisMenu.clicked)
									$(this).removeClass("selected");
								$(document).one("click.closeMbMenu",function(){
									$("[isOpen]").removeAttr("isOpen");
									$(this).removeClass("selected");
									$(this).removeMbMenu(thisMenu,true);
									thisMenu.rootMenu=false;thisMenu.clicked=false;
								});
							}
					);
				});
			});
		},

		buildContextualMenu:  function (options){
			return this.each (function ()
			{
				var thisMenu = this;
				thisMenu.options = {};
				$.extend (thisMenu.options, $.mbMenu.options);
				$.extend (thisMenu.options, options);
				$(".mbmenu").hide();
				thisMenu.clicked = false;
				thisMenu.rootMenu=false;
				thisMenu.actualOpenedMenu=false;
				thisMenu.menuvoice=false;
				var cMenuEls;
				if ($.metadata){
					$.metadata.setType("class");
					cMenuEls= $(this).find(".cmVoice");
					$(cMenuEls).each(function(){
						if ($(this).metadata().cMenu) $(this).attr("cMenu",$(this).metadata().cMenu);
					});
				}
				cMenuEls= $(this).find("[cMenu]").add($(this).filter("[cMenu]"));

				$(cMenuEls).each(function(){
					$(this).css({"-webkit-user-select":"none","-moz-user-select":"none"});
					var cm=this;
					cm.id = !cm.id ? "menu_"+Math.floor (Math.random () * 100): cm.id;
					$(cm).css({cursor:"default"});
					//var evt= $.browser.opera?"dblclick":"contextmenu";
					var evt= "contextmenu";
					$(cm).on(evt,function(event){

						event.preventDefault();
						event.stopPropagation();
						event.cancelBubble=true;

						$.mbMenu.lastContextMenuEl=cm;

						if ($.mbMenu.options.actualMenuOpener) {
							$(thisMenu).removeMbMenu($.mbMenu.options.actualMenuOpener);
						}
						/*add custom behavior to contextMenuEvent passing the el and the event
						 *you can for example store to global var the obj that is fireing the event
						 *mbActualContextualMenuObj=cm;
						 *
						 * you can for example create a function that manipulate the voices of the menu
						 * you are opening according to a certain condition...
						 */

						thisMenu.options.onContextualMenu(this,event);

						$(this).buildMbMenu(thisMenu,$(this).attr("cMenu"),"cm",event);
						$(this).attr("isOpen","true");

					});
				});
			});
		}
	};
	$.fn.extend({
		buildMbMenu: function(op,m,type,e){
			var mouseOver=$.browser.msie?"mouseenter":"mouseover";
			var mouseOut=$.browser.msie?"mouseleave":"mouseout";
			if (e) {
				this.mouseX=$(this).getMouseX(e);
				this.mouseY=$(this).getMouseY(e);
			}

			if ($.mbMenu.options.actualMenuOpener && $.mbMenu.options.actualMenuOpener!=op)
				$(op).removeMbMenu($.mbMenu.options.actualMenuOpener);
			$.mbMenu.options.actualMenuOpener=op;
			if(!type || type=="cm")	{
				if (op.rootMenu) {
					$(op.rootMenu).removeMbMenu(op);
					$(op.actualOpenedMenu).removeAttr("isOpen");
					$("[isOpen]").removeAttr("isOpen");
				}
				op.clicked=true;
				op.actualOpenedMenu=this;
				$(op.actualOpenedMenu).attr("isOpen","true");
				$(op.actualOpenedMenu).addClass("selected");
			}

			//empty menu
			if($(this).attr("menu")=="empty"){
				return;
			}

			var opener=this;
			var where=(!type|| type=="cm")?$(document.body):$(this).parent().parent();

			var menuClass= op.options.menuSelector.replace(".","");

			if(op.rootMenu) menuClass+=" submenuContainer";
			if(!op.rootMenu && $(opener).attr("isDisable")) menuClass+=" disabled";

			where.append("<div class='menuDiv'><div class='"+menuClass+" '></div></div>");
			this.menu  = where.find(".menuDiv");
			$(this.menu).css({width:0, height:0});
			if (op.options.minZindex!="auto"){
				$(this.menu).css({zIndex:op.options.minZindex++});
			}else{
				$(this.menu).mb_bringToFront();
			}
			this.menuContainer  = $(this.menu).find(op.options.menuSelector);

			$(this.menuContainer).on(mouseOver,function(){
				$(opener).addClass("selected");
			});
			$(this.menuContainer).css({
				position:"absolute",
				opacity:op.options.opacity
			});
			if (!$("#"+m).html() || op.options.ajaxAlwaysReload){
				$("#"+m).remove();
				op.options.menuFetcher(op, m, function(html){
					$("body").append(html);
					$("#"+m).hide();
				});
			}
			$(this.menuContainer).attr("id", "mb_"+m).hide();

			//LITERAL MENU SUGGESTED BY SvenDowideit
			var isBoxmenu=$("#"+m).hasClass("boxMenu");

			if (isBoxmenu) {
				this.voices = op.options.cloner(op, m, $("#"+m));
				this.voices.css({display: "block"});
				this.voices.attr("id", m+"_clone");
			} else {
				//TODO this will break <a rel=text> - if there are nested a's
				this.voices= op.options.cloner(op, m, $("#"+m).find("a"));
			}

			/*
			 *using metadata plugin you can add attribut writing them inside the class attr with a JSON sintax
			 * for ex: class="rootVoice {menu:'menu_2'}"
			 */
			if ($.metadata){
				$.metadata.setType("class");
				$(this.voices).each(function(){
					if ($(this).metadata().disabled) $(this).attr("isdisable",$(this).metadata().disabled);
					if ($(this).metadata().img) $(this).attr("img",$(this).metadata().img);
					if ($(this).metadata().menu) $(this).attr("menu",$(this).metadata().menu);
					if ($(this).metadata().action) $(this).attr("action",$(this).metadata().action);
				});
			}

			// build each voices of the menu
			$(this.voices).each(function(i){

				var voice=this;
				var imgPlace="";

				var isText=$(voice).data("type")=="text";
				var isTitle=$(voice).data("type")=="title";
				var isDisabled=$(voice).is("[isdisable]");
				if(!op.rootMenu && $(opener).attr("isDisable"))
					isDisabled=true;

				var isSeparator=$(voice).data("type")=="separator";

				// boxMenu SUGGESTED by Sven Dowideit
				if (op.options.hasImages && !isText && !isBoxmenu){

					var imgPath=$(voice).attr("img")?$(voice).attr("img"):"blank.gif";
					imgPath=(imgPath.length>3 && imgPath.indexOf(".")>-1)?"<img class='imgLine' src='"+op.options.iconPath+imgPath+"'>":imgPath;
					imgPlace="<td class='img'>"+imgPath+"</td>";
				}

				var line="<table id='"+m+"_"+i+"' class='line"+(isTitle?" title":"")+"' cellspacing='0' cellpadding='0' border='0' style='width:100%;' width='100%'><tr>"+imgPlace+"<td class='voice' nowrap></td></tr></table>";

				if(isSeparator)
					line="<p class='separator' style='width:100%;'></p>";

				if(isText)
					line="<div style='width:100%; display:table' class='line' id='"+m+"_"+i+"'><div class='voice'></div></div>";

				// boxMenu SUGGESTED by Sven Dowideit
				if(isBoxmenu)
					line="<div style='width:100%; display:inline' class='' id='"+m+"_"+i+"'><div class='voice'></div></div>";

				$(opener.menuContainer).append(line);

				var menuLine = $(opener.menuContainer).find("#" + m + "_" + i);
				var menuVoice = menuLine.find(".voice");
				if(!isSeparator){
					menuVoice.append(this);
					if($(this).attr("menu") && !isDisabled){
						menuLine.find(".voice a").wrap("<div class='menuArrow'></div>");
						menuLine.find(".menuArrow").addClass("subMenuOpener");
						menuLine.css({cursor:"default"});
						this.isOpener=true;
					}
					if(isText){
						menuVoice.addClass("textBox");
						if ($.browser.msie) menuVoice.css({maxWidth:op.options.menuWidth});
						this.isOpener=true;
					}
					if(isDisabled){
						menuLine.addClass("disabled").css({cursor:"default"});
					}

					if(!(isText || isTitle || isDisabled ||isBoxmenu)){
						menuLine.css({cursor:"pointer"});

						menuLine.on("mouseover",function(){
							clearTimeout($.mbMenu.deleteOnMouseOut);
							$(this).addClass("selected");
						});

						menuLine.on("mouseout",function(){
							$(this).removeClass("selected");
						});

						menuLine.mb_hover(
								op.options.submenuHoverIntent,
								function(event){
									if(opener.menuContainer.actualSubmenu && !$(voice).attr("menu")){
										$(opener.menu).find(".menuDiv").remove();
										$(opener.menuContainer.actualSubmenu).removeClass("selected");
										opener.menuContainer.actualSubmenu=false;
									}
									if ($(voice).attr("menu")){
										if(opener.menuContainer.actualSubmenu && opener.menuContainer.actualSubmenu!=this){
											$(opener.menu).find(".menuDiv").remove();
											$(opener.menuContainer.actualSubmenu).removeClass("selected");
											opener.menuContainer.actualSubmenu=false;
										}
										if (!$(voice).attr("action")) $(opener.menuContainer).find("#"+m+"_"+i).css("cursor","default");
										if (!opener.menuContainer.actualSubmenu || opener.menuContainer.actualSubmenu!=this){
											$(opener.menu).find(".menuDiv").remove();

											opener.menuContainer.actualSubmenu=false;
											$(this).buildMbMenu(op,$(voice).attr("menu"),"sm",event);
											opener.menuContainer.actualSubmenu=this;
										}
										$(this).attr("isOpen","true");
										return false;
									}
								},
								function(){}
						);
					}
					if(isDisabled || isTitle || isText || isBoxmenu){
						$(this).removeAttr("href");
						menuLine.on(mouseOver,function(){
							if (closeOnMouseOut) clearTimeout($.mbMenu.deleteOnMouseOut);
							if(opener.menuContainer.actualSubmenu){
								$(opener.menu).find(".menuDiv").remove();
								opener.menuContainer.actualSubmenu=false;
							}
						}).css("cursor","default");
					}
					if(isBoxmenu){
						menuLine.find("a").css("cursor","pointer").click(function(){$.fn.removeMbMenu($.mbMenu.options.actualMenuOpener,true);})
					}
					menuLine.on("click",function(event){
						if (($(voice).attr("action") || $(voice).attr("href")) && !isDisabled &&  !isBoxmenu && !isText){
							var target=$(voice).attr("target")?$(voice).attr("target"):"_self";
							if ($(voice).attr("href") && $(voice).attr("href").indexOf("javascript:")>-1){
								$(voice).attr("action",$(voice).attr("href").replace("javascript:",""));
							}
							var link = $(voice).attr("action") ? $(voice).attr("action") : "window.open(\"" + $(voice).attr("href") + "\", \"" + target + "\")";
							$(voice).removeAttr("href");
							eval(link);
							$(this).removeMbMenu(op,true);
						}else{
							$(document).off("click.closeMbMenu");
						}
					});
				}
			});

			// Close on Mouseout

			var closeOnMouseOut=$(op)[0].options.closeOnMouseOut;
			if (closeOnMouseOut){
				$(opener.menuContainer).on("mouseenter",function(){
					clearTimeout($.mbMenu.deleteOnMouseOut);
				});
				$(opener.menuContainer).on("mouseleave",function(){
					var menuToRemove=$.mbMenu.options.actualMenuOpener;
					$.mbMenu.deleteOnMouseOut= setTimeout(function(){$(this).removeMbMenu(menuToRemove,true);$(document).off("click.closeMbMenu");},$(op)[0].options.closeAfter);
				});
			}

			//positioning opened - todo: manage fixed position.
			var t=0,l=0;
			$(this.menuContainer).css({
				minWidth:op.options.menuWidth
			});
			if ($.browser.msie) $(this.menuContainer).css("width",$(this.menuContainer).width()+2);

			switch(type){
				case "sm":
					t=$(this).position().top+op.options.submenuTop;
					l=$(this).position().left+$(this).width()-op.options.submenuLeft;
					break;
				case "cm":
					t=this.mouseY-5;
					l=this.mouseX-5;
					break;
				default:
					if (op.options.openOnRight){
						t=$(this).offset().top-($.browser.msie?2:0)+op.options.menuTop;
						l=$(this).offset().left+$(this).outerWidth()-op.options.menuLeft-($.browser.msie?2:0);
					}else{
						t=$(this).offset().top+$(this).outerHeight()-(!$.browser.mozilla?2:0)+op.options.menuTop;
						l=$(this).offset().left+op.options.menuLeft;
					}
					break;
			}

			$(this.menu).css({
				position:"absolute",
				top:t,
				left:l
			});

			if (!type || type=="cm") op.rootMenu=this.menu;
			$(this.menuContainer).on(mouseOut,function(){
				$(document).one("click.closeMbMenu",function(){$(document).removeMbMenu(op,true);});
			});

			if (op.options.fadeInTime>0) $(this.menuContainer).fadeIn(op.options.fadeInTime);
			else $(this.menuContainer).show();

			var wh= (op.options.containment=="window")?$(window).height():$("#"+op.options.containment).offset().top+$("#"+op.options.containment).outerHeight();
			var ww=(op.options.containment=="window")?$(window).width():$("#"+op.options.containment).offset().left+$("#"+op.options.containment).outerWidth();

			var mh=$(this.menuContainer).outerHeight();
			var mw=$(this.menuContainer).outerWidth();

			var actualX=$(where.find(".menuDiv:first")).offset().left-$(window).scrollLeft();
			var actualY=$(where.find(".menuDiv:first")).offset().top-$(window).scrollTop();

			switch(type){
				case "sm":
					if ((actualX+mw)>= ww && mw<ww){
						l-=((op.options.menuWidth*2)-(op.options.submenuLeft*2));
					}
					break;
				case "cm":
					if ((actualX+(op.options.menuWidth*1.5))>= ww && mw<ww){
						l-=((op.options.menuWidth)-(op.options.submenuLeft));
					}
					break;
				default:
					if ((actualX+mw)>= ww && mw<ww){
						l-=($(this.menuContainer).offset().left+mw)-ww+18;
					}
					break;
			}
			if ((actualY+mh)>= wh-10 && mh<wh){
				t-=((actualY+mh)-wh)+10;
			}

			$(this.menu).css({
				top:t,
				left:l
			});
		},

		removeMbMenu: function(op,fade){
			if(!op)op=$.mbMenu.options.actualMenuOpener;
			if(!op) return;
			if (op.rootMenu) {
				$(op.actualOpenedMenu)
						.removeAttr("isOpen")
						.removeClass("selected");
				$("[isOpen]").removeAttr("isOpen");
				$(op.rootMenu).css({width:1, height:1});
				if (fade) $(op.rootMenu).fadeOut(op.options.fadeOutTime,function(){$(this).remove();});
				else $(op.rootMenu).remove();
				op.rootMenu=false;
				op.clicked=false;
			}
		},

		//mouse  Position
		getMouseX : function (e){
			var mouseX;
			if ($.browser.msie)mouseX = e.clientX + document.documentElement.scrollLeft;
			else mouseX = e.pageX;
			if (mouseX < 0) mouseX = 0;
			return mouseX;
		},
		getMouseY : function (e){
			var mouseY;
			if ($.browser.msie)	mouseY = e.clientY + document.documentElement.scrollTop;
			else mouseY = e.pageY;
			if (mouseY < 0)	mouseY = 0;
			return mouseY;
		},
		//get max z-inedex of the page
		mb_bringToFront: function(zIndexContext){
			var zi=10;
			var els= zIndexContext && zIndexContext!="auto" ? $(zIndexContext):$("*");
			els.not(".alwaysOnTop").each(function() {
				if(!$(this).css("position")=="static"){
					var cur = parseInt($(this).css('zIndex'));
					zi = cur > zi ? parseInt($(this).css('zIndex')) : zi;
				}
			});
			$(this).not(".alwaysOnTop").css('zIndex',zi+=1);
			return zi;
		},
		mb_hover:function(hoverIntent, fn1, fn2){
			if(hoverIntent==0)
				$(this).hover(fn1,fn2);
			else
				$(this).hoverIntent({
					sensitivity: 30,
					interval: hoverIntent,
					timeout: 0,
					over:fn1,
					out:fn2
				});
		}
	});

	$.fn.buildMenu = $.mbMenu.buildMenu;
	$.fn.buildContextualMenu = $.mbMenu.buildContextualMenu;

})(jQuery);
