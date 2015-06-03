// LICENSE: MIT
// Author: github.com/bryceg
// Modifications by github.com/vktr

using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Hadouken.Core.Http.WebSockets.Extensions;
using Microsoft.Owin;

namespace Hadouken.Core.Http.WebSockets {
    public abstract class WebSocketConnection {
        private readonly CancellationTokenSource _cancellationToken;
        private readonly TaskQueue _sendQueue;
        private WebSocket _webSocket;

        protected WebSocketConnection(int maxMessageSize = 1024*64) {
            this._sendQueue = new TaskQueue();
            this._cancellationToken = new CancellationTokenSource();
            this.MaxMessageSize = maxMessageSize;
        }

        /// <summary>
        ///     Maximum message size in bytes for the receive buffer
        /// </summary>
        public int MaxMessageSize { get; private set; }

        /// <summary>
        ///     Arguments captured from URI using Regex
        /// </summary>
        public Dictionary<string, string> Arguments { get; private set; }

        /// <summary>
        ///     Queue of send operations to the client
        /// </summary>
        public TaskQueue QueueSend {
            get { return this._sendQueue; }
        }

        /// <summary>
        ///     Closes the websocket connection
        /// </summary>
        /// <returns></returns>
        public Task Close(WebSocketCloseStatus status, string reason) {
            return this._webSocket.CloseAsync(status, reason, CancellationToken.None);
        }

        /// <summary>
        ///     Sends data to the client with binary message type
        /// </summary>
        /// <param name="buffer">Data to send</param>
        /// <param name="endOfMessage">End of the message?</param>
        /// <returns>Task to send the data</returns>
        public Task SendAsyncBinary(byte[] buffer, bool endOfMessage) {
            return this.SendAsync(new ArraySegment<byte>(buffer), endOfMessage, WebSocketMessageType.Binary);
        }

        /// <summary>
        ///     Sends data to the client with the text message type
        /// </summary>
        /// <param name="buffer">Data to send</param>
        /// <param name="endOfMessage">End of the message?</param>
        /// <returns>Task to send the data</returns>
        public Task SendAsyncText(byte[] buffer, bool endOfMessage) {
            return this.SendAsync(new ArraySegment<byte>(buffer), endOfMessage, WebSocketMessageType.Text);
        }

        /// <summary>
        ///     Sends data to the client
        /// </summary>
        /// <param name="buffer">Data to send</param>
        /// <param name="endOfMessage">End of the message?</param>
        /// <param name="type">Message type of the data</param>
        /// <returns>Task to send the data</returns>
        public Task SendAsync(ArraySegment<byte> buffer, bool endOfMessage, WebSocketMessageType type) {
            var sendContext = new SendContext {Buffer = buffer, EndOfMessage = endOfMessage, Type = type};

            return this._sendQueue.Enqueue(
                async s => {
                    await this._webSocket.SendAsync(s.Buffer, s.Type, s.EndOfMessage, CancellationToken.None);
                },
                sendContext);
        }

        /// <summary>
        ///     Close the websocket connection using the close handshake
        /// </summary>
        /// <param name="status">Reason for closing the websocket connection</param>
        /// <param name="statusDescription">Human readable explanation of why the connection was closed</param>
        public Task CloseConnection(WebSocketCloseStatus status, string statusDescription) {
            return this._webSocket.CloseAsync(status, statusDescription, CancellationToken.None);
        }

        /// <summary>
        ///     Fires after the websocket has been opened with the client
        /// </summary>
        public virtual void OnOpen() {}

        /// <summary>
        ///     Fires when data is received from the client
        /// </summary>
        /// <param name="message">Data that was received</param>
        /// <param name="type">Message type of the data</param>
        public virtual void OnMessageReceived(ArraySegment<byte> message, WebSocketMessageType type) {}

        /// <summary>
        ///     Fires with the connection with the client has closed
        /// </summary>
        /// <param name="closeStatus">Status for the web socket close status</param>
        /// <param name="closeDescription">Description for the web socket close</param>
        public virtual void OnClose(WebSocketCloseStatus closeStatus, string closeDescription) {}

        /// <summary>
        ///     Receive one entire message from the web socket
        /// </summary>
        protected async Task<Tuple<ArraySegment<byte>, WebSocketMessageType>> ReceiveOneMessage(byte[] buffer) {
            var count = 0;
            WebSocketReceiveResult result;
            do {
                var segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                result = await this._webSocket.ReceiveAsync(segment, this._cancellationToken.Token);

                count += result.Count;
            } while (!result.EndOfMessage);

            return new Tuple<ArraySegment<byte>, WebSocketMessageType>(new ArraySegment<byte>(buffer, 0, count),
                result.MessageType);
        }

        internal void AcceptSocket(IOwinContext context, IDictionary<string, string> argumentMatches) {
            var accept =
                context.Get<Action<IDictionary<string, object>, Func<IDictionary<string, object>, Task>>>(
                    "websocket.Accept");
            if (accept == null) {
                throw new InvalidOperationException("Not a web socket request");
            }

            this.Arguments = new Dictionary<string, string>(argumentMatches);

            accept(null, this.RunWebSocket);
        }

        private async Task RunWebSocket(IDictionary<string, object> websocketContext) {
            this._webSocket = new OwinWebSocket(websocketContext);

            this.OnOpen();

            var buffer = new byte[this.MaxMessageSize];
            do {
                try {
                    var received = await this.ReceiveOneMessage(buffer);
                    if (received.Item1.Count > 0) {
                        this.OnMessageReceived(received.Item1, received.Item2);
                    }
                }
                catch (TaskCanceledException) {
                    break;
                }
            } while (!this._webSocket.CloseStatus.HasValue);

            await
                this._webSocket.CloseAsync(this._webSocket.CloseStatus.GetValueOrDefault(WebSocketCloseStatus.Empty),
                    this._webSocket.CloseStatusDescription, this._cancellationToken.Token);

            this._cancellationToken.Cancel();

            this.OnClose(this._webSocket.CloseStatus.GetValueOrDefault(WebSocketCloseStatus.Empty),
                this._webSocket.CloseStatusDescription);
        }
    }

    internal class SendContext {
        public ArraySegment<byte> Buffer;
        public bool EndOfMessage;
        public WebSocketMessageType Type;
    }
}