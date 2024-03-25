class Router {
  constructor() {
    this.routes = {
      "lli-view" : "../LLIManagementPage/index.html",
      "notes-view": "../PersonalNotePage/index.html"
    };
  }
  
    // go to a specific route.
    goTo = (route) => {
      window.location = route;
    };
  
    // set default route.
    setDefault = (route) => {
      this.defaultRoute = route;
    };

    navigatePages = () =>
    {
      // attach routes to navbar elements
      const element = (id) => document.getElementById(id);

      const registerClickHandler = (id, route) =>
      {
          element(id).addEventListener("click", ()=> 
          {
           this.goTo(route);
          })
      }

      for (let id in this.routes) {
        const route = this.routes[id];
        registerClickHandler(id, route);
      }
    }
  }

  export default Router;