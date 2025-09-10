using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace eTurns.DAL
{
    [HubName("eTurnsHub")]
    public class eTurnsHub : Hub
    {
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();
        public static List<string> Users = new List<string>();
        //public static List<UserMasterLoginDTO> objUserMasterLoginDTOList = new List<UserMasterLoginDTO>();
        public static List<string> objUserList = new List<string>();
        public void UpdateRedCircleCount()
        {
            Clients.All.UpdateRedCircleCountInClients();
        }

        public void Send(int count, List<string> userList)
        {
            // Call the addNewMessageToPage method to update clients.
            var context = GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>();
            context.Clients.All.updateUsersOnlineCount(count, userList);
        }

        public Task JoinRoom(string roomName)
        {
            return Groups.Add(Context.ConnectionId, roomName);
        }

        public Task LeaveRoom(string roomName)
        {
            return Groups.Remove(Context.ConnectionId, roomName);
        }

        public override Task OnConnected()
        {
            //long EID, CID, RID;

            //long.TryParse(Convert.ToString(HttpContext.Current.Session["RoomID"]), out RID);
            //long.TryParse(Convert.ToString(HttpContext.Current.Session["CompanyID"]), out CID);
            //long.TryParse(Convert.ToString(HttpContext.Current.Session["EnterPriceID"]), out EID);

            if (Context.User != null && Context.User.Identity != null && !string.IsNullOrWhiteSpace(Context.User.Identity.Name))
            {
                string name = Convert.ToString(Context.User.Identity.Name);
                _connections.Add(name, Context.ConnectionId);
            }

            string clientId = GetClientId();

            if (Users.IndexOf(clientId) == -1)
            {
                objUserList.Add(clientId);
                Users.Add(clientId);
            }

            // Send the current count of users
            Send(Users.Count, objUserList);

            //if (EID > 0 && CID > 0 && RID > 0)
            //{
            //    Groups.Add(Context.ConnectionId, EID + "_" + CID + "_" + RID);
            //}


            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            //long EID, CID, RID;

            //long.TryParse(Convert.ToString(HttpContext.Current.Session["RoomID"]), out RID);
            //long.TryParse(Convert.ToString(HttpContext.Current.Session["CompanyID"]), out CID);
            //long.TryParse(Convert.ToString(HttpContext.Current.Session["EnterPriceID"]), out EID);


            if (Context.User != null && Context.User.Identity != null && !string.IsNullOrWhiteSpace(Context.User.Identity.Name))
            {
                string name = Convert.ToString(Context.User.Identity.Name);
                _connections.Remove(name, Context.User.Identity.Name);
            }

            string clientId = GetClientId();

            if (Users.IndexOf(clientId) > -1)
            {
                Users.Remove(clientId);
                objUserList.Remove(clientId);
            }

            // Send the current count of users
            Send(Users.Count, objUserList);

            //if (EID > 0 && CID > 0 && RID > 0)
            //{
            //    Groups.Remove(Context.ConnectionId, EID + "_" + CID + "_" + RID);
            //}

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            //long EID, CID, RID;

            //long.TryParse(Convert.ToString(HttpContext.Current.Session["RoomID"]), out RID);
            //long.TryParse(Convert.ToString(HttpContext.Current.Session["CompanyID"]), out CID);
            //long.TryParse(Convert.ToString(HttpContext.Current.Session["EnterPriceID"]), out EID);


            if (Context.User != null && Context.User.Identity != null && !string.IsNullOrWhiteSpace(Context.User.Identity.Name))
            {
                string name = Convert.ToString(Context.User.Identity.Name);

                if (!_connections.GetConnections(name).Contains(Context.User.Identity.Name))
                {
                    _connections.Add(name, Context.User.Identity.Name);
                }
            }


            string clientId = GetClientId();
            if (Users.IndexOf(clientId) == -1)
            {
                Users.Add(clientId);
                objUserList.Add(clientId);
            }

            // Send the current count of users
            Send(Users.Count, objUserList);

            //if (EID > 0 && CID > 0 && RID > 0)
            //{
            //    Groups.Add(Context.ConnectionId, EID + "_" + CID + "_" + RID);
            //}

            return base.OnReconnected();
        }

        private string GetClientId()
        {
            //string str = HttpContext.Current.Session.SessionID;
            //string str = HttpContext.Current.Request.Cookies["ASP.NET_SessionId"].Value;
            string clientId = "";
            if (Context.QueryString["clientId"] != null)
            {
                // clientId passed from application 
                clientId = this.Context.QueryString["clientId"];
            }

            if (string.IsNullOrEmpty(clientId.Trim()))
            {
                clientId = Context.ConnectionId;
            }

            //return clientId;
            return Context.User.Identity.Name;
        }

    }


    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, HashSet<string>> _connections =
            new Dictionary<T, HashSet<string>>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
    }

}