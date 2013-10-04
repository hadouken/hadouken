module Hadouken.Http {
    export class JsonRpcClient {
        private _url: string;
        private _requestId: number = 0;

        constructor(url: string) {
            this._url = url;
        }

        call(method: string, callback: { (result: any): void; }): void {
            this.callParams(method, null, callback);
        }

        callParams(method: string, params: any, callback: { (result: any): void; }): void {
            this._requestId += 1;

            var data = {
                id: this._requestId,
                jsonrpc: "2.0",
                method: method,
                params: params
            };

            var jsonData = JSON.stringify(data);

            $.post(this._url, jsonData, (response: any) => {
                if (typeof response.result === "undefined")
                    return;

                callback(response.result);
            });
        }
    }
}