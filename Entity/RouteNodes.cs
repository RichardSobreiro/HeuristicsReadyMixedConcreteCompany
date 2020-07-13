using System;

namespace Entity
{
    public class RouteNodes
    {
        public int RouteNodeId { get; set; }
        public int RouteId { get; set; }
        public DateTime BeginService { get; set; }
        public DateTime EndService { get; set; }
    }
}
