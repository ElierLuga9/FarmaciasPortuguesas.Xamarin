﻿using System;
using System.Threading.Tasks;
using ANFAPP.Logic;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Resources;
using ANFAPP.Pages;
using ANFAPP.Views;
using Xamarin.Forms;
using XLabs.Platform.Device;
using XLabs.Ioc;

namespace ANFAPP.Pages.PharmacyLocator
{
    public partial class KnowThisPharmacy : ANFPage
    {
        #region Properties
        
		private KnowThisFarmViewModel _viewModel;

        public Pharmacy _pharmacy;
        private KnowThisPharmacy() : base() { }

        private System.Collections.Generic.List<object> PharmHourList;
        private System.Collections.Generic.List<object> PharmNotAvailableList;

        public KnowThisPharmacy(Pharmacy p, bool isPharmOpen, System.Collections.Generic.List<object> pharmHourList, System.Collections.Generic.List<object> pharmNotAvailableList) : this()
        {
            this._pharmacy = p;
            _viewModel = new KnowThisFarmViewModel(p.Code);

			// Set the application bar title
			ApplicationBar.SetTitle(_pharmacy.Name);
            //if (PharmacyUtils.GetIsInService(_pharmacy.Code))
            if (isPharmOpen)
            {
				ApplicationBar.SetSubtitle("FARMÁCIA ABERTA");
			}
            else
            {
                ApplicationBar.SetSubtitle("FARMÁCIA FECHADA");
            }

            PharmHourList = pharmHourList;
            PharmNotAvailableList = pharmNotAvailableList;
        }

        #endregion

        #region Events
        async Task OnLoadStart()
        {
            MainContent.IsVisible = false;
            LoadingView.IsVisible = true;
        }

        void OnLoadSuccess()
        {
            Scheduler.IsVisible = PharmHourList != null && PharmHourList.Count > 0;
            if (Scheduler.IsVisible)
                HourList.ItemsSource = PharmHourList;
            
            contanctScheduleValue.IsVisible = _viewModel.PharmacyDetail.IsContactsAvailable && !Scheduler.IsVisible;
            contanctScheduleLabel.IsVisible = _viewModel.PharmacyDetail.IsContactsAvailable && !Scheduler.IsVisible;

            NotAvailableSeparator.IsVisible = NotAvailableList.IsVisible = NotAvailableLabel.IsVisible = PharmNotAvailableList != null && PharmNotAvailableList.Count > 0;
            if (NotAvailableList.IsVisible)
                NotAvailableList.ItemsSource = PharmNotAvailableList;
                
            BindingContext = _viewModel.PharmacyDetail;
            MainContent.IsVisible = true;
            
			ConfigureContentWebview();
            
			LoadingView.IsVisible = false;
        }

        void OnLoadError(string title, string message)
        {

            LoadingView.IsVisible = false;
            DisplayAlert(title, message, AppResources.OK);
        }

		void WebPageButtonClicked(object sender, EventArgs args)
		{
			try {
				Device.OpenUri(new Uri(_viewModel.PharmacyDetail.Url));
			} catch { /* Ignore this exception, because sometimes the WS will return dummy data. */ }
		}

		void GoToInfarmed(object sender, EventArgs args)
		{
			try {
				Device.OpenUri(new Uri("http://www.infarmed.pt/documents/15786/1559752/Listagem+das+farm%C3%A1cias+e+locais+de+venda+de+medicamentos+n%C3%A3o+sujeitos+a+receita+m%C3%A9dica+%28LVMNSRM%29+que+dispensam+medicamentos+ao+domic%C3%ADlio+ou+atrav%C3%A9s+da+internet/79c47e06-2d38-47f5-b0e5-7ed684613f91"));
			} catch { }
		}

        #endregion

        #region Application Bar Methods
        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        #endregion

        #region Page Lifecylce

