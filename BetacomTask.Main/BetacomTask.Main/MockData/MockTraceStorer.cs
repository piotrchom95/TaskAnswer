using BetacomTask.Main.Contracts;

namespace BetacomTask.Main.MockData
{
    public class MockTraceStorer : IGpsTraceStorer
    {
        public List<GpsTrace> Traces { get; set; }  

        public MockTraceStorer()
        {
            Traces = new List<GpsTrace>();



        }
    }
}
