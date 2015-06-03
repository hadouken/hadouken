using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Hadouken.Core.Http.WebSockets {
    using WebSocketCloseAsync =
        Func
            <
                int /* closeStatus */,
                string /* closeDescription */,
                CancellationToken /* cancel */,
                Task
                >;
    using WebSocketReceiveAsync =
        Func
            <
                ArraySegment<byte> /* data */,
                CancellationToken /* cancel */,
                Task
                    <
                        Tuple
                            <
                                int /* messageType */,
                                bool /* endOfMessage */,
                                int /* count */
                                >
                        >
                >;
    using WebSocketSendAsync =
        Func
            <
                ArraySegment<byte> /* data */,
                int /* messageType */,
                bool /* endOfMessage */,
                CancellationToken /* cancel */,
                Task
                >;

    internal class OwinWebSocket : WebSocket {
        private readonly WebSocketCloseAsync _closeAsync;
        private readonly WebSocketReceiveAsync _receiveAsync;
        private readonly WebSocketSendAsync _sendAsync;
        private WebSocketCloseStatus? _closeStatus;
        private string _closeStatusDescription;

        public OwinWebSocket(IDictionary<string, object> environment) {
            this._sendAsync = (WebSocketSendAsync) environment["websocket.SendAsync"];
            this._receiveAsync = (WebSocketReceiveAsync) environment["websocket.ReceiveAsync"];
            this._closeAsync = (WebSocketCloseAsync) environment["websocket.CloseAsync"];
        }

        public override WebSocketCloseStatus? CloseStatus {
            get { return this._closeStatus; }
        }

        public override string CloseStatusDescription {
            get { return this._closeStatusDescription ?? string.Empty; }
        }

        public override string SubProtocol {
            get { throw new NotImplementedException(); }
        }

        public override WebSocketState State {
            get { throw new NotImplementedException(); }
        }

        public override void Abort() {}

        public override Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription,
            CancellationToken cancellationToken) {
            this._closeStatusDescription = statusDescription;
            this._closeStatus = closeStatus;
            return this._closeAsync((int) closeStatus, statusDescription, cancellationToken);
        }

        public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription,
            CancellationToken cancellationToken) {
            return this.CloseAsync(closeStatus, statusDescription, cancellationToken);
        }

        public override void Dispose() {}

        public override async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer,
            CancellationToken cancellationToken) {
            var tuple = await this._receiveAsync(buffer, cancellationToken);

            var messageType = tuple.Item1;
            var endOfMessage = tuple.Item2;
            var count = tuple.Item3;

            return new WebSocketReceiveResult(count, OpCodeToEnum(messageType), endOfMessage);
        }

        public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage,
            CancellationToken cancellationToken) {
            return this._sendAsync(buffer, EnumToOpCode(messageType), endOfMessage, cancellationToken);
        }

        private static WebSocketMessageType OpCodeToEnum(int messageType) {
            switch (messageType) {
                case 0x1:
                    return WebSocketMessageType.Text;
                case 0x2:
                    return WebSocketMessageType.Binary;
                case 0x8:
                    return WebSocketMessageType.Close;
                default:
                    throw new ArgumentOutOfRangeException("messageType", messageType, string.Empty);
            }
        }

        private static int EnumToOpCode(WebSocketMessageType webSocketMessageType) {
            switch (webSocketMessageType) {
                case WebSocketMessageType.Text:
                    return 0x1;
                case WebSocketMessageType.Binary:
                    return 0x2;
                case WebSocketMessageType.Close:
                    return 0x8;
                default:
                    throw new ArgumentOutOfRangeException("webSocketMessageType", webSocketMessageType, string.Empty);
            }
        }
    }
}