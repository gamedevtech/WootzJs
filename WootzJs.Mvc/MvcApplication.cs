﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.WootzJs;
using System.Threading.Tasks;
using System.Web;
using WootzJs.Models;
using WootzJs.Mvc.Routes;
using WootzJs.Mvc.Views;
using WootzJs.Mvc.Views.Css;
using WootzJs.Web;

namespace WootzJs.Mvc
{
    public class MvcApplication
    {
        public event Action BottomBounced;

        public string Host { get; set; }
        public string Port { get; set; }
        public string Scheme { get; set; }
        public GlobalStyle GlobalStyle { get; private set; }
        public NavigationContext NavigationContext { get; private set; }

        public IControllerFactory ControllerFactory { get; protected set; }
        public IDependencyResolver DependencyResolver { get; protected set; }

        private RouteTree routeTree;
        private View view;
        private HtmlControl html = new HtmlControl(Browser.Document.GetElementByTagName("html"));
        private HtmlControl body = new HtmlControl(Browser.Document.GetElementByTagName("body"));
        private string initialPath = Browser.Window.Location.PathName;
        private string currentPath;
        private UrlHelper url = new UrlHelper();
        private ActionHelper action = new ActionHelper();

        private static MvcApplication instance;

        public MvcApplication()
        {
            instance = this;
            DependencyResolver = new ReflectionDependencyResolver();
        }

        public static MvcApplication Instance
        {
            get { return instance; }
        }

        public HtmlControl Html
        {
            get { return html; }
        }

        public View View
        {
            get { return view; }
        }

        public string CurrentPath
        {
            get { return currentPath ?? initialPath; }
        }

        public HtmlControl Body
        {
            get { return body; }
        }

        public UrlHelper Url
        {
            get { return url; }
        }

        public ActionHelper Action
        {
            get { return action; }
        }

        public string CurrentUrl
        {
            get { return Browser.Window.Location.PathName + (!string.IsNullOrEmpty(Browser.Window.Location.Search) ? Browser.Window.Location.Search : ""); }
        }

        public async void Start(Assembly assembly)
        {
            Host = Browser.Window.Location.Host;
            Port = Browser.Window.Location.Port;
            Scheme = Browser.Window.Location.Protocol;

            Browser.Window.AddEventListener("popstate", evt => OnPopState(evt.As<PopStateEvent>()));

            var path = Browser.Window.Location.PathName;
            Console.WriteLine(path);

            ControllerFactory = new DefaultControllerFactory(DependencyResolver);

            var routeGenerator = new RouteGenerator();
            routeTree = routeGenerator.GenerateRoutes(assembly);

            await OnStarting();
            await OnStarted();
        }

        /// <summary>
        /// This occurs immediately before opening the initial page.
        /// </summary>
        protected virtual async Task OnStarting()
        {
            await InitializeGlobalStyle();
        }

        protected virtual async Task OnStarted()
        {
            await Open(CurrentUrl, false);
        }

        protected virtual async Task InitializeGlobalStyle()
        {
            var style = Browser.Document.CreateElement("style");
            style.AppendChild(Browser.Document.CreateTextNode(""));  // Webkit hack
            Browser.Document.Head.AppendChild(style);
            var styleSheet = style.As<JsObject>()["sheet"].As<StyleSheet>();
            GlobalStyle = new GlobalStyle(styleSheet);
        }

        private async void OnPopState(PopStateEvent evt)
        {
            // If state is null then it means it's firing on first load, which we never care about
            var path = (string)evt.State;
            if (path != null && path != currentPath)
                await Open(path, false);
        }

        public Task Open(string url)
        {
            return Open(url, true);
        }

        public async Task Open(string url, bool pushState)
        {
            var parts = url.Split('?');
            var path = parts[0];
            var queryString = url.Length > 1 ? parts[1] : null;
            currentPath = path;
            var view = await Execute(path, queryString);
            if (pushState)
                Browser.Window.History.PushState(url, view.Title, url);

            Open(view);
            OnOpen(url);
        }

        public void Open(View view)
        {
            Browser.Document.Title = view.Title;

            if (this.view is Layout && view.LayoutType != null)
            {
                var layout = (Layout)this.view;
                var container = layout.FindLayout(view.LayoutType);
                container.AddView(view);
            }
            else
            {
                if (this.view != null)
                {
                    this.view.NotifyViewDetached();
                    body.Remove(this.view.Content);
                }

                var rootView = view.GetRootView();
                rootView.NotifyViewAttached();
                body.Add(rootView.Content);
                this.view = rootView;
            }

            if (this.view is Layout)
            {
                var layout = (Layout)this.view;
                var sections = view.Sections;
                layout.LoadSections(sections);
//                foreach (var section in sections.Values)
//                {
//                    section.NotifyOnAddedToView();
//                }
            }            
        }

        protected virtual void OnOpen(string url)
        {
        }

        private NavigationContext CreateNavigationContext(string path, string queryString)
        {
            // Create http context
            var queryStringDictionary = new Dictionary<string, string>();
            if (queryString != null)
            {
                var parts = queryString.Split(new[] { '&' });
                foreach (var part in parts)
                {
                    var pair = part.Split(new[] { '=' });
                    var key = HttpUtility.UrlDecode(pair[0]);
                    var value = HttpUtility.UrlDecode(pair[1]);
                    queryStringDictionary[key] = value;
                }
            }
            var routeData = routeTree.Apply(path, "GET");
            var request = new NavigationRequest
            {
                Path = path,
                QueryString = queryStringDictionary,
                RouteData = routeData
            };
            var response = new NavigationResponse();

            var navigationContext = new NavigationContext
            {
                Request = request,
                Response = response
            };

            return navigationContext;
        }

        protected async Task<View> Execute(string path, string queryString)
        {
            var context = CreateNavigationContext(path, queryString);
            NavigationContext = context;
            var controller = ControllerFactory.CreateController(context);
            await controller.Execute(this, context);
            return context.Response.View;
        }

        public virtual ViewContext CreateViewContext(Controller controller)
        {
            return new ViewContext { ControllerContext = controller.ControllerContext };
        }

        internal void NotifyOnValidatedControl(Control control, Failure[] failures)
        {
            OnValidatedControl(control, failures);
        }

        protected virtual void OnValidatedControl(Control control, Failure[] failures)
        {
        }

        internal void NotifyBindModel(Controller controller, Model model)
        {
            OnBindModel(controller, model);
        }

        protected virtual void OnBindModel(Controller controller, Model model)
        {
        }

        internal void NotifyOnBottomBounced()
        {
            OnBottomBounced();
        }

        protected virtual void OnBottomBounced()
        {
            if (BottomBounced != null)
                BottomBounced();
        }
    }
}