		protected override void InitPage()
		{
			InitializeComponent();
			base.InitPage();

			BindingContext = _viewModel;
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _viewModel.OnLoadStart += OnLoadStart;
            _viewModel.OnLoadError += OnLoadError;
            _viewModel.OnLoadSuccess += OnLoadSuccess;

            _viewModel.LoadData();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _viewModel.OnLoadStart -= OnLoadStart;
            _viewModel.OnLoadError -= OnLoadError;
            _viewModel.OnLoadSuccess -= OnLoadSuccess;

            BindingContext = null;
        }
        #endregion

        #region Auxiliary Methods

        public bool ConfigureContentWebview()
        {
  
            string slideshowElements = "";
            var pageTemplate = "";
            if (_viewModel == null)  return false;

            if (_viewModel.PharmacyDetail.Images == null || _viewModel.PharmacyDetail.Images.Count == 0) 
            {
            
				ImagesGallery.IsVisible = false;
				PlaceholderImage.IsVisible = true;
                
//				pageTemplate = @"
//                    <html><head>
//                    <meta name=""viewport"" content=""width={0}; minimum-scale=1.0; maximum-scale=1.0; user-scalable=no"">
//                    <style type=""text/css"">
//	                    body {{
//		                    margin: 0;
//		                    padding: 0;
//                            height:{1};
//	                    }}
//
//                        no_image{{
//                            padding-top:50%;
//                            padding-left:50%;
//                            height:{1};
//                            width:{0};
//                        }}
//                    </style>
//                    </head>
//                        <body>
//                        <div class=""no_image"">Sem imagens Disponíveis.</div>
//   
//            </body>
//            </html>";

            }
            else 
            { 
              slideshowElements = "<div class=\"slideshow swipeshow\"><ul class=\"slides\">";
              foreach (FarmDetailImage img in _viewModel.PharmacyDetail.Images)
              {
                   slideshowElements += string.Format("<li class=\"slide\"><img src=\"{0}\"></li>", img.Url1);
              }
              slideshowElements += "</ul><div class=\"dots\"></div></div>";

                pageTemplate = @"
            <html><head>
            <meta name=""viewport"" content=""width={0}, height={1}, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0"" />
            <script src=""https://ajax.googleapis.com/ajax/libs/jquery/2.1.4/jquery.min.js""></script>
            <script type=""text/javascript"">
                (function($){{$.swipeshow={{}};$.swipeshow.version=""0.10.9"";var transitions=typeof $(""<div>"").css({{transition:""all""}}).css(""transition"")==""string"";var touchEnabled=""ontouchstart""in document.documentElement;var has3d=function(){{var div=$(""<div>"");div.css(""transform"",""translate3d(0,0,0)"");return div.css(""transform"")!==""""}}();var instances=0;function Swipeshow(element,options){{this.$slideshow=$(element);this.$container=this.$slideshow.find(""> .slides"");this.$slides=this.$container.find(""> .slide"");this.options=options;this.tag="".swipeshow.swipeshow-""+ ++instances;this.disabled=false;this.$next=getElement(this.$slideshow,options.$next,"".next"",""~ .controls .next"");this.$previous=getElement(this.$slideshow,options.$previous,"".previous"",""~ .controls .previous"");this.$dots=getElement(this.$slideshow,options.$dots,"".dots"",""~ .controls .dots"");this._addClasses();this._bindButtons();this._buildDots();if(options.keys)this._bindKeys();this.cycler=this._getCycler();if(options.autostart!==false)this._startSlideshow();this._bindSwipeEvents();this._bindHoverPausing();this._bindResize();return this}}Swipeshow.prototype={{goTo:function(n){{this.cycler.goTo(n);return this}},previous:function(){{this.cycler.previous();return this}},next:function(){{this.cycler.next();return this}},pause:function(){{this.cycler.pause();return this}},start:function(){{this.cycler.start();return this}},isStarted:function(){{return this.cycler&&this.cycler.isStarted()}},isPaused:function(){{return!this.isStarted()}},defaults:{{speed:400,friction:.3,mouse:true,keys:true,swipeThreshold:{{distance:10,time:400}}}},unbind:function(){{var $slideshow=this.$slideshow;var $container=this.$container;var $slides=this.$slides;var $dots=this.$dots;var tag=this.tag;this.cycler.pause();$container.find(""img"").off(tag);$container.off(tag);$(document).off(tag);$(window).off(tag);if($dots.length)$dots.html("""");$slideshow.data(""swipeshow"",null);$slideshow.removeClass(""running paused swipeshow-active touch no-touch"");$container.removeClass(""gliding grabbed"");$slides.removeClass(""active"");$dots.removeClass(""active"");$(""html"").removeClass(""swipeshow-grabbed"")}},_getCycler:function(){{var ss=this;var options=this.options;return new Cycler(ss.$slides,$.extend({{}},options,{{autostart:false,onactivate:$.proxy(this._onactivate,this),onpause:$.proxy(this._onpause,this),onstart:$.proxy(this._onstart,this)}}))}},_onactivate:function(current,i,prev,j){{if(this.options.onactivate)this.options.onactivate(current,i,prev,j);if(prev)$(prev).removeClass(""active"");if(current)$(current).addClass(""active"");if(this.$dots.length){{this.$dots.find("".dot-item.active"").removeClass(""active"");this.$dots.find('.dot-item[data-index=""'+i+'""]').addClass(""active"")}}this._moveToSlide(i)}},_moveToSlide:function(i){{var width=this.$slideshow.width();setOffset(this.$container,-1*width*i,this.options.speed)}},_onpause:function(){{if(this.options.onpause)this.options.onpause();this.$slideshow.addClass(""paused"").removeClass(""running"")}},_onstart:function(){{if(this.options.onstart)this.options.onstart();this.$slideshow.removeClass(""paused"").addClass(""running"")}},_addClasses:function(){{this.$slideshow.addClass(""paused swipeshow-active"");this.$slideshow.addClass(touchEnabled?""touch"":""no-touch"")}},_buildDots:function(){{var ss=this;var $dots=ss.$dots;var tag=ss.tag;if(!$dots.length)return;$dots.html("""").addClass(""active"");ss.$slides.each(function(i){{$dots.append($(""<button class='dot-item' data-index='""+i+""'>""+""<span class='dot' data-number='""+(i+1)+""'></span>""+""</button>""))}});$dots.on(""click""+tag,"".dot-item"",function(){{var index=+$(this).data(""index"");ss.goTo(index)}})}},_bindKeys:function(){{var ss=this;var tag=ss.tag;var RIGHT=39,LEFT=37;$(document).on(""keyup""+tag,function(e){{if(e.keyCode==RIGHT)ss.next();else if(e.keyCode==LEFT)ss.previous()}})}},_bindButtons:function(){{var ss=this;this.$next.on(""click"",function(e){{e.preventDefault();if(!ss.disabled)ss.next()}});this.$previous.on(""click"",function(e){{e.preventDefault();if(!ss.disabled)ss.previous()}})}},_startSlideshow:function(){{var ss=this;var $images=ss.$slideshow.find(""img"");if($images.length===0){{ss.start()}}else{{ss.disabled=true;ss.$slideshow.addClass(""disabled"");$images.onloadall(function(){{ss.disabled=false;ss.$slideshow.removeClass(""disabled"");ss.start()}})}}}},_bindResize:function(){{var ss=this;$(window).on(""resize""+ss.tag,function(){{var width=ss.$slideshow.width();setOffset(ss.$container,-1*width*ss.cycler.current,0);ss._reposition()}});$(window).trigger(""resize""+ss.tag)}},_reposition:function(){{var width=this.$slideshow.width();var count=this.$slides.length;this.$slides.css({{width:width}});this.$container.css({{width:width*count}});this.$slides.css({{visibility:""visible""}});this.$slides.each(function(i){{$(this).css({{left:width*i}})}})}},_bindHoverPausing:function(){{if(touchEnabled)return;var ss=this;var tag=ss.tag;var hoverPaused=false;ss.$slideshow.on(""mouseenter""+tag,function(){{if(!ss.isStarted())return;hoverPaused=true;ss.pause()}});ss.$slideshow.on(""mouseleave""+tag,function(){{if(!hoverPaused)return;hoverPaused=false;ss.start()}})}},_bindSwipeEvents:function(){{var ss=this;var $slideshow=ss.$slideshow;var $container=ss.$container;var c=ss.cycler;var options=ss.options;var tag=ss.tag;var moving=false;var origin;var start;var delta;var lastTouch;var minDelta;var width;var length=c.list.length;var friction=options.friction;$slideshow.data(""swipeshow:tag"",tag);$container.find(""img"").on(""mousedown""+tag,function(e){{e.preventDefault()}});$container.on(""touchstart""+tag+(options.mouse?"" mousedown""+tag:""""),function(e){{if(isFlash(e))return;if(e.type===""mousedown"")e.preventDefault();if(ss.disabled)return;if($container.is("":animated""))$container.stop();if($(e.target).is(""button, a, input, select, [data-tappable]"")){{minDelta=100}}else{{minDelta=0}}$container.addClass(""grabbed"");$(""html"").addClass(""swipeshow-grabbed"");width=$slideshow.width();moving=true;origin={{x:null}};start={{x:getOffset($container),started:c.isStarted()}};delta=0;lastTouch=null;if(start.started)c.pause()}});$(document).on(""touchmove""+tag+(options.mouse?"" mousemove""+tag:""""),function(e){{if(ss.disabled)return;if($container.is("":animated""))return;if(!moving)return;var x=getX(e);if(isNaN(x))return;if(origin.x===null)origin.x=x;delta=x-origin.x;if(Math.abs(delta)<=minDelta)delta=0;var target=start.x+delta;var max=-1*width*(length-1);if(Math.abs(delta)>3)e.preventDefault();if(target>0)target*=friction;if(target<max)target=max+(target-max)*friction;lastTouch=+new Date;setOffset($container,target,0)}});$(document).on(""touchend""+tag+(options.mouse?"" mouseup""+tag:""""),function(e){{if(ss.disabled)return;if($container.is("":animated""))return;if(!moving)return;if(isFlash(e))return;var left=getOffset($container);$container.removeClass(""grabbed"");$(""html"").removeClass(""swipeshow-grabbed"");var index=-1*Math.round(left/width);if(lastTouch&&c.current===index){{var timeDelta=+new Date-lastTouch;var threshold=options.swipeThreshold;if(Math.abs(delta)>threshold.distance&&timeDelta<threshold.time){{var sign=delta<0?-1:1;index-=sign}}}}if(index<0)index=0;if(index>c.list.length-1)index=c.list.length-1;c.goTo(index);if(start.started)c.start();moving=false}})}}}};$.fn.swipeshow=function(options){{if(!options)options={{}};options=$.extend({{}},Swipeshow.prototype.defaults,options);$(this).each(function(){{if($(this).data(""swipeshow""))return;var ss=new Swipeshow(this,options);$(this).data(""swipeshow"",ss)}});return $(this).data(""swipeshow"")}};$.fn.unswipeshow=function(){{return this.each(function(){{var ss=$(this).data(""swipeshow"");if(ss)ss.unbind()}})}};function getElement(root){{var arg;for(var i=1;i<arguments.length;++i){{arg=arguments[i];if(typeof arg===""string""){{var $el=$(arg,root);if($el.length)return $el}}else if(typeof arg===""object""&&arg.constructor===$&&arg.length){{return arg}}}}return $()}}var offsetTimer;function setOffset($el,left,speed){{$el.data(""swipeshow:left"",left);if(transitions){{if(speed===0){{$el.css({{transform:translate(left,0),transition:""none""}})}}else{{$el.css({{transform:translate(left,0),transition:""all ""+speed+""ms ease""}})}}}}else{{if(speed===0){{$el.css({{left:left}})}}else{{$el.animate({{left:left}},speed)}}}}$el.addClass(""gliding"");if(typeof offsetTimer===""undefined"")clearTimeout(offsetTimer);offsetTimer=setTimeout(function(){{$el.removeClass(""gliding"");offsetTimer=undefined}},speed)}}function translate(x,y){{if(has3d){{return""translate3d(""+x+""px,""+y+""px,0)""}}else{{return""translate(""+x+""px,""+y+""px)""}}}}function getOffset($el){{return $el.data(""swipeshow:left"")||0}}function getX(e){{if(e.originalEvent&&e.originalEvent.touches&&e.originalEvent.touches[0])return e.originalEvent.touches[0].clientX;if(e.clientX)return e.clientX}}function isFlash(e){{return $(e.target).is(""embed, object"")}}}})(jQuery);(function($){{$.fn.onloadall=function(callback){{var $images=this;var images={{loaded:0,total:$images.length}};$images.on(""load.onloadall"",function(){{if(++images.loaded>=images.total){{callback.apply($images)}}}});$(function(){{$images.each(function(){{if(this.complete)$(this).trigger(""load.onloadall"")}})}});return this}}}})(jQuery);(function(){{function Cycler(list,options){{this.interval=options.interval||3e3;this.onactivate=options.onactivate||function(){{}};this.onpause=options.onpause||function(){{}};this.onstart=options.onstart||function(){{}};this.initial=typeof options.initial===""undefined""?0:options.initial;this.autostart=typeof options.autostart===""undefined""?true:options.autostart;this.list=list;this.current=null;this.goTo(this.initial);if(this.autostart&&typeof options.interval===""number"")this.start();return this}}Cycler.prototype={{start:function(silent){{var self=this;if(!self.isStarted()&&!silent)self.onstart.apply(self);self.pause(true);self._timer=setTimeout(function(){{self.next()}},self.interval);return self}},pause:function(silent){{if(this.isStarted()){{if(!silent)this.onpause.apply(this);clearTimeout(this._timer);this._timer=null}}return this}},restart:function(silent){{if(this.isStarted())this.pause(true).start(silent);return this}},previous:function(){{return this.next(-1)}},isStarted:function(){{return!!this._timer}},isPaused:function(){{return!this.isStarted()}},next:function(i){{if(typeof i===""undefined"")i=1;var len=this.list.length;if(len===0)return this;var idx=(this.current+i+len*2)%len;return this.goTo(idx)}},goTo:function(idx){{if(typeof idx!==""number"")return this;var prev=this.current;this.current=idx;this.onactivate.call(this,this.list[idx],idx,this.list[prev],prev);this.restart(true);return this}}}};window.Cycler=Cycler}})();
            </script>
            <style type=""text/css"">
	            body {{
		            margin: 0;
		            padding: 0;
					overflow: hidden;
					-ms-content-zooming: none;
					height: {1};
	            }}
	            img {{
		            width: 100%;
	            }}

	            .swipeshow,
	            .swipeshow .slides,
	            .swipeshow .slide {{
  	            /* Basic resets */
  	            display: block;
  	            margin: 0;
  	            padding: 0;
  	            list-style: none;
  	            position: relative;

  	            /* Prevent flickers */
  	            -webkit-transform: translate3d(0, 0, 0);
  	            -moz-transform: translate3d(0, 0, 0);
  	            -ms-transform: translate3d(0, 0, 0);
  	            -o-transform: translate3d(0, 0, 0);
  	            transform: translate3d(0, 0, 0);

  	            /* In case you add padding */
  	            -webkit-box-sizing: border-box;
  	            -moz-box-sizing: border-box;
  	            -ms-box-sizing: border-box;
  	            -o-box-sizing: border-box;
  	            box-sizing: border-box;
	            }}

	            /* iOS: disable text select, disable callout, image save panel (popup) */
	            .swipeshow img {{
  	            -webkit-touch-callout: none; 
  	            -webkit-user-select: none;   
  	            -moz-user-select: none;   
  	            user-select: none;   
	            }}

	            .swipeshow {{
  	            overflow: hidden;
	            }}

	            /* Cursor (open-hand) */
	            .swipeshow-active .slides {{
  	            cursor: -moz-grab !important;
  	            cursor: ew-resize;
	            }}

	            /* Cursor (closed-hand) */
	            html.swipeshow-grabbed,
	            html.swipeshow-grabbed * {{
  	            cursor: -moz-grabbing !important;
  	            cursor: ew-resize;
	            }}

	            /* Positioning */
	            .swipeshow .slides,
	            .swipeshow .slide {{
  	            position: absolute;
  	            top: 0;
  	            left: 0;
  	            width: 100%;
  	            height: 100%;
	            }}

	            /* First slide should be visible by default */
	            .swipeshow .slide {{
  	            visibility: hidden;
	            }}

	            .swipeshow .slide:first-child {{
  	            visibility: visible;
	            }}

	            /* Defaults: OVERRIDE THIS! */
	            .swipeshow {{
  	            width: {0}px;
  	            height: {1}px;
	            }}

	            .slideshow .dots,
	            .slideshow .dot-item,
	            .slideshow .dot {{
  	            margin: 0;
  	            padding: 0;
  	            list-style: none;

  	            border: 0;
  	            background: transparent;
	            }}

	            .slideshow .dots {{
  	            position: absolute;
  	            bottom: 16px;
  	            left: 0;
  	            right: 8px;
  	            text-align: right;
  	            list-style: none;

  	            font-size: 0;
	            }}

	            .slideshow .dot-item {{
  	            list-style: none;
  	            display: inline-block;

  	            width: 18px;
  	            height: 20px;
  	            line-height: 20px;
  	            text-align: center;

  	            cursor: pointer;

  	            opacity: 0.9;
	            }}

	            .slideshow .dot {{
  	            display: inline-block;
  	            width: 5px;
  	            height: 5px;
  	            border-radius: 6px;
  	            font-size: 0;

  	            border: solid 1px rgba(255, 255, 255, 0.8);
  	            box-shadow: 0 0 8px rgba(0, 0, 0, 0.3);

  	            background-color: rgba(0, 0, 0, 0.5);
	            }}

	            .no-touch .dot-item:hover .dot {{
  	            background-color: rgba(255, 255, 255, 0.2);
	            }}

	            .slideshow .dot-item:active .dot,
	            .slideshow .dot-item.active .dot {{
  	            background-color: white;
	            }}

                .no_image
                {{
                    heigth:{1};
                    visibility:visible;
                }}

                .with_images
                {{
                    height=0;
                    visibility:none;
                }}

            </style>
            </head>
                <body>
                {2}
            <script>
	            $("".slideshow"").swipeshow({{
					friction: 0,
					interval: 5000,
  		            $dots: $(""div.dots"")
	            }});
            </script>
            </body>
            </html>";
            
            }

			var display = Resolver.Resolve<IDisplay>();
			var width = ImagesGallery.Width <= 0 ? display.Width : ImagesGallery.Width;

			var page = string.Format(pageTemplate, width, DimenResources.BannerHeight, slideshowElements);
			//var source = new HtmlWebViewSource();
			//source.Html = page;
			//ImagesGallery.Source = source;
			ImagesGallery.CustomSource = page;

            return true;
        }

        #endregion

    }
}
