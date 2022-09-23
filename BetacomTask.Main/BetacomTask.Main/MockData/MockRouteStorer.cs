using BetacomTask.Main.Contracts;

namespace BetacomTask.Main.MockData
{
    public class MockRouteStorer : IRouteStorer
    {

        public List<Route> Routes  { get; set; } 

        public MockRouteStorer()
        {
            Routes = new List<Route>();
        }


    }
}
