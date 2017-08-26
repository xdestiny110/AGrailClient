using Framework;
using Framework.Network;
using Framework.Message;

namespace AGrail
{
    public class UserData : Singleton<UserData>, IMessageListener<MessageType>
    {
        private LoginState state = LoginState.Prepare;

        public string UserName { get; private set; }
        public bool IsVIP { get; private set; }

        public LoginState State
        {
            get { return state; }
            private set
            {
                state = value;
                MessageSystem<MessageType>.Notify(MessageType.LoginState);
            }
        }

        public UserData() : base()
        {
            MessageSystem<MessageType>.Regist(MessageType.LOGINRESPONSE, this);
            if (GameManager.TCPInstance.Connected)
                State = LoginState.Ready;
            else
            {
                State = LoginState.Prepare;
                MessageSystem<MessageType>.Regist(MessageType.OnConnect, this);
            }
        }

        public void Login(string userName, string password)
        {
            var request = new network.LoginRequest() { asGuest = false, user_id = userName, user_password = password, version = GameManager.Version };
            GameManager.TCPInstance.Send(new Protobuf() { Proto = request, ProtoID = ProtoNameIds.LOGINREQUEST });
        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.OnConnect:
                    State = LoginState.Ready;
                    break;
                case MessageType.LOGINRESPONSE:
                    var proto = parameters[0] as network.LoginResponse;
                    switch(proto.state)
                    {
                        case 0:
                            IsVIP = false;
                            State = LoginState.Success;
                            break;
                        case 4:
                            IsVIP = true;
                            State = LoginState.Success;
                            break;
                        case 1:
                            State = LoginState.Wrong;
                            break;
                        case 2:
                            State = LoginState.Forbidden;
                            break;
                        case 3:
                            State = LoginState.Update;
                            break;
                    }
                    break;
            }
        }
    }

    public enum LoginState
    {
        Prepare,
        Ready,
        Wrong,
        Forbidden,
        Update,
        Logining,
        Success,
    }
}